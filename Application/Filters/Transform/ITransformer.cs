using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MyPhotoshop
{
    public interface ITransformer<TParameters> where TParameters : new()
    {
        void Normalize(Size size, TParameters parameters);
        Size ResultSize { get; }
        Point? MapPoint(Point newPoint);
    }
}
