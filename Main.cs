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
            window.AddFilter(new PixelFilter<LighteningParameters>("����������/����������",
                (pixel, parameters) => pixel * (parameters.Coefficient/50)));
            window.AddFilter(new PixelFilter<ColorParameters>("�������� ������",
                (pixel, parameters) =>
                {
                    var r = Pixel.Trim(pixel.R + parameters.R * 128 / 100);
                    var g = Pixel.Trim(pixel.G + parameters.G * 128 / 100);
                    var b = Pixel.Trim(pixel.B + parameters.B * 128 / 100);
                    return new Pixel(r, g, b);
                }));
            window.AddFilter(new PixelFilter<ContrastParameters>("�������������",
                (pixel, parameters) =>
                {
                    var r = Pixel.Trim((pixel.R * 100 - 128 * parameters.Coefficient) / (100 - parameters.Coefficient));
                    var g = Pixel.Trim((pixel.G * 100 - 128 * parameters.Coefficient) / (100 - parameters.Coefficient));
                    var b = Pixel.Trim((pixel.B * 100 - 128 * parameters.Coefficient) / (100 - parameters.Coefficient));
                    return new Pixel(r, g, b);
                }));
            window.AddFilter(new PixelFilter<EmptyParameters>("������� ������",
                (pixel, parameters) =>
                {
                    var brightness = (0.299 * (pixel.R)) + 0.587 * (pixel.G) + 0.114 * (pixel.B);
                    return new Pixel(brightness, brightness, brightness);
                }));
            window.AddFilter(new PixelFilter<EmptyParameters>("�����",
                (pixel, parameters) =>
                {
                    var r = Pixel.Trim((0.393 * (pixel.R)) + 0.769 * (pixel.G) + 0.189 * (pixel.B));
                    var g = Pixel.Trim((0.349 * (pixel.R)) + 0.686 * (pixel.G) + 0.168 * (pixel.B));
                    var b = Pixel.Trim((0.272 * (pixel.R)) + 0.534 * (pixel.G) + 0.131 * (pixel.B));
                    return new Pixel(r, g, b);
                }));
            window.AddFilter(new PixelFilter<EmptyParameters>("�������",
                (pixel, parameters) =>
                {
                    return new Pixel(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                }));

            window.AddFilter(new TransformFilter("�������� �� �����������", size => size,
                (point, size) => new Point(size.Width - point.X - 1, point.Y)
                ));
            window.AddFilter(new TransformFilter("��������� �� �.�.", size => new Size(size.Height, size.Width),
                (point, size) => new Point(point.Y, point.X)));


            window.AddFilter(new TransformFilter<RotationParameters>("��������� ��������",
                new RotateTransformer()));
            Application.Run(window);
        }
    }
}
