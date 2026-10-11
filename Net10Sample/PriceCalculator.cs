namespace Net10Sample;

public partial class PriceCalculator
{
    public decimal CalculateFinalPrice(decimal price, decimal discountPercent)
    {
        return price - CalculateDiscountAmount(price, discountPercent);
    }

    [TestExpose]
    private decimal CalculateDiscountAmount(decimal price, decimal discountPercent)
    {
        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "가격은 0 이상이어야 합니다.");
        }

        if (discountPercent < 0 || discountPercent > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(discountPercent), "할인율은 0부터 100 사이여야 합니다.");
        }

        return decimal.Round(price * discountPercent / 100m, 2, MidpointRounding.AwayFromZero);
    }
}

