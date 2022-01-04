namespace MyPhotoshop
{
    public class StandardParameters : IParameters
    {
        [ParameterData(Name = "Коэффициент", MaxValue = 100, MinValue = 0, Increment = 1, DefaultValue = 50)]
        public double Coefficient { get; set; }
    }
}
