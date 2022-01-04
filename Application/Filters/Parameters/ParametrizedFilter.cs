namespace MyPhotoshop
{
    public abstract class ParametrizedFilter<TParameters> : IFilter
        where TParameters : new()
    {
        readonly ParametersHandler<TParameters> handler = new ParametersHandler<TParameters>();
        public ParameterData[] GetParameters() => handler.GetDescription();

        public Photo Process(Photo photo, double[] values)
        {
            var parameters = handler.CreateParameters(values);
            return Process(photo, parameters);
        }

        public abstract Photo Process(Photo photo, TParameters parameters);
    }
}
