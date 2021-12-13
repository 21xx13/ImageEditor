using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPhotoshop
{
    public class ScaleParameters
    {
        [ParameterInfo(Name = "Ширина (в процентах)", MaxValue = 2000, MinValue = 1, Increment = 1, DefaultValue = 500,
            IsNumeric = true)]
        public double ScaleX { get; set; }
        [ParameterInfo(Name = "Высота (в процентах)", MaxValue = 2000, MinValue = 1, Increment = 1, DefaultValue = 500,
            IsNumeric = true)]
        public double ScaleY { get; set; }
    }
}
