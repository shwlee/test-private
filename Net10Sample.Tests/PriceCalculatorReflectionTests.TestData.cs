namespace Net10Sample.Tests;

public partial class PriceCalculatorReflectionTests
{
    public static TheoryData<decimal, decimal, decimal> ValidCases => new()
    {
        { 100m, 10m, 10m },
        { 0m, 50m, 0m },
        { 100m, 0m, 0m },
        { 100m, 100m, 100m },
        { 200m, 12.5m, 25m },
        { 1.24m, 10m, 0.12m },
        { 1.25m, 10m, 0.13m },
        { 1.26m, 10m, 0.13m },
    };

    public static TheoryData<decimal, decimal, string> InvalidCases => new()
    {
        { -1m, 10m, "price" },
        { 100m, -1m, "discountPercent" },
        { 100m, 101m, "discountPercent" },
    };
}

