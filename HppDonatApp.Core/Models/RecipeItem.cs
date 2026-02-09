namespace HppDonatApp.Core.Models;

/// <summary>
/// Represents an individual ingredient item in a recipe with quantity and unit.
/// </summary>
public record RecipeItem(
    string IngredientId,
    decimal Quantity,
    string Unit,
    decimal PricePerUnit)
{
    /// <summary>Gets or sets the optional ingredient reference (populated from data layer).</summary>
    public Ingredient? Ingredient { get; set; }

    /// <summary>Calculates the total cost for this recipe item.</summary>
    /// <param name="batchMultiplier">Multiplier for batch size calculations.</param>
    /// <returns>Total cost as decimal.</returns>
    public decimal CalculateCost(decimal batchMultiplier = 1m)
    {
        return PricePerUnit * Quantity * batchMultiplier;
    }
}
