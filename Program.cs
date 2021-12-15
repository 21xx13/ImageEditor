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
            //container.Bind(x => x.FromThisAssembly().SelectAllClasses().InheritedFrom<IParameters>().BindAllInterfaces());         
            var window = container.Get<MainWindow>();

            window.AddFilter(new PixelFilter<LighteningParameters>("Осветление/затемнение",
                (pixel, parameters) => Pixel.Trim(pixel * (parameters.Coefficient/50))));
            window.AddFilter(new PixelFilter<ColorParameters>("Цветовой баланс",
                (pixel, parameters) =>
                {
                    var r = Pixel.Trim(pixel.R + parameters.R * 128 / 100);
                    var g = Pixel.Trim(pixel.G + parameters.G * 128 / 100);
                    var b = Pixel.Trim(pixel.B + parameters.B * 128 / 100);
                    return new Pixel(r, g, b);
                }));
            window.AddFilter(new PixelFilter<ContrastParameters>("Контрастность",
                (pixel, parameters) =>
                {
                    return Pixel.Trim((pixel * 100 - 128 * parameters.Coefficient) / (100 - parameters.Coefficient));
                }));
            window.AddFilter(new PixelFilter<EmptyParameters>("Оттенки серого",
                (pixel, parameters) =>
                {
                    var brightness = (0.299 * (pixel.R)) + 0.587 * (pixel.G) + 0.114 * (pixel.B);
                    return Pixel.Trim(new Pixel(brightness, brightness, brightness));
                }));
            window.AddFilter(new PixelFilter<EmptyParameters>("Сепия",
                (pixel, parameters) =>
                {
                    return Pixel.Trim(new Pixel(
                        (0.393 * (pixel.R)) + 0.769 * (pixel.G) + 0.189 * (pixel.B), 
                        (0.349 * (pixel.R)) + 0.686 * (pixel.G) + 0.168 * (pixel.B), 
                        (0.272 * (pixel.R)) + 0.534 * (pixel.G) + 0.131 * (pixel.B)
                        ));
                }));
            window.AddFilter(new PixelFilter<EmptyParameters>("Негатив",
                (pixel, parameters) => Pixel.Trim(255 - pixel)));
            window.AddFilter(new TransformFilter("Отразить по горизонтали", size => size,
                (point, size) => new Point(size.Width - point.X - 1, point.Y)
                ));
            window.AddFilter(new TransformFilter("Повернуть по ч.с.", size => new Size(size.Height, size.Width),
                (point, size) => new Point(point.Y, point.X)));
            window.AddFilter(new TransformFilter<RotationParameters>("Свободное вращение",
                new RotateTransformer()));
            window.AddFilter(new TransformFilter<ScaleParameters>("Увеличить/уменьшить",
                new ScaleTransformer()));
            Application.Run(window);
        }
    }
}
