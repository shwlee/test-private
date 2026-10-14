using System.Runtime.CompilerServices;

namespace Net10Sample.Tests;

public class PriceCalculatorUnsafeAccessorNestedTests
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
        var sut = Create();

        Assert.Equal(10m, CallCalculate(sut, 100m, 10m));
    }
}