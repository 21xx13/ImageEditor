using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPhotoshop
{
    public class ScaleParameters: IParameters
    {
        [ParameterData(Name = "Ширина (в пикселях)", MaxValue = 2000, MinValue = 1, Increment = 1, DefaultValue = 500)]
        public double ScaleX { get; set; }
        [ParameterData(Name = "Высота (в пикселях)", MaxValue = 2000, MinValue = 1, Increment = 1, DefaultValue = 500)]
        public double ScaleY { get; set; }
    }
}
