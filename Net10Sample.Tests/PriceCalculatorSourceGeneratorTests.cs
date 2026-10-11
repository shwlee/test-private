#if TEST_EXPOSE
namespace Net10Sample.Tests;

public partial class PriceCalculatorSourceGeneratorTests
{

    [Theory]
    [MemberData(nameof(ValidCases))]
    public void CalculateDiscountAmount_ReturnsExpectedAmount(
        decimal price, decimal discountPercent, decimal expected)
    {
        var sut = new PriceCalculator();
        var actual = sut.Test_CalculateDiscountAmount(price, discountPercent);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(InvalidCases))]
    public void CalculateDiscountAmount_InvalidInput_ThrowsForExpectedParameter(
        decimal price, decimal discountPercent, string expectedParameter)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var sut = new PriceCalculator();
            sut.Test_CalculateDiscountAmount(price, discountPercent);
        });

        Assert.Equal(expectedParameter, exception.ParamName);
    }
}
#endif

