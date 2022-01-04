using System;

namespace MyPhotoshop
{
    public class ParameterData: Attribute
    {
        public string Name;
        public double DefaultValue;
        public double MinValue = 0;
        public double MaxValue = 1;
        public double Increment;
    }
}
