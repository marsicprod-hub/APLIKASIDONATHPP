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
    [InlineData(1m, "kg", "gram", 1000m)]
    [InlineData(1000m, "gram", "kg", 1m)]
    [InlineData(1m, "liter", "ml", 1000m)]
    [InlineData(1m, "kg", "kg", 1m)]  // Same unit
    public void UnitConverter_ShouldConvertCorrectly(
        decimal quantity, string from, string to, decimal expected)
    {
        // Act
        var result = UnitConverter.Convert(quantity, from, to);

        // Assert
        Assert.Equal(expected, result, 2); // 2 decimal places precision
    }

    /// <summary>
    /// Test: Rounding engine should apply rules correctly.
    /// </summary>
    [Theory]
    [InlineData(1234m, "round100", 1200m)]
    [InlineData(1568m, "round500", 1500m)]
    [InlineData(4567m, "round1k", 5000m)]
    public void RoundingEngine_ShouldApplyRulesCorrectly(
        decimal price, string rule, decimal expected)
    {
        // Act
        var result = RoundingEngine.ApplyRule(price, rule);

        // Assert
        Assert.Equal(expected, result);
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
