using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaServerNet.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace MediaServerNet.Models
{
    public sealed class Album
    {
        #region Progress

        /// <summary> How many bytes to we expect to read/write ? </summary>
        public long TotalBytesExpected { get; private set; }

        /// <summary> How many bytes have we read/written so far ? </summary>
        public long TotalBytesSofar { get; private set; }

        /// <summary> The current work status. </summary>
        public string Status { get; private set; }

        public delegate void OnProgressHandler(Album album);

        public event OnProgressHandler Progress;

        private void OnProgress(Album album)
        {
            var handler = Progress;
            if (handler != null && !_cts.IsCancellationRequested) handler(album);
        }

        #endregion

        #region Azure sync

        /// <summary> True if files are being downloaded from azure. </summary>
        private bool _azureDownload = true;

        /// <summary> Spawns a background thread that syncs with Azure. </summary>
        private async Task StartAzureSync(CancellationToken cancel)
        {
            var azure = Azure.Account.CreateCloudBlobClient();
            var container = azure.GetContainerReference("album-" + Hash);
            try
            {
                if (File.Exists(Path.Combine(_path, "uploading.lock")))
                {
                    // Upload lock exists: previous upload was interrupted, so resume it
                    // and do not download from Azure (album.json overwrite risk)
                    foreach (var file in Directory.EnumerateFiles(_path))
                    {
                        if (file.EndsWith(".lock")) continue;
                        _toUpload.Enqueue(file);
                    }
                }
                else if (await container.ExistsAsync(cancel))
                {
                    // Step 1: reading from azure any existing data (except zip)

                    var files = container.ListBlobs()
                        .OfType<CloudBlockBlob>()
                        .Where(b => !b.Name.EndsWith(".min.jpg") && b.Name.EndsWith(".jpg") || b.Name.EndsWith(".json"))
                        .ToArray();

                    var totalSize = files.Sum(b => b.Properties.Length);
                    TotalBytesExpected += totalSize;

                    Status = "Downloading from Azure";

                    foreach (var file in files)
                    {
                        OnProgress(this);

                        var filepath = Path.Combine(_path, file.Name);
                        if (file.Name.EndsWith(".jpg") && File.Exists(filepath))
                        {
                            // No need to download original imagess which are already present
                            // (because they are keyed by MD5)
                            TotalBytesSofar += file.Properties.Length;
                            continue;
                        }

                        Status = "Downloading " + file.Name;

                        using (var input = await file.OpenReadAsync(cancel))
                        using (var output = File.OpenWrite(filepath)) 
                            await Copy(input, output, cancel);
                    }

                    OnProgress(this);
                }
                else
                {
                    await container.CreateIfNotExistsAsync(
                        BlobContainerPublicAccessType.Blob,
                        new BlobRequestOptions(),
                        new OperationContext(),
                        cancel);
                }

                var jsonPath = Path.Combine(_path, "album.json");
                if (File.Exists(jsonPath))
                    using (var stream = File.OpenRead(jsonPath))
                    using (var reader = new StreamReader(stream))
                        _info = JsonSerializer.Create().Deserialize<AlbumInfo>(new JsonTextReader(reader));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }

            _azureDownload = false;

            // Step 2: wait for updates and synchronize them            
            while (!cancel.IsCancellationRequested)
            {
                try
                {
                    await PerformAzureUploads(container);
                    await Task.Delay(1000, cancel);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }
            }
        }

        /// <summary> Wait for uploads and synchronize them. </summary>
        private async Task PerformAzureUploads(CloudBlobContainer container)
        {
            // The name of files to be uploaded, and their size
            var toUpload = new Dictionary<string, long>();
            var locked = false;
            
            while (true)
            {
                long expectedVariation = 0;

                // Move files from '_toUpload' to 'toUpload'
                string nextFile;
                while (_toUpload.TryDequeue(out nextFile))
                {
                    nextFile = Path.GetFileName(nextFile) ?? nextFile;

                    var path = Path.Combine(_path, nextFile);
                    if (!File.Exists(path))
                    {
                        _toUpload.Enqueue(path);
                        await Task.Delay(500);
                        continue;
                    }

                    long oldSize;
                    if (toUpload.TryGetValue(nextFile, out oldSize))
                        expectedVariation -= oldSize;

                    var size = new FileInfo(path).Length;

                    Trace.WriteLine("Dequeue file: " + nextFile);

                    locked = true;
                    File.WriteAllText(Path.Combine(_path, "uploading.lock"), "");

                    toUpload[nextFile] = size;
                    expectedVariation += size;
                }

                if (expectedVariation != 0)
                {
                    TotalBytesExpected += expectedVariation;
                    OnProgress(this);
                }

                // Nothing to upload: completed! 
                if (toUpload.Count == 0)
                {
                    TotalBytesExpected = 0;
                    TotalBytesSofar = 0;

                    if (locked)
                        File.Delete(Path.Combine(_path, "uploading.lock"));

                    return;
                }

                // Pick a file and upload it

                var upload = toUpload.First();
                toUpload.Remove(upload.Key);

                var localPath = Path.Combine(_path, upload.Key);

                var blob = container.GetBlockBlobReference(upload.Key);
                var hasMd5Key = upload.Key.EndsWith(".jpg") && !upload.Key.EndsWith(".min.jpg");
                if (!hasMd5Key || !await blob.ExistsAsync())
                {
                    Status = "Send file: " + upload.Key;

                    // Only perform upload if necessary
                    using (var output = await blob.OpenWriteAsync())
                    using (var input = File.OpenRead(localPath))
                        await Copy(input, output);
                }
                else
                {
                    TotalBytesSofar += upload.Value;
                }

                OnProgress(this);
            }
        }

        /// <summary> Async copy with progress bar tracking. </summary>
        private async Task Copy(Stream input, Stream output, CancellationToken cancel = default(CancellationToken))
        {
            var buffer = new byte[10240];
            while (true)
            {
                cancel.ThrowIfCancellationRequested();

                var length = await input.ReadAsync(buffer, 0, buffer.Length, cancel);
                if (length == 0) return;

                await output.WriteAsync(buffer, 0, length, cancel);

                TotalBytesSofar += length;
                OnProgress(this);
            }
        }

        /// <summary> Files (paths) to be uploaded. </summary>
        private readonly ConcurrentQueue<string> _toUpload = new ConcurrentQueue<string>();

        #endregion

        /// <summary> The album name. </summary>
        public readonly string Name;

        /// <summary> The on-disk path of this album. </summary>
        private readonly string _path;

        /// <summary> How many local-to-local copies pending ? </summary>
        private int _localCopyPending;

        /// <summary> Information about the album, as currently saved on-disk. </summary>
        private AlbumInfo _info;

        /// <summary> The album's hash. Deterministic, extracted from name.  </summary>
        public string Hash
        {
            get
            {
                var salt = ConfigurationManager.AppSettings["Salt"];
                var algo = HMAC.Create("HMACSHA1");
                algo.Key = Encoding.UTF8.GetBytes(salt);
                var hash = algo.ComputeHash(Encoding.UTF8.GetBytes(Name));

                var sb = new StringBuilder();
                foreach (var b in hash) sb.AppendFormat("{0:x2}", b);
                return sb.ToString();
            }
        }

        #region Disconnecting

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        /// <summary>
        /// Interrupt all operations on the album. Waits until current downloads are
        /// finished.
        /// </summary>
        public void Disconnect()
        {
            _cts.Cancel();
        }

        #endregion

        public Album(string path)
        {
            _path = path;
            Directory.CreateDirectory(_path);

            Name = Path.GetFileName(_path);
            if (Name == null) throw new ArgumentException(@"Path with no filename", _path);

            _info = new AlbumInfo(Name, Hash, new MediaInfo[0]);

            Task.Run(() => StartAzureSync(_cts.Token));
        }

        #region Adding medias to the album

        /// <summary> Pictures added to the album on this iteration. </summary>
        private readonly ConcurrentQueue<MediaInfo> _newMedias = new ConcurrentQueue<MediaInfo>();

        public void Add(Media media, bool hidden = false)
        {
            _newMedias.Enqueue(new MediaInfo(
                media.Filename,
                media.Hash,
                hidden, 
                media is Movie));

            Interlocked.Increment(ref _localCopyPending);

            Task.Run(() =>
            {
                var originalPath = Path.Combine(_path, media.Hash + media.Extension);
                var smallPath = Path.Combine(_path, media.Hash + ".min.jpg");

                _toUpload.Enqueue(originalPath);
                _toUpload.Enqueue(smallPath);

                if (!File.Exists(originalPath)) 
                    using (var s = File.OpenWrite(originalPath))
                        media.WriteOriginal(s);

                if (!File.Exists(smallPath))
                    using (var s = File.OpenWrite(smallPath))
                        media.WriteThumbnail(s);

                Interlocked.Decrement(ref _localCopyPending);
            });
        }

        #endregion

        #region Saving (to disk)

        /// <summary> How many pending requests to save to disk ? </summary>
        private int _savingToDisk;

        /// <summary> Save the album to disk. </summary>
        /// <remarks> This will trigger a sync-to-azure. </remarks>
        public void SaveToDisk()
        {
            lock (this)
            {
                _savingToDisk++;
                if (_savingToDisk > 1) return;
            }

            Task.Run(async () =>
            {
                // Loop until no more 'save' requets pending.
                while (true)
                {
                    while (_localCopyPending > 0 && _azureDownload) 
                        await Task.Delay(500);

                    var pics = _info.Medias.ToList();
                    var picsByHash = pics.Select((p, i) => new KeyValuePair<string, int>(p.Hash, i))
                        .GroupBy(kv => kv.Key)
                        .ToDictionary(g => g.Key, g => g.First().Value);

                    MediaInfo next;
                    while (_newMedias.TryDequeue(out next))
                    {
                        int pos;
                        if (!picsByHash.TryGetValue(next.Hash, out pos))
                        {
                            pos = pics.Count;
                            pics.Add(next);
                            picsByHash.Add(next.Hash, pos);
                        }
                        else
                        {
                            pics[pos] = next;
                        }
                    }

                    _info = new AlbumInfo(
                        _info.Name,
                        _info.Hash,
                        pics);

                    using (var stream = File.OpenWrite(Path.Combine(_path, "album.json")))
                    using (var writer = new StreamWriter(stream))
                        JsonSerializer.Create().Serialize(writer, _info);

                    _toUpload.Enqueue("album.json");

                    var zipPath = Path.GetTempFileName();
                    File.Delete(zipPath);

                    try
                    {
                        using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                        {
                            var written = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            foreach (var file in _info.Medias)
                            {
                                if (file.IsHidden) continue;
                                if (written.Contains(file.Name)) continue;
                                written.Add(file.Name);

                                var source = Path.Combine(_path, file.Hash + (file.IsMovie ? ".mov" : ".jpg"));
                                zip.CreateEntryFromFile(source, file.Name, CompressionLevel.NoCompression);
                            }
                        }

                        File.Copy(zipPath, Path.Combine(_path, "album.zip"), true);

                        _toUpload.Enqueue("album.zip");
                    }
                            
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch {}

                    lock (this)
                    {
                        if (--_savingToDisk == 0) return;
                    }
                    
                }
            });
        }

        #endregion
    }
}
