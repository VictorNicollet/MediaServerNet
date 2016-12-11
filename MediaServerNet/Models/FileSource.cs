using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaServerNet.Models
{
    /// <summary> Provides access to the image files in a directory. </summary>
    public sealed class FileSource
    {
        /// <summary> The path of the directory mapped to this source. </summary>
        public readonly string FromPath;

        /// <summary> The number of pictures in the directory. </summary>
        public int PictureCount => _mediaNames.Count;

        /// <summary> The names of the pictures in the directory. </summary>
        private readonly IReadOnlyList<string> _mediaNames;

        public FileSource(string fromPath)
        {
            FromPath = fromPath;

            try
            {
                _mediaNames = Directory.EnumerateFiles(fromPath)
                    .Where(file => 
                        file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || 
                        file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                        file.EndsWith(".mov", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }
            catch
            {
                _mediaNames = new string[0];
            }

            Task.Run(() =>
            {
                for (var i = 0; i < _mediaNames.Count; ++i)
                {
                    MediaAt(i);
                }
            });
        }

        /// <summary> Return the photo at the specified location, from cache if possible. </summary>
        private Media MediaAt(int i)
        {
            return _contents.GetOrAdd(i, j =>
            {
                try
                {
                    return Media.Load(Path.Combine(FromPath, _mediaNames[j]));
                }
                catch
                {
                    return null;
                }
            });
        }

        /// <summary> A cache of all photos in this dictionary. </summary>
        private readonly ConcurrentDictionary<int, Media> _contents = 
            new ConcurrentDictionary<int, Media>();

        /// <summary> Return the photo at the specified location, from cache if possible. </summary>
        public Media this[int i] => MediaAt(i);
    }
}
