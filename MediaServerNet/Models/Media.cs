using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MediaServerNet.Models
{
    /// <summary> Represents a photo loaded from an on-disk file or remote location. </summary>
    public sealed class Media
    {
        /// <summary> The source path. </summary>
        private readonly string _path;

        /// <summary> The thumbnail. </summary>
        public readonly Image Small;

        public const int MaxSmallDimension = 600;

        /// <summary> The MD5 hash of the file. </summary>
        public readonly string Hash;

        /// <summary> The size, in bytes, of the file. </summary>
        public readonly long Size;

        /// <summary> The name of the file. </summary>
        public readonly string Filename;

        public void WriteOriginal(Stream stream)
        {
            using (var input = File.OpenRead(_path))
                input.CopyTo(stream);
        }

        public void WriteSmall(Stream stream)
        {
            Small.Save(stream, ImageFormat.Jpeg);
        }

        /// <summary> Load the photo from an on-disk path. </summary>
        public Media(string path)
        {
            _path = path;
            Filename = Path.GetFileName(path);
            if (Filename == null) 
                throw new ArgumentException(@"Filename needed", path);

            using (var file = File.OpenRead(path))
            {
                var hash = MD5.Create().ComputeHash(file);
                var sb = new StringBuilder();
                foreach (var b in hash) sb.AppendFormat("{0:x2}", b);
                Hash = sb.ToString();
                Size = file.Length;
            }

            using (var file = File.OpenRead(path))
            {
                var original = Image.FromStream(file);
                var ow = original.Width;
                var oh = original.Height;

                int sw, sh;
                if (ow > oh)
                {
                    sw = MaxSmallDimension;
                    sh = oh*MaxSmallDimension/ow;
                }
                else
                {
                    sh = MaxSmallDimension;
                    sw = ow*MaxSmallDimension/oh;
                }

                Small = ResizeImage(original, sw, sh);
            }
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public void RotateLeft()
        {
            if (Small != null)
                Small.RotateFlip(RotateFlipType.Rotate270FlipNone);
        }

        public void RotateRight()
        {
            if (Small != null)
                Small.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }
    }
}
