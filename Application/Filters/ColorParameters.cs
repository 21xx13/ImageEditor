using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPhotoshop
{
    public class ColorParameters: IParameters
    {

        [ParameterData(Name = "Красный", MaxValue = 100, MinValue = 0, Increment = 1, DefaultValue = 0)]
        public double R { get; set; }
        [ParameterData(Name = "Зелёный", MaxValue = 100, MinValue = 0, Increment = 1, DefaultValue = 0)]
        public double G { get; set; }
        [ParameterData(Name = "Синий", MaxValue = 100, MinValue = 0, Increment = 1, DefaultValue = 0)]
        public double B { get; set; }
    }
}
