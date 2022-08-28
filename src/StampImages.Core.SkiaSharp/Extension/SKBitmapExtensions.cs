using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StampImages.Core.SkiaSharp
{
    public static class SKBitmapExtensions
    {

        public static void Save(this SKBitmap bitmap, string fileName)
        {

            if (!fileName.ToLower().EndsWith(".png"))
            {
                fileName += ".png";
            }

            using (var image = SKImage.FromBitmap(bitmap))
            using (var stream = File.Create(fileName))
            {
                var data = image.Encode(SKEncodedImageFormat.Png, 100);
                data.SaveTo(stream);
            }

        }
    }
}
