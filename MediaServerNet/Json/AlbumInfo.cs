using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MediaServerNet.Json
{
    [DataContract]
    public sealed class AlbumInfo
    {
        /// <summary> The title of the album. </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary> The hash of the album's name (with private salt). </summary>
        /// <remarks> This is sloppy cryptography. </remarks>
        [DataMember]
        public string Hash { get; private set; }

        /// <summary> All pictures in this album. </summary>
        [DataMember]
        public MediaInfo[] Medias { get; private set; }

        public AlbumInfo(string name, string hash, IEnumerable<MediaInfo> pictures)
        {
            Name = name;
            Hash = hash;
            Medias = pictures.ToArray();
        }
    }
}
