namespace HppDonatApp.Core.Models;

/// <summary>
/// Represents a donut recipe with its ingredients and metadata.
/// </summary>
public class Recipe
{
    /// <summary>Gets or sets the unique identifier.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the recipe name (e.g., "Donat Original").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets detailed description of the recipe.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the collection of recipe items (ingredients with quantities).</summary>
    public ICollection<RecipeItem> Items { get; set; } = new List<RecipeItem>();

    /// <summary>Gets or sets the theoretical output count per batch (before waste).</summary>
    public int TheoreticalOutput { get; set; }

    /// <summary>Gets or sets the waste percentage (0.05 for 5%).</summary>
    public decimal WastePercent { get; set; } = 0.05m;

    /// <summary>Gets or sets the version number for tracking recipe changes.</summary>
    public int Version { get; set; } = 1;

    /// <summary>Gets or sets whether this recipe is currently active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets creation timestamp.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets last update timestamp.</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Calculates the sellable units after accounting for waste.</summary>
    /// <returns>Number of sellable units as decimal.</returns>
    public decimal CalculateSellableUnits()
    {
        return (decimal)Math.Floor(TheoreticalOutput * (1m - WastePercent));
    }
}
