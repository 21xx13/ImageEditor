using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyPhotoshop
{
    public class ParametersHandler<TParameters> : IParametersHandler<TParameters>
        where TParameters: new()
    {
        static PropertyInfo[] properties;
        static ParameterInfo[] descriptions;

        static ParametersHandler()
        {
            properties = typeof(TParameters).GetProperties()
                .Where(x => x.GetCustomAttributes(typeof(ParameterInfo), false).Length > 0).ToArray();

            descriptions = typeof(TParameters).GetProperties()
                   .Select(x => x.GetCustomAttributes(typeof(ParameterInfo), false))
                   .Where(x => x.Length > 0)
                   .Select(z => z[0])
                   .Cast<ParameterInfo>()
                   .ToArray();
        }
        public TParameters CreateParameters(double[] values)
        {
            var parameters = new TParameters();
            if (properties.Length != values.Length)
                throw new ArgumentException();
            for (int i = 0; i < values.Length; i++)
                properties[i].SetValue(parameters, values[i], new object[0]);

            return parameters;
        }

        public ParameterInfo[] GetDescription() => descriptions;
    }
}
