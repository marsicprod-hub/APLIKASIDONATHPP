namespace HppDonatApp.Core.Interfaces;

using HppDonatApp.Core.Models;

/// <summary>
/// Core interface for the HPP donat pricing engine.
/// Implements complex formulas for batch cost calculation with multiple cost factors.
/// All monetary calculations use decimal for precision.
/// </summary>
public interface IPricingEngine
{
    /// <summary>
    /// Calculates comprehensive batch pricing including all cost components.
    /// </summary>
    /// <param name="request">The batch request with all parameters.</param>
    /// <returns>Detailed batch result with cost breakdown.</returns>
    BatchResult CalculateBatchCost(BatchRequest request);

    /// <summary>
    /// Calculates the unit cost (cost per donut).
    /// </summary>
    /// <param name="request">The batch request.</param>
    /// <returns>Unit cost as decimal.</returns>
    decimal CalculateUnitCost(BatchRequest request);

    /// <summary>
    /// Calculates the suggested selling price using the specified strategy.
    /// </summary>
    /// <param name="unitCost">The unit cost (decimal for precision).</param>
    /// <param name="strategy">The pricing strategy to use.</param>
    /// <param name="strategyParams">Parameters for the strategy.</param>
    /// <returns>Suggested selling price as decimal.</returns>
    decimal CalculateSuggestedPrice(decimal unitCost, IPricingStrategy strategy, Dictionary<string, object> strategyParams);

    /// <summary>
    /// Gets all available pricing strategies.
    /// </summary>
    /// <returns>Collection of available pricing strategies.</returns>
    IEnumerable<IPricingStrategy> GetAvailableStrategies();

    /// <summary>
    /// Applies rounding rules to a price.
    /// </summary>
    /// <param name="price">The price to round (decimal for precision).</param>
    /// <param name="roundingRule">The rounding rule key.</param>
    /// <returns>Rounded price as decimal.</returns>
    decimal ApplyRoundingRule(decimal price, string roundingRule);
}
