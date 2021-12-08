﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPhotoshop
{
    public class ColorParameters
    {

        [ParameterInfo(Name = "Красный", MaxValue = 100, MinValue = 0, Increment = 1, DefaultValue = 0)]
        public double R { get; set; }
        [ParameterInfo(Name = "Зелёный", MaxValue = 100, MinValue = 0, Increment = 1, DefaultValue = 0)]
        public double G { get; set; }
        [ParameterInfo(Name = "Синий", MaxValue = 100, MinValue = 0, Increment = 1, DefaultValue = 0)]
        public double B { get; set; }
    }
}
