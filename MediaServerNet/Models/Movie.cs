using System.Drawing;
using System.IO;
using NReco.VideoConverter;

namespace MediaServerNet.Models
{
    public sealed class Movie : Media
    {
        private Movie(string path, Image small) : base(path, small) {}

        public static Movie Make(string path)
        {
            var converter = new FFMpegConverter();
            var image = new MemoryStream();
            converter.GetVideoThumbnail(path, image);
            image.Position = 0;
            return new Movie(path, Smaller(image));
        }

        public override string Extension => ".mov";
    }
}
