using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MyPhotoshop
{
    public class ScaleTransformer : ITransformer<ScaleParameters>
    {
        public Size OriginalSize { get; private set; }
        public Size ResultSize { get; private set; }
        public double ScaleX { get; private set; }
        public double ScaleY { get; private set; }
        public readonly bool IsIncrease;
        public ScaleTransformer(bool isIncrease)
        {
            IsIncrease = isIncrease;
        }

        public Point? MapPoint(Point point)
        {
            var x = (int)(point.X / ScaleX);
            var y = (int)(point.Y / ScaleY);
            return new Point(x, y);
        }


        public void Prepare(Size size, ScaleParameters parameters)
        {
            OriginalSize = size;
            int factor = -1;
            if (IsIncrease) factor = 1;
            ScaleX = 1 + factor * parameters.ScaleX / 100;
            ScaleY = 1 + factor * parameters.ScaleY / 100;
            ResultSize = new Size((int)(size.Width * ScaleX), (int)(size.Height * ScaleY));
        }
    }
}
