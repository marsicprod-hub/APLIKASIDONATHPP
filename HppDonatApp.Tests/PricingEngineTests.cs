namespace HppDonatApp.Tests;

using HppDonatApp.Core.Interfaces;
using HppDonatApp.Core.Models;
using HppDonatApp.Core.Services;

/// <summary>
/// Comprehensive unit tests for the PricingEngine.
/// Tests verify all calculations with decimal precision for monetary values.
/// </summary>
public class PricingEngineTests
{
    private readonly IPricingEngine _pricingEngine;

    public PricingEngineTests()
    {
        _pricingEngine = new PricingEngine();
    }

    /// <summary>
    /// Test 1: Basic ingredient cost calculation.
    /// Verifies that ingredient costs are calculated correctly: Sum(quantity * price) * batchMultiplier
    /// </summary>
    [Fact]
    public void CalculateBatchCost_WithBasicIngredients_ShouldCalculateCorrectly()
    {
        // Arrange
        var items = new List<RecipeItem>
        {
            new("flour", 3m, "kg", 12500m),  // 3 * 12500 = 37500
            new("sugar", 1.5m, "kg", 14000m) // 1.5 * 14000 = 21000
        };
        // Total ingredient cost: 37500 + 21000 = 58500

        var request = new BatchRequest
        {
            Items = items,
            BatchMultiplier = 1m,
            TheoreticalOutput = 100,
            WastePercent = 0.05m,
            Markup = 0.5m,
            OilUsedLiters = 0m,
            EnergyKwh = 0m,
            OverheadAllocated = 0m,
            PackagingPerUnit = 0m,
            VatPercent = 0.1m
        };

        // Act
        var result = _pricingEngine.CalculateBatchCost(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(58500m, result.IngredientCost);
        Assert.True(result.IsValid, string.Join(",", result.Errors));
    }

    /// <summary>
    /// Test 2: Complex batch with all cost components.
    /// Verifies all cost factors are included: ingredients, oil, energy, labor, overhead, packaging.
    /// </summary>
    [Fact]
    public void CalculateBatchCost_WithAllCostComponents_ShouldIncludeAllFactors()
    {
        // Arrange
        var items = new List<RecipeItem>
        {
            new("flour", 3m, "kg", 12500m),
            new("eggs", 0.8m, "kg", 29000m)
        };

        var request = new BatchRequest
        {
            Items = items,
            BatchMultiplier = 2m, // 2x batch
            TheoreticalOutput = 100,
            WastePercent = 0.05m,
            OilUsedLiters = 4m,
            OilPricePerLiter = 18000m, // 4 * 18000 = 72000
            OilChangeCost = 100000m,
            BatchesPerOilChange = 50, // 100000 / 50 = 2000
            EnergyKwh = 10m,
            EnergyRatePerKwh = 3000m, // 10 * 3000 = 30000
            Labor = new[] { ("Baker", 8m, 25000m) }, // 8 * 25000 = 200000
            OverheadAllocated = 50000m,
            PackagingPerUnit = 500m,
            Markup = 0.5m,
            VatPercent = 0.1m
        };

        // Act
        var result = _pricingEngine.CalculateBatchCost(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsValid);
        
        // Verify key calculations
        decimal ingredientCost = (3m * 12500m * 2m) + (0.8m * 29000m * 2m);
        Assert.Equal(ingredientCost, result.IngredientCost);
        
        Assert.Equal(72000m + 2000m, result.OilAmortizedCost); // Oil + amortization
        Assert.Equal(30000m, result.EnergyCost);
        Assert.Equal(200000m, result.LaborCost);
        Assert.Equal(50000m, result.OverheadCost);
        
        // Sellable units: floor(100 * (1 - 0.05)) = 95
        Assert.Equal(95m, result.SellableUnits);
        
        // Packaging cost: 500 * 95 = 47500
        Assert.Equal(47500m, result.PackagingCost);

        // Total batch cost verification
        decimal expectedTotalBatchCost = ingredientCost + 74000m + 30000m + 200000m + 50000m + 47500m;
        Assert.Equal(expectedTotalBatchCost, result.TotalBatchCost);
    }

    /// <summary>
    /// Test 3: Unit cost and pricing calculation.
    /// Verifies: UnitCost = TotalBatchCost / SellableUnits
    /// and SuggestedPrice = RoundRule(UnitCost * (1 + Markup))
    /// </summary>
    [Fact]
    public void CalculateUnitCost_WithValidRequest_ShouldReturnCorrectCost()
    {
        // Arrange
        var items = new List<RecipeItem>
        {
            new("flour", 5m, "kg", 10000m) // 50000
        };

        var request = new BatchRequest
        {
            Items = items,
            BatchMultiplier = 1m,
            TheoreticalOutput = 100,
            WastePercent = 0.10m, // 10% waste, so sellable = 90
            Markup = 0.5m,
            OilUsedLiters = 1m,
            OilPricePerLiter = 15000m,
            EnergyKwh = 0m,
            OverheadAllocated = 0m,
            PackagingPerUnit = 0m,
            VatPercent = 0.1m
        };

        // Act
        var unitCost = _pricingEngine.CalculateUnitCost(request);

        // Assert
        // Total cost = 50000 (ingredients) + 15000 (oil) = 65000
        // Sellable units = 90
        // Unit cost = 65000 / 90 = 722.22...
        decimal expectedUnitCost = 65000m / 90m;
        Assert.Equal(expectedUnitCost, unitCost, 2); // Tolerance of 2 decimals for rounding
    }

    /// <summary>
    /// Test 4: Margin calculation verification.
    /// Verifies: Margin = (SuggestedPrice - UnitCost) / SuggestedPrice
    /// </summary>
    [Fact]
    public void CalculateBatchCost_WithMarkup_ShouldCalculateMarginCorrectly()
    {
        // Arrange
        var items = new List<RecipeItem>
        {
            new("ingredient", 10m, "kg", 10000m)  // 100000
        };

        var request = new BatchRequest
        {
            Items = items,
            BatchMultiplier = 1m,
            TheoreticalOutput = 100,
            WastePercent = 0m, // No waste for easier calculation
            Markup = 1m, // 100% markup
            OilUsedLiters = 0m,
            EnergyKwh = 0m,
            OverheadAllocated = 0m,
            PackagingPerUnit = 0m,
            VatPercent = 0m
        };

        // Act
        var result = _pricingEngine.CalculateBatchCost(request);

        // Assert
        Assert.NotNull(result);
        
        // Unit cost = 100000 / 100 = 1000
        Assert.Equal(1000m, result.UnitCost);
        
        // Suggested price = 1000 * (1 + 1) = 2000
        Assert.Equal(2000m, result.SuggestedPrice);
        
        // Margin = (2000 - 1000) / 2000 = 0.5 (50%)
        Assert.Equal(0.5m, result.MarginPercent);
    }

    /// <summary>
    /// Test 5: Rounding rule application.
    /// Verifies that rounding rules correctly transform prices.
    /// </summary>
    [Theory]
    [InlineData(1234m, "round100", 1200m)]
    [InlineData(1567m, "round500", 1500m)]
    [InlineData(1234m, "round1k", 1000m)]
    [InlineData(999m, "round100", 1000m)]
    [InlineData(1500m, "noround", 1500m)]
    public void ApplyRoundingRule_WithVariousRules_ShouldRoundCorrectly(
        decimal price, string rule, decimal expected)
    {
        // Act
        var result = _pricingEngine.ApplyRoundingRule(price, rule);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Test 6: Batch multiplier scaling.
    /// Verifies that batch multiplier correctly scales ingredient costs.
    /// </summary>
    [Fact]
    public void CalculateBatchCost_WithBatchMultiplier_ShouldScaleIngredientsCorrectly()
    {
        // Arrange - 2x batch
        var items = new List<RecipeItem>
        {
            new("ingredient", 10m, "kg", 10000m)
        };

        var baseRequest = new BatchRequest
        {
            Items = items,
            BatchMultiplier = 1m,
            TheoreticalOutput = 100,
            WastePercent = 0m,
            Markup = 0m,
            OilUsedLiters = 0m,
            EnergyKwh = 0m,
            OverheadAllocated = 0m,
            PackagingPerUnit = 0m
        };

        var doubleRequest = new BatchRequest
        {
            Items = items,
            BatchMultiplier = 2m,
            TheoreticalOutput = 100,
            WastePercent = 0m,
            Markup = 0m,
            OilUsedLiters = 0m,
            EnergyKwh = 0m,
            OverheadAllocated = 0m,
            PackagingPerUnit = 0m
        };

        // Act
        var baseResult = _pricingEngine.CalculateBatchCost(baseRequest);
        var doubleResult = _pricingEngine.CalculateBatchCost(doubleRequest);

        // Assert
        Assert.Equal(100000m, baseResult.IngredientCost);  // 10 * 10000
        Assert.Equal(200000m, doubleResult.IngredientCost); // 10 * 10000 * 2
    }

    /// <summary>
    /// Test 7: Waste calculation verification.
    /// Verifies sellable units = floor(TheoreticalOutput * (1 - WastePercent))
    /// </summary>
    [Theory]
    [InlineData(100, 0.05, 95)]       // 5% waste
    [InlineData(100, 0.10, 90)]       // 10% waste
    [InlineData(100, 0.00, 100)]      // No waste
    [InlineData(1000, 0.075, 925)]    // 7.5% waste
    public void CalculateBatchCost_WithWastPercent_ShouldCalculateSellableUnits(
        int theoretical, decimal waste, decimal expectedSellable)
    {
        // Arrange
        var request = new BatchRequest
        {
            Items = new List<RecipeItem>(),
            BatchMultiplier = 1m,
            TheoreticalOutput = theoretical,
            WastePercent = waste,
            Markup = 0m,
            OilUsedLiters = 0m,
            EnergyKwh = 0m,
            OverheadAllocated = 0m,
            PackagingPerUnit = 0m
        };

        // Act
        var result = _pricingEngine.CalculateBatchCost(request);

        // Assert
        Assert.Equal(expectedSellable, result.SellableUnits);
    }

    /// <summary>
    /// Test 8: Available strategies retrieval.
    /// Verifies that pricing engine provides access to available strategies.
    /// </summary>
    [Fact]
    public void GetAvailableStrategies_ShouldReturnMultipleStrategies()
    {
        // Act
        var strategies = _pricingEngine.GetAvailableStrategies().ToList();

        // Assert
        Assert.NotEmpty(strategies);
        Assert.True(strategies.Count >= 3, "Should have at least 3 strategies");
        Assert.Contains(strategies, s => s.Name.Contains("Markup"));
        Assert.Contains(strategies, s => s.Name.Contains("Margin"));
        Assert.Contains(strategies, s => s.Name.Contains("Competitive"));
    }

    /// <summary>
    /// Test 9: VAT calculation.
    /// Verifies: PriceIncVat = SuggestedPrice * (1 + VatPercent)
    /// </summary>
    [Fact]
    public void CalculateBatchCost_WithVAT_ShouldCalculateCorrectly()
    {
        // Arrange
        var items = new List<RecipeItem>
        {
            new("ingredient", 10m, "kg", 10000m)
        };

        var request = new BatchRequest
        {
            Items = items,
            BatchMultiplier = 1m,
            TheoreticalOutput = 100,
            WastePercent = 0m,
            Markup = 0m,
            OilUsedLiters = 0m,
            EnergyKwh = 0m,
            OverheadAllocated = 0m,
            PackagingPerUnit = 0m,
            VatPercent = 0.15m // 15% VAT
        };

        // Act
        var result = _pricingEngine.CalculateBatchCost(request);

        // Assert
        // Suggested price = 1000 (with 0 markup, suggested = unit cost)
        // Price with VAT = 1000 * 1.15 = 1150
        Assert.Equal(1150m, result.PriceIncludingVat);
    }

    /// <summary>
    /// Test 10: Error handling for invalid input.
    /// Verifies that invalid batch requests produce error messages.
    /// </summary>
    [Fact]
    public void CalculateBatchCost_WithInvalidInput_ShouldHandleGracefully()
    {
        // Arrange
        var invalidRequest = new BatchRequest
        {
            Items = new List<RecipeItem>(),
            TheoreticalOutput = 0, // Invalid: must be > 0
            WastePercent = 0.05m,
            Markup = 0.5m
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _pricingEngine.CalculateBatchCost(invalidRequest));
        
        Assert.Contains("Theoretical output", exception.Message);
    }
}
