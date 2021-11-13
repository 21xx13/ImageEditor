using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MyPhotoshop
{
    public class ExpressionParametersHandler<TParameters> : IParametersHandler<TParameters>
        where TParameters: IParameters, new()
    {
        static ParameterInfo[] descriptions;
        static Func<double[], TParameters> parser;

        static ExpressionParametersHandler()
        {

            descriptions = typeof(TParameters).GetProperties()
                   .Select(x => x.GetCustomAttributes(typeof(ParameterInfo), false))
                   .Where(x => x.Length > 0)
                   .Select(z => z[0])
                   .Cast<ParameterInfo>()
                   .ToArray();

            var arg = Expression.Parameter(typeof(double[]), "values");
            var properties = typeof(TParameters).GetProperties()
                .Where(x => x.GetCustomAttributes(typeof(ParameterInfo), false).Length > 0).ToArray();
            var bindings = new List<MemberBinding>();
            for (int i = 0; i < properties.Length; i++)
            {
                var binding = Expression.Bind(properties[i], Expression.ArrayIndex(arg, Expression.Constant(i)));
                bindings.Add(binding);
            }

            var body = Expression.MemberInit(Expression.New(typeof(TParameters).GetConstructor(new Type[0])), bindings);

            var lambda = Expression.Lambda<Func<double[], TParameters>>(body, arg);
            parser = lambda.Compile();
        }
        public TParameters CreateParameters(double[] values)
        {
            return parser(values);
        }

        public ParameterInfo[] GetDescription()
        {
            return descriptions;
        }
    }
}
