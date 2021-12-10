using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MyPhotoshop
{
    public class FreeTransformer : ITransformer<EmptyParameters>
    {
        readonly Func<Size, Size> sizeTransformer;
        readonly Func<Point, Size, Point> pointTransformer;
        Size oldSize;
        public Size ResultSize { get; private set; }

        public FreeTransformer(Func<Size, Size> sizeTransformer, Func<Point, Size, Point> pointTransformer)
        {
            this.pointTransformer = pointTransformer;
            this.sizeTransformer = sizeTransformer;
        }

        public Point? MapPoint(Point newPoint) => pointTransformer(newPoint, oldSize);

        public void Prepare(Size size, EmptyParameters parameters)
        {
            oldSize = size;
            ResultSize = sizeTransformer(oldSize);
        }
    }
}
