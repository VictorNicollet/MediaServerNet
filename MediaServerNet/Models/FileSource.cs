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
        public int PictureCount { get { return _pictureNames.Count; } }

        /// <summary> The names of the pictures in the directory. </summary>
        private readonly IReadOnlyList<string> _pictureNames;

        public FileSource(string fromPath)
        {
            FromPath = fromPath;

            try
            {
                _pictureNames = Directory.EnumerateFiles(fromPath)
                    .Where(file => 
                        file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || 
                        file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }
            catch
            {
                _pictureNames = new string[0];
            }

            Task.Run(() =>
            {
                for (var i = 0; i < _pictureNames.Count; ++i)
                {
                    PhotoAt(i);
                }
            });
        }

        /// <summary> Return the photo at the specified location, from cache if possible. </summary>
        private Photo PhotoAt(int i)
        {
            return _contents.GetOrAdd(i, j =>
            {
                try
                {
                    return new Photo(Path.Combine(FromPath, _pictureNames[j]));
                }
                catch
                {
                    return null;
                }
            });
        }

        /// <summary> A cache of all photos in this dictionary. </summary>
        private readonly ConcurrentDictionary<int, Photo> _contents = 
            new ConcurrentDictionary<int, Photo>();

        /// <summary> Return the photo at the specified location, from cache if possible. </summary>
        public Photo this[int i]
        {
            get
            {
                return PhotoAt(i);
            }
        }
    }
}
