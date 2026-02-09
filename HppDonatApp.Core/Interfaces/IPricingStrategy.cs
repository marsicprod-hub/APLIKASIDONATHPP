namespace HppDonatApp.Core.Interfaces;

/// <summary>
/// Interface for pluggable pricing strategies.
/// Different pricing strategies can be swapped at runtime.
/// </summary>
public interface IPricingStrategy
{
    /// <summary>Gets the friendly name of this strategy.</summary>
    string Name { get; }

    /// <summary>Gets the description of how this strategy calculates prices.</summary>
    string Description { get; }

    /// <summary>
    /// Calculates the suggested selling price based on unit cost and strategy parameters.
    /// </summary>
    /// <param name="unitCost">The cost per unit (decimal for precision).</param>
    /// <param name="parameters">Strategy-specific parameters.</param>
    /// <returns>Suggested selling price as decimal.</returns>
    decimal CalculatePrice(decimal unitCost, Dictionary<string, object> parameters);

    /// <summary>
    /// Calculates the margin percentage achieved by this pricing strategy.
    /// </summary>
    /// <param name="unitCost">The cost per unit (decimal for precision).</param>
    /// <param name="suggestedPrice">The calculated suggested price.</param>
    /// <returns>Margin percentage (0.35 = 35%).</returns>
    decimal CalculateMargin(decimal unitCost, decimal suggestedPrice);
}
