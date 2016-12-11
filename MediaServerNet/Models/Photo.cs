using System.Drawing;
using System.IO;

namespace MediaServerNet.Models
{
    public class Photo : Media
    {
        private Photo(string path, Image small) : base(path, small) {}

        public static Photo Make(string path) => 
            new Photo(path, Smaller(File.OpenRead(path)));

        public override string Extension => ".jpg";
    }
}
