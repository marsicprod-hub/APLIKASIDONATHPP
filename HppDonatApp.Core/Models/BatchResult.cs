namespace HppDonatApp.Core.Models;

/// <summary>
/// Comprehensive result from HPP donat pricing calculations.
/// All monetary values use decimal for precision.
/// </summary>
public class BatchResult
{
    /// <summary>Gets or sets total ingredient cost.</summary>
    public decimal IngredientCost { get; set; }

    /// <summary>Gets or sets the oil amortized cost.</summary>
    public decimal OilAmortizedCost { get; set; }

    /// <summary>Gets or sets the energy cost.</summary>
    public decimal EnergyCost { get; set; }

    /// <summary>Gets or sets the total labor cost.</summary>
    public decimal LaborCost { get; set; }

    /// <summary>Gets or sets allocated overhead.</summary>
    public decimal OverheadCost { get; set; }

    /// <summary>Gets or sets the packaging cost.</summary>
    public decimal PackagingCost { get; set; }

    /// <summary>Gets or sets the number of sellable units after waste calc.</summary>
    public decimal SellableUnits { get; set; }

    /// <summary>Gets or sets total batch cost (all costs combined).</summary>
    public decimal TotalBatchCost { get; set; }

    /// <summary>Gets or sets cost per unit.</summary>
    public decimal UnitCost { get; set; }

    /// <summary>Gets or sets suggested selling price before VAT.</summary>
    public decimal SuggestedPrice { get; set; }

    /// <summary>Gets or sets the unit margin percentage (0.35 = 35% margin).</summary>
    public decimal MarginPercent { get; set; }

    /// <summary>Gets or sets the final price including VAT.</summary>
    public decimal PriceIncludingVat { get; set; }

    /// <summary>Gets or sets cost breakdown by category for visualization.</summary>
    public Dictionary<string, decimal> CostBreakdown { get; set; } = [];

    /// <summary>Gets or sets individual ingredient costs for detailed analysis.</summary>
    public Dictionary<string, decimal> IngredientCosts { get; set; } = [];

    /// <summary>Gets or sets the pricing strategy used for calculation.</summary>
    public string PricingStrategy { get; set; } = "MarkupBased";

    /// <summary>Gets or sets validation errors if any.</summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>Gets whether the result is valid (no errors).</summary>
    public bool IsValid => Errors.Count == 0;
}
