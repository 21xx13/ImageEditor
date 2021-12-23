using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPhotoshop
{
    public class LighteningParameters : IParameters
    {
        [ParameterData(Name = "Коэффициент", MaxValue = 100, MinValue = 0, Increment = 1, DefaultValue = 50)]
        public double Coefficient { get; set; }
    }
}
