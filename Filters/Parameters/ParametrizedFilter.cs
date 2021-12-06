using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPhotoshop
{
    public abstract class ParametrizedFilter<TParameters> : IFilter
        where TParameters : IParameters, new()
    {
        IParametersHandler<TParameters> handler = new StaticParametersHandler<TParameters>();
        public ParameterInfo[] GetParameters()
        {
            return handler.GetDescription();
        }

        public Photo Process(Photo photo, double[] values)
        {
            var parameters = handler.CreateParameters(values);
            return Process(photo, parameters);
        }

        public abstract Photo Process(Photo photo, TParameters parameters);

    }
}
