using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyPhotoshop
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var window = new MainWindow();
            window.AddFilter(new PixelFilter<LighteningParameters>("Осветление/затемнение",
                (pixel, parameters) => pixel * parameters.Coefficient));
            window.AddFilter(new PixelFilter<ColorParameters>("Цветовой баланс",
                (pixel, parameters) =>
                {
                    var r = Pixel.Trim(pixel.R + parameters.R / 100);
                    var g = Pixel.Trim(pixel.G + parameters.G / 100);
                    var b = Pixel.Trim(pixel.B + parameters.B / 100);
                    return new Pixel(r, g, b);
                }));
            window.AddFilter(new PixelFilter<ContrastParameters>("Контрастность",
                (pixel, parameters) =>
                {
                    var r = Pixel.Trim((pixel.R * 255 * 100 - 128 * parameters.Coefficient) / (100 - parameters.Coefficient) / 255);
                    var g = Pixel.Trim((pixel.G * 255 * 100 - 128 * parameters.Coefficient) / (100 - parameters.Coefficient) / 255);
                    var b = Pixel.Trim((pixel.B * 255 * 100 - 128 * parameters.Coefficient) / (100 - parameters.Coefficient) / 255);
                    return new Pixel(r, g, b);

                }));
            window.AddFilter(new PixelFilter<EmptyParameters>("Оттенки серого",
                (pixel, parameters) =>
                {
                    var brightness = (0.299 * (pixel.R)) +
                              0.587 * (pixel.G) + 0.114 * (pixel.B);
                    return new Pixel(brightness, brightness, brightness);
                }));

            window.AddFilter(new TransformFilter("Отразить по горизонтали", size => size,
                (point, size) => new Point(size.Width - point.X - 1, point.Y)
                ));
            window.AddFilter(new TransformFilter("Повернуть по ч.с.", size => new Size(size.Height, size.Width),
                (point, size) => new Point(point.Y, point.X)));


            window.AddFilter(new TransformFilter<RotationParameters>("Свободное вращение",
                new RotateTransformer()));
            Application.Run(window);
        }
    }
}
