using System;

namespace MyPhotoshop
{
    public interface IFilter
    {
        ParameterData[] GetParameters();
        Photo Process(Photo original, double[] parameters);
    }
}

