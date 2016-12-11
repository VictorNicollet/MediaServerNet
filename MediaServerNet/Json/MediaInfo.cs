using System.Runtime.Serialization;

namespace MediaServerNet.Json
{
    [DataContract]
    public sealed class MediaInfo
    {
        /// <summary> The filename of the picture. </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary> The MD5 hash of the picture. </summary>
        [DataMember]
        public string Hash { get; private set; }

        /// <summary> Is this picture hidden ?  </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsHidden { get; private set; }

        public MediaInfo(string name, string hash, bool isHidden)
        {
            Name = name;
            Hash = hash;
            IsHidden = isHidden;
        }
    }
}