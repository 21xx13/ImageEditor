using System;
using System.Linq;
using System.Reflection;

namespace MyPhotoshop
{
    public class ParametersHandler<TParameters>
        where TParameters: new()
    {
        readonly static PropertyInfo[] properties;
        readonly static ParameterData[] descriptions;

        static ParametersHandler()
        {
            properties = typeof(TParameters).GetProperties()
                .Where(x => x.GetCustomAttributes(typeof(ParameterData), false).Length > 0).ToArray();

            descriptions = typeof(TParameters).GetProperties()
                   .Select(x => x.GetCustomAttributes(typeof(ParameterData), false))
                   .Where(x => x.Length > 0)
                   .Select(z => z[0])
                   .Cast<ParameterData>()
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

        public ParameterData[] GetDescription() => descriptions;
    }
}
