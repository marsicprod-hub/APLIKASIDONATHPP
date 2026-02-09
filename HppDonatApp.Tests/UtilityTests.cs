namespace HppDonatApp.Tests;

using HppDonatApp.Core.Models;
using HppDonatApp.Core.Utils;

/// <summary>
/// Tests for utility classes: UnitConverter and RoundingEngine.
/// </summary>
public class UtilityTests
{
    /// <summary>
    /// Test: Unit converter should convert between units correctly.
    /// </summary>
    [Theory]
    [InlineData(1.0, "kg", "gram", 1000.0)]
    [InlineData(1000.0, "gram", "kg", 1.0)]
    [InlineData(1.0, "liter", "ml", 1000.0)]
    [InlineData(1.0, "kg", "kg", 1.0)]
    public void UnitConverter_ShouldConvertCorrectly(
        double quantity, string from, string to, double expected)
    {
        // Act
        var result = UnitConverter.Convert((decimal)quantity, from, to);

        // Assert
        Assert.Equal((decimal)expected, result, 2);
    }

    /// <summary>
    /// Test: Rounding engine should apply rules correctly.
    /// </summary>
    [Theory]
    [InlineData(1234.0, "round100", 1200.0)]
    [InlineData(1568.0, "round500", 1500.0)]
    [InlineData(4567.0, "round1k", 5000.0)]
    public void RoundingEngine_ShouldApplyRulesCorrectly(
        double price, string rule, double expected)
    {
        // Act
        var result = RoundingEngine.ApplyRule((decimal)price, rule);

        // Assert
        Assert.Equal((decimal)expected, result);
    }

    /// <summary>
    /// Test: Recipe item should calculate cost correctly.
    /// </summary>
    [Fact]
    public void RecipeItem_CalculateCost_ShouldComputeCorrectly()
    {
        // Arrange
        var item = new RecipeItem("ingredient1", 5m, "kg", 10000m);

        // Act
        var cost = item.CalculateCost(1m);

        // Assert
        Assert.Equal(50000m, cost);
    }

    /// <summary>
    /// Test: Recipe item cost with batch multiplier.
    /// </summary>
    [Fact]
    public void RecipeItem_CalculateCost_WithMultiplier_ShouldScale()
    {
        // Arrange
        var item = new RecipeItem("ingredient1", 5m, "kg", 10000m);

        // Act
        var cost = item.CalculateCost(2m);

        // Assert
        Assert.Equal(100000m, cost); // 5 * 10000 * 2
    }

    /// <summary>
    /// Test: Recipe sellable units calculation.
    /// </summary>
    [Fact]
    public void Recipe_CalculateSellableUnits_ShouldAccountForWaste()
    {
        // Arrange
        var recipe = new Recipe
        {
            Id = "test",
            Name = "Test Recipe",
            TheoreticalOutput = 100,
            WastePercent = 0.05m
        };

        // Act
        var sellable = recipe.CalculateSellableUnits();

        // Assert
        Assert.Equal(95m, sellable); // floor(100 * 0.95)
    }

    /// <summary>
    /// Test: Unit converter should handle unknown units gracefully.
    /// </summary>
    [Fact]
    public void UnitConverter_WithUnknownUnit_ShouldReturnOriginal()
    {
        // Act
        var result = UnitConverter.Convert(10m, "unknown", "kg");

        // Assert
        Assert.Equal(10m, result); // Returns original if conversion fails
    }

    /// <summary>
    /// Test: Rounding engine should provide available rules.
    /// </summary>
    [Fact]
    public void RoundingEngine_GetRule_ShouldFindValidRules()
    {
        // Act
        var rule = RoundingEngine.GetRule("round100");

        // Assert
        Assert.NotNull(rule);
        Assert.Equal("Round to 100", rule.Name);
    }

    /// <summary>
    /// Test: Pricing strategy unit cost.
    /// </summary>
    [Fact]
    public void RecipeItem_WithZeroQuantity_ShouldCalculateZeroCost()
    {
        // Arrange
        var item = new RecipeItem("ingredient", 0m, "kg", 10000m);

        // Act
        var cost = item.CalculateCost(1m);

        // Assert
        Assert.Equal(0m, cost);
    }
}
