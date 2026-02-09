namespace HppDonatApp.Core.Models;

/// <summary>
/// Represents an ingredient used in donuts recipes.
/// </summary>
public class Ingredient
{
    /// <summary>Gets or sets the unique identifier for the ingredient.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the ingredient name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the unit of measurement (kg, liter, piece, etc).</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>Gets or sets the current price per unit (decimal for monetary precision).</summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>Gets or sets the supplier information.</summary>
    public string? Supplier { get; set; }

    /// <summary>Gets or sets the category (oil, flour, sugar, etc).</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>Gets or sets the minimum stock level for alerts.</summary>
    public decimal MinimumStock { get; set; }

    /// <summary>Gets or sets creation timestamp.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets last update timestamp.</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
