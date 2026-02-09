namespace HppDonatApp.Core.Models;

/// <summary>
/// Comprehensive batch request containing all parameters for complex HPP donat calculations.
/// Uses decimal throughout for monetary precision.
/// </summary>
public class BatchRequest
{
    /// <summary>Gets or sets the recipe items (ingredients with quantities).</summary>
    public IEnumerable<RecipeItem> Items { get; set; } = [];

    /// <summary>Gets or sets the batch size multiplier (e.g., 2.0 for double batch).</summary>
    public decimal BatchMultiplier { get; set; } = 1m;

    /// <summary>Gets or sets the oil used in liters.</summary>
    public decimal OilUsedLiters { get; set; }

    /// <summary>Gets or sets the oil price per liter (decimal for precision).</summary>
    public decimal OilPricePerLiter { get; set; }

    /// <summary>Gets or sets the cost of oil change (replacement and labor).</summary>
    public decimal OilChangeCost { get; set; }

    /// <summary>Gets or sets how many batches are made before oil needs changing.</summary>
    public int BatchesPerOilChange { get; set; } = 100;

    /// <summary>Gets or sets the energy consumption in kWh.</summary>
    public decimal EnergyKwh { get; set; }

    /// <summary>Gets or sets the energy rate per kWh (decimal for precision).</summary>
    public decimal EnergyRatePerKwh { get; set; }

    /// <summary>Gets or sets labor information: (Role, Hours, Rate per Hour).</summary>
    public IEnumerable<(string Role, decimal Hours, decimal Rate)> Labor { get; set; } = [];

    /// <summary>Gets or sets allocated overhead cost for this batch.</summary>
    public decimal OverheadAllocated { get; set; }

    /// <summary>Gets or sets the theoretical maximum output (donuts) per batch before waste.</summary>
    public int TheoreticalOutput { get; set; } = 100;

    /// <summary>Gets or sets waste percentage (0.05 = 5%).</summary>
    public decimal WastePercent { get; set; } = 0.05m;

    /// <summary>Gets or sets packaging cost per unit (decimal for precision).</summary>
    public decimal PackagingPerUnit { get; set; }

    /// <summary>Gets or sets the markup multiplier (0.5 = 50% markup).</summary>
    public decimal Markup { get; set; } = 0.5m;

    /// <summary>Gets or sets VAT percentage (0.10 = 10%).</summary>
    public decimal VatPercent { get; set; } = 0.1m;

    /// <summary>Gets or sets optional rounding rule key (e.g., "round100", "round500", "round1k").</summary>
    public string RoundingRule { get; set; } = "round100";

    /// <summary>Gets or sets the recipe ID this batch is based on.</summary>
    public string? RecipeId { get; set; }

    /// <summary>Gets or sets the calculation date for historical/forecasting purposes.</summary>
    public DateTime CalculationDate { get; set; } = DateTime.UtcNow;
}
