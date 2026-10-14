using System.Runtime.CompilerServices;

namespace Net10Sample.Tests;

public partial class PriceCalculatorUnsafeAccessorTests
{
    private const string TargetType =
        "Net10Sample.PriceCalculator+HiddenCalculator, Net10Sample";
    
    // private 생성자를 호출해 인스턴스 생성
    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    [return: UnsafeAccessorType(TargetType)]
    private static extern object Create();
    
    // 해당 인스턴스의 private 메서드 호출
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Calculate")]
    private static extern decimal CallCalculate(
        [UnsafeAccessorType(TargetType)] object instance,
        decimal price,
        decimal rate);
    
    [Fact]
    public void NestedCalculate_ReturnsDiscount()
    {
        var sut = Create();

        Assert.Equal(10m, CallCalculate(sut, 100m, 10m));
    }
    
    [Fact]
    public void Calculate_ReturnsDiscount()
    {
        object sut = Create();

        Assert.Equal(10m, CallCalculate(sut, 100m, 10m));
    }
    
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

