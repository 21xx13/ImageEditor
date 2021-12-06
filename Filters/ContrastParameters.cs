using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPhotoshop
{
        public class ContrastParameters : IParameters
        {

            [ParameterInfo(Name = "Контрастность", MaxValue = 50, MinValue = -50, Increment = 1, DefaultValue = 0)]
            public double Coefficient { get; set; }

        }
    }
