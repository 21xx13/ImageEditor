using System;
using System.Drawing;

namespace MyPhotoshop
{
    public static class Convertors
    {
        public static Photo BitmapToPhoto(Bitmap bmp)
        {
            var photo = new Photo(bmp.Width, bmp.Height);
            for (int x = 0; x < bmp.Width; x++)
                for (int y = 0; y < bmp.Height; y++)
                {
                    var pixel = bmp.GetPixel(x, y);
                    photo[x, y] = new Pixel((double)pixel.R,(double)pixel.G, (double)pixel.B);
        }
            return photo;
        }

        static int ToChannel(double val)
        {
            if (val < 0 || val > 255)
                throw new Exception(string.Format("Некорректное значение {0} (значение должно быть между 0 и 255)", val));
            return (int)(val);
        }

        public static Bitmap PhotoToBitmap(Photo photo)
        {
            var bmp = new Bitmap(photo.Width, photo.Height);
            for (int x = 0; x < bmp.Width; x++)
                for (int y = 0; y < bmp.Height; y++)
                    bmp.SetPixel(x, y, Color.FromArgb(
                        ToChannel(photo[x, y].R),
                        ToChannel(photo[x, y].G),
                        ToChannel(photo[x, y].B)));

            return bmp;
        }
    }
}

