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

        public Point? MapPoint(Point point)
        {
            var x = (int)(point.X / (ScaleX == 0 ? 1 : ScaleX));
            var y = (int)(point.Y / (ScaleY == 0 ? 1 : ScaleY));
            return new Point(x, y);
        }

        public void Prepare(Size size, ScaleParameters parameters)
        {
            OriginalSize = size;           
            ScaleX = Math.Abs(parameters.ScaleX / size.Width) ;
            ScaleY = Math.Abs(parameters.ScaleY / size.Height) ;
            ResultSize = new Size((int)parameters.ScaleX, (int)parameters.ScaleY);
        }
    }
}
