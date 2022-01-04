namespace MyPhotoshop
{
    public class ContrastParameters : IParameters
    {
        [ParameterData(Name = "Контрастность", MaxValue = 50, MinValue = -50, Increment = 1, DefaultValue = 0)]
        public double Coefficient { get; set; }
    }
}
