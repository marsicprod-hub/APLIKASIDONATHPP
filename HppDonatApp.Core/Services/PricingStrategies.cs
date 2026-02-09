namespace HppDonatApp.Core.Services;

using HppDonatApp.Core.Interfaces;

/// <summary>
/// Pricing strategy based on fixed markup percentage.
/// Formula: Price = UnitCost * (1 + Markup)
/// </summary>
public class MarkupBasedStrategy : IPricingStrategy
{
    /// <summary>
    /// Gets the display name for this pricing strategy.
    /// </summary>
    public string Name => "Markup Based";

    /// <summary>
    /// Gets a short description of how this strategy prices items.
    /// </summary>
    public string Description => "Applies a fixed markup percentage to unit cost";

    /// <summary>
    /// Calculates a price using a markup percentage.
    /// </summary>
    /// <param name="unitCost">Base unit cost.</param>
    /// <param name="parameters">Strategy parameters (expects "markup").</param>
    /// <returns>Suggested selling price.</returns>
    public decimal CalculatePrice(decimal unitCost, Dictionary<string, object> parameters)
    {
        var markup = parameters.TryGetValue("markup", out var markupObj)
            ? Convert.ToDecimal(markupObj)
            : 0.5m; // Default 50% markup

        return unitCost * (1m + markup);
    }

    /// <summary>
    /// Calculates the profit margin for a given price and cost.
    /// </summary>
    /// <param name="unitCost">Base unit cost.</param>
    /// <param name="suggestedPrice">Suggested selling price.</param>
    /// <returns>Margin as a fraction of price.</returns>
    public decimal CalculateMargin(decimal unitCost, decimal suggestedPrice)
    {
        if (suggestedPrice <= 0)
            return 0m;

        return (suggestedPrice - unitCost) / suggestedPrice;
    }
}

/// <summary>
/// Pricing strategy targeting a specific profit margin.
/// Formula: Price = UnitCost / (1 - TargetMargin)
/// </summary>
public class TargetMarginStrategy : IPricingStrategy
{
    /// <summary>
    /// Gets the display name for this pricing strategy.
    /// </summary>
    public string Name => "Target Margin";

    /// <summary>
    /// Gets a short description of how this strategy prices items.
    /// </summary>
    public string Description => "Calculates price to achieve a target profit margin";

    /// <summary>
    /// Calculates a price to reach a target margin.
    /// </summary>
    /// <param name="unitCost">Base unit cost.</param>
    /// <param name="parameters">Strategy parameters (expects "targetMargin").</param>
    /// <returns>Suggested selling price.</returns>
    public decimal CalculatePrice(decimal unitCost, Dictionary<string, object> parameters)
    {
        var targetMargin = parameters.TryGetValue("targetMargin", out var marginObj)
            ? Convert.ToDecimal(marginObj)
            : 0.35m; // Default 35% margin target

        // Validate to avoid division problems
        if (targetMargin >= 1m)
            targetMargin = 0.9m; // Cap at 90% to ensure price > cost

        return unitCost > 0 && targetMargin < 1m
            ? unitCost / (1m - targetMargin)
            : unitCost * 1.5m; // Fallback to 50% markup
    }

    /// <summary>
    /// Calculates the profit margin for a given price and cost.
    /// </summary>
    /// <param name="unitCost">Base unit cost.</param>
    /// <param name="suggestedPrice">Suggested selling price.</param>
    /// <returns>Margin as a fraction of price.</returns>
    public decimal CalculateMargin(decimal unitCost, decimal suggestedPrice)
    {
        if (suggestedPrice <= 0)
            return 0m;

        return (suggestedPrice - unitCost) / suggestedPrice;
    }
}

/// <summary>
/// Pricing strategy using competitive-friendly rounding rules.
/// Rounds price to psychologically appealing numbers (9.99, 14.99, etc.)
/// </summary>
public class CompetitiveRoundingStrategy : IPricingStrategy
{
    /// <summary>
    /// Gets the display name for this pricing strategy.
    /// </summary>
    public string Name => "Competitive Rounding";

    /// <summary>
    /// Gets a short description of how this strategy prices items.
    /// </summary>
    public string Description => "Applies psychologically appealing prices with rounding";

    /// <summary>
    /// Calculates a price and applies competitive rounding.
    /// </summary>
    /// <param name="unitCost">Base unit cost.</param>
    /// <param name="parameters">Strategy parameters (expects "markup").</param>
    /// <returns>Suggested selling price.</returns>
    public decimal CalculatePrice(decimal unitCost, Dictionary<string, object> parameters)
    {
        var markup = parameters.TryGetValue("markup", out var markupObj)
            ? Convert.ToDecimal(markupObj)
            : 0.5m;

        var basePrice = unitCost * (1m + markup);

        // Apply competitive rounding to create attractive prices
        // This is a simplified version - could be enhanced
        var roundedBase = Math.Round(basePrice, 0);

        // Add psychological pricing: prices ending in 99, 95, or 90
        if (roundedBase > 0)
        {
            // For prices >= 10k, use numbers ending in 000, 500  
            if (roundedBase >= 10000)
                return Math.Round(roundedBase / 1000) * 1000 - 100;

            // For prices >= 1k, use numbers ending in 000, 500, 900
            if (roundedBase >= 1000)
                return Math.Round(roundedBase / 500) * 500 - 100;

            // For smaller prices, round to end in 99 or 95
            return Math.Round(roundedBase / 100) * 100 - 1;
        }

        return basePrice;
    }

    /// <summary>
    /// Calculates the profit margin for a given price and cost.
    /// </summary>
    /// <param name="unitCost">Base unit cost.</param>
    /// <param name="suggestedPrice">Suggested selling price.</param>
    /// <returns>Margin as a fraction of price.</returns>
    public decimal CalculateMargin(decimal unitCost, decimal suggestedPrice)
    {
        if (suggestedPrice <= 0)
            return 0m;

        return (suggestedPrice - unitCost) / suggestedPrice;
    }
}
