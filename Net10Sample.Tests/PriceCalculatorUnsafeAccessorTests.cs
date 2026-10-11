using System.Runtime.CompilerServices;

namespace Net10Sample.Tests;

public partial class PriceCalculatorUnsafeAccessorTests
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "CalculateDiscountAmount")]
    private static extern decimal CalculateDiscountAmount(
        PriceCalculator instance,
        decimal price,
        decimal discountPercent);

    [Theory]
    [MemberData(nameof(ValidCases))]
    public void CalculateDiscountAmount_ReturnsExpectedAmount(
        decimal price, decimal discountPercent, decimal expected)
    {
        var actual = CalculateDiscountAmount(
            new PriceCalculator(), price, discountPercent);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(InvalidCases))]
    public void CalculateDiscountAmount_InvalidInput_ThrowsForExpectedParameter(
        decimal price, decimal discountPercent, string expectedParameter)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            CalculateDiscountAmount(new PriceCalculator(), price, discountPercent);
        });

        Assert.Equal(expectedParameter, exception.ParamName);
    }
}

