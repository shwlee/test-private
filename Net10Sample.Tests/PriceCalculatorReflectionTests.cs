using System.Reflection;

namespace Net10Sample.Tests;

public partial class PriceCalculatorReflectionTests
{
    private static readonly MethodInfo DiscountMethod =
        typeof(PriceCalculator).GetMethod(
            "CalculateDiscountAmount",
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(decimal), typeof(decimal) },
            modifiers: null)
        ?? throw new MissingMethodException(
            typeof(PriceCalculator).FullName,
            "CalculateDiscountAmount");

    [Theory]
    [MemberData(nameof(ValidCases))]
    public void CalculateDiscountAmount_ReturnsExpectedAmount(
        decimal price, decimal discountPercent, decimal expected)
    {
        var actual = InvokeDiscount(price, discountPercent);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(InvalidCases))]
    public void CalculateDiscountAmount_InvalidInput_ThrowsForExpectedParameter(
        decimal price, decimal discountPercent, string expectedParameter)
    {
        // Reflection wraps the exception thrown by the private method.
        var wrapper = Assert.Throws<TargetInvocationException>(() =>
        {
            InvokeDiscount(price, discountPercent);
        });

        var exception =
            Assert.IsType<ArgumentOutOfRangeException>(wrapper.InnerException);

        Assert.Equal(expectedParameter, exception.ParamName);
    }

    private static decimal InvokeDiscount(decimal price, decimal discountPercent)
    {
        var result = DiscountMethod.Invoke(
            new PriceCalculator(),
            new object[] { price, discountPercent });

        return Assert.IsType<decimal>(result);
    }
}


