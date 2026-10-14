namespace Net10Sample;

public partial class PriceCalculator
{
    private class HiddenCalculator
    {
        private HiddenCalculator() { }

        private decimal Calculate(decimal price, decimal rate)
            => price * rate / 100m;
    }
}