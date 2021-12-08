using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPhotoshop
{
    public class LighteningParameters 
    {
        
        [ParameterInfo(Name = "Коэффициент", MaxValue = 100, MinValue = 0, Increment = 1, DefaultValue = 10)]
        public double Coefficient { get; set; }
    }
}
