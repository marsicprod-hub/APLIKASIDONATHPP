namespace HppDonatApp.Core.Services;

using System.Globalization;
using HppDonatApp.Core.Interfaces;
using HppDonatApp.Core.Models;

/// <summary>
/// Comprehensive pricing engine for HPP donat calculations.
/// Implements all formulas with decimal precision for accurate monetary calculations.
/// Supports multiple pricing strategies and configurable rounding rules.
/// </summary>
public class PricingEngine : IPricingEngine
{
    private readonly List<IPricingStrategy> _strategies;

    /// <summary>
    /// Initializes a new instance of the PricingEngine with default pricing strategies.
    /// </summary>
    public PricingEngine()
    {
        _strategies = new List<IPricingStrategy>
        {
            new MarkupBasedStrategy(),
            new TargetMarginStrategy(),
            new CompetitiveRoundingStrategy()
        };
    }

    /// <inheritdoc/>
    public BatchResult CalculateBatchCost(BatchRequest request)
    {
        ValidateBatchRequest(request);

        var result = new BatchResult();

        try
        {
            // Calculate ingredient cost: Sum(item.PricePerUnit * item.Quantity * BatchMultiplier)
            var ingredientCost = CalculateIngredientCost(request);
            result.IngredientCost = ingredientCost;

            // Store individual ingredient costs for visualization
            foreach (var item in request.Items)
            {
                var itemCost = item.CalculateCost(request.BatchMultiplier);
                result.IngredientCosts[item.IngredientId] = itemCost;
            }

            // Calculate oil cost with amortization: OilUsedLiters * OilPricePerLiter
            var oilCost = request.OilUsedLiters * request.OilPricePerLiter;

            // Calculate oil amortization: OilChangeCost / BatchesPerOilChange
            var oilAmortization = request.BatchesPerOilChange > 0
                ? request.OilChangeCost / request.BatchesPerOilChange
                : 0m;
            result.OilAmortizedCost = oilCost + oilAmortization;

            // Calculate energy cost: EnergyKwh * EnergyRatePerKwh
            var energyCost = request.EnergyKwh * request.EnergyRatePerKwh;
            result.EnergyCost = energyCost;

            // Calculate labor cost: Sum(Hours * Rate)
            var laborCost = request.Labor.Aggregate(0m, (sum, labor) => sum + (labor.Hours * labor.Rate));
            result.LaborCost = laborCost;

            // Overhead allocation
            result.OverheadCost = request.OverheadAllocated;

            // Calculate sellable units: floor(TheoreticalOutput * (1 - WastePercent))
            var sellableUnits = (decimal)Math.Floor(request.TheoreticalOutput * (1m - request.WastePercent));
            result.SellableUnits = sellableUnits;

            // Calculate packaging cost: PackagingPerUnit * SellableUnits
            var packagingCost = request.PackagingPerUnit * sellableUnits;
            result.PackagingCost = packagingCost;

            // Calculate total batch cost: sum of all costs
            var totalBatchCost = ingredientCost + result.OilAmortizedCost + energyCost + laborCost + result.OverheadCost + packagingCost;
            result.TotalBatchCost = totalBatchCost;

            // Validate sellable units before division
            if (sellableUnits <= 0)
            {
                result.Errors.Add("Sellable units must be greater than 0 for cost calculations");
                return result;
            }

            // Calculate unit cost: TotalBatchCost / SellableUnits
            var unitCost = totalBatchCost / sellableUnits;
            result.UnitCost = unitCost;

            // Build cost breakdown for visualization
            result.CostBreakdown = new Dictionary<string, decimal>
            {
                { "Ingredients", ingredientCost },
                { "Oil & Amortization", result.OilAmortizedCost },
                { "Energy", energyCost },
                { "Labor", laborCost },
                { "Overhead", result.OverheadCost },
                { "Packaging", packagingCost }
            };

            // Calculate suggested price using selected markup: RoundRule(UnitCost * (1 + Markup))
            var basePrice = unitCost * (1m + request.Markup);
            result.SuggestedPrice = ApplyRoundingRule(basePrice, request.RoundingRule);

            // Calculate margin: (SuggestedPrice - UnitCost) / SuggestedPrice
            if (result.SuggestedPrice > 0)
            {
                result.MarginPercent = (result.SuggestedPrice - unitCost) / result.SuggestedPrice;
            }

            // Calculate price including VAT: SuggestedPrice * (1 + VatPercent)
            result.PriceIncludingVat = result.SuggestedPrice * (1m + request.VatPercent);
            result.PricingStrategy = "Fixed Markup";
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Error calculating batch cost: {ex.Message}");
        }

        return result;
    }

    /// <inheritdoc/>
    public decimal CalculateUnitCost(BatchRequest request)
    {
        var ingredientCost = CalculateIngredientCost(request);
        var oilCost = request.OilUsedLiters * request.OilPricePerLiter;
        var oilAmortization = request.BatchesPerOilChange > 0
            ? request.OilChangeCost / request.BatchesPerOilChange
            : 0m;
        var energyCost = request.EnergyKwh * request.EnergyRatePerKwh;
        var laborCost = request.Labor.Aggregate(0m, (sum, labor) => sum + (labor.Hours * labor.Rate));
        var sellableUnits = (decimal)Math.Floor(request.TheoreticalOutput * (1m - request.WastePercent));
        var packagingCost = request.PackagingPerUnit * sellableUnits;

        var totalBatchCost = ingredientCost + oilCost + oilAmortization + energyCost + laborCost + request.OverheadAllocated + packagingCost;

        return sellableUnits > 0 ? totalBatchCost / sellableUnits : 0m;
    }

    /// <inheritdoc/>
    public decimal CalculateSuggestedPrice(decimal unitCost, IPricingStrategy strategy, Dictionary<string, object> strategyParams)
    {
        ArgumentNullException.ThrowIfNull(strategy);

        return strategy.CalculatePrice(unitCost, strategyParams ?? []);
    }

    /// <inheritdoc/>
    public IEnumerable<IPricingStrategy> GetAvailableStrategies()
    {
        return _strategies.AsReadOnly();
    }

    /// <inheritdoc/>
    public decimal ApplyRoundingRule(decimal price, string roundingRule)
    {
        return roundingRule switch
        {
            "round100" => Math.Round(price / 100) * 100,
            "round250" => Math.Round(price / 250) * 250,
            "round500" => Math.Round(price / 500) * 500,
            "round1k" => Math.Round(price / 1000) * 1000,
            "noround" => price,
            _ => Math.Round(price / 100) * 100 // Default to round100
        };
    }

    /// <summary>
    /// Validates batch request for required values and constraints.
    /// </summary>
    /// <param name="request">The batch request to validate.</param>
    private static void ValidateBatchRequest(BatchRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.TheoreticalOutput <= 0)
            throw new ArgumentException("Theoretical output must be greater than 0", nameof(request.TheoreticalOutput));

        if (request.WastePercent < 0 || request.WastePercent >= 1)
            throw new ArgumentException("Waste percent must be between 0 and 100%", nameof(request.WastePercent));

        if (request.BatchMultiplier <= 0)
            throw new ArgumentException("Batch multiplier must be greater than 0", nameof(request.BatchMultiplier));

        if (request.VatPercent < 0)
            throw new ArgumentException("VAT percent cannot be negative", nameof(request.VatPercent));
    }

    /// <summary>
    /// Calculates total ingredient cost.
    /// </summary>
    /// <param name="request">The batch request.</param>
    /// <returns>Total ingredient cost as decimal.</returns>
    private static decimal CalculateIngredientCost(BatchRequest request)
    {
        return request.Items.Aggregate(0m, (sum, item) => sum + item.CalculateCost(request.BatchMultiplier));
    }
}
