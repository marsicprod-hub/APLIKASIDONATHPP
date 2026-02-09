namespace HppDonatApp.Core.Models;

/// <summary>
/// Represents a price history entry for an ingredient (time-series data).
/// </summary>
public class PriceHistory
{
    /// <summary>Gets or sets the unique identifier.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the ingredient ID this history belongs to.</summary>
    public string IngredientId { get; set; } = string.Empty;

    /// <summary>Gets or sets the ingredient reference.</summary>
    public Ingredient? Ingredient { get; set; }

    /// <summary>Gets or sets the price at this point in time (decimal for monetary precision).</summary>
    public decimal Price { get; set; }

    /// <summary>Gets or sets the date of this price entry.</summary>
    public DateTime Date { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets optional notes about price changes (e.g., "seasonal increase").</summary>
    public string? Notes { get; set; }
}
