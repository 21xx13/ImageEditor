using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPhotoshop
{
    public class ScaleParameters
    {
        [ParameterInfo(Name = "Ширина", MaxValue = 100, MinValue = 0, Increment = 1, DefaultValue = 0)]
        public double ScaleX { get; set; }
        [ParameterInfo(Name = "Высота", MaxValue = 100, MinValue = 0, Increment = 1, DefaultValue = 0)]
        public double ScaleY { get; set; }
    }
}
