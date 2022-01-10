using System;
using System.Drawing;

namespace MyPhotoshop
{
    public static class Filters
    {
        [Filter]
        public static IFilter LighteningFilter
        {
            get => new PixelFilter<StandardParameters>("Осветление/затемнение",
                (pixel, parameters) => Pixel.Trim(pixel * (parameters.Coefficient / 50)));
        }

        [Filter]
        public static IFilter ColorFilter
        {
            get => new PixelFilter<ColorParameters>("Цветовой баланс",
                (pixel, parameters) =>
                {
                    var r = Pixel.Trim(pixel.R + parameters.R * 128 / 100);
                    var g = Pixel.Trim(pixel.G + parameters.G * 128 / 100);
                    var b = Pixel.Trim(pixel.B + parameters.B * 128 / 100);
                    return new Pixel(r, g, b);
                });
        }

        [Filter]
        public static IFilter ContrastFilter
        {
            get => new PixelFilter<ContrastParameters>("Контрастность",
                (pixel, parameters) =>
                {
                    return Pixel.Trim((pixel * 100 - 128 * parameters.Coefficient) / (100 - parameters.Coefficient));
                });
        }

        [Filter]
        public static IFilter GrayFilter
        {
            get => new PixelFilter<EmptyParameters>("Оттенки серого",
                (pixel, parameters) =>
                {
                    var brightness = (0.299 * (pixel.R)) + 0.587 * (pixel.G) + 0.114 * (pixel.B);
                    return Pixel.Trim(new Pixel(brightness, brightness, brightness));
                });
        }

        [Filter]
        public static IFilter SepiaFilter
        {
            get => new PixelFilter<EmptyParameters>("Сепия",
                (pixel, parameters) =>
                {
                    return Pixel.Trim(new Pixel(
                    (0.393 * (pixel.R)) + 0.769 * (pixel.G) + 0.189 * (pixel.B),
                    (0.349 * (pixel.R)) + 0.686 * (pixel.G) + 0.168 * (pixel.B),
                    (0.272 * (pixel.R)) + 0.534 * (pixel.G) + 0.131 * (pixel.B)
                    ));
                });
        }

        [Filter]
        public static IFilter NegativeFilter
        {
            get => new PixelFilter<EmptyParameters>("Негатив", (pixel, parameters) => Pixel.Trim(255 - pixel));
        }

        [Filter]
        public static IFilter HorizontalFlip
        {
            get => new TransformFilter("Отразить по горизонтали", size => size,
            (point, size) => new Point(size.Width - point.X - 1, point.Y)
            );
        }

        [Filter]
        public static IFilter ClockwiseRotate
        {
            get => new TransformFilter("Повернуть по ч.с.", size => new Size(size.Height, size.Width),
            (point, size) => new Point(point.Y, point.X));
        }

        [Filter]
        public static IFilter RotateFilter
        {
            get => new TransformFilter<RotationParameters>("Свободное вращение",
            new RotateTransformer());
        }

        [Filter]
        public static IFilter ScaleFilter
        {
            get => new TransformFilter<ScaleParameters>("Увеличить/уменьшить",
            new ScaleTransformer());
        }
    }
}
