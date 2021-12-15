using System;
using System.Drawing;
using System.Windows.Forms;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Extensions.Conventions;
namespace MyPhotoshop
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var container = new StandardKernel();
            container.Bind<MainWindow>().ToSelf().InSingletonScope()
                .OnActivation(w =>
                {
                    w.AddFilter(new PixelFilter<LighteningParameters>("Осветление/затемнение",
                        (pixel, parameters) => Pixel.Trim(pixel * (parameters.Coefficient / 50))));
                    w.AddFilter(new PixelFilter<ColorParameters>("Цветовой баланс",
                        (pixel, parameters) =>
                        {
                            var r = Pixel.Trim(pixel.R + parameters.R * 128 / 100);
                            var g = Pixel.Trim(pixel.G + parameters.G * 128 / 100);
                            var b = Pixel.Trim(pixel.B + parameters.B * 128 / 100);
                            return new Pixel(r, g, b);
                        }));
                    w.AddFilter(new PixelFilter<ContrastParameters>("Контрастность",
                        (pixel, parameters) =>
                        {
                            return Pixel.Trim((pixel * 100 - 128 * parameters.Coefficient) / (100 - parameters.Coefficient));
                        }));
                    w.AddFilter(new PixelFilter<EmptyParameters>("Оттенки серого",
                        (pixel, parameters) =>
                        {
                            var brightness = (0.299 * (pixel.R)) + 0.587 * (pixel.G) + 0.114 * (pixel.B);
                            return Pixel.Trim(new Pixel(brightness, brightness, brightness));
                        }));
                    w.AddFilter(new PixelFilter<EmptyParameters>("Сепия",
                        (pixel, parameters) =>
                        {
                            return Pixel.Trim(new Pixel(
                                (0.393 * (pixel.R)) + 0.769 * (pixel.G) + 0.189 * (pixel.B),
                                (0.349 * (pixel.R)) + 0.686 * (pixel.G) + 0.168 * (pixel.B),
                                (0.272 * (pixel.R)) + 0.534 * (pixel.G) + 0.131 * (pixel.B)
                                ));
                        }));
                    w.AddFilter(new PixelFilter<EmptyParameters>("Негатив",
                        (pixel, parameters) => Pixel.Trim(255 - pixel)));
                    w.AddFilter(new TransformFilter("Отразить по горизонтали", size => size,
                        (point, size) => new Point(size.Width - point.X - 1, point.Y)
                        ));
                    w.AddFilter(new TransformFilter("Повернуть по ч.с.", size => new Size(size.Height, size.Width),
                        (point, size) => new Point(point.Y, point.X)));
                    w.AddFilter(new TransformFilter<RotationParameters>("Свободное вращение",
                        new RotateTransformer()));
                    w.AddFilter(new TransformFilter<ScaleParameters>("Увеличить/уменьшить",
                        new ScaleTransformer()));
                });
            var window = container.Get<MainWindow>();
            Application.Run(window);
        }
    }
}
