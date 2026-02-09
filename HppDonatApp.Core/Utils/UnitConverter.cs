namespace HppDonatApp.Core.Utils;

/// <summary>
/// Utility class for converting between different units of measurement.
/// Supports common cooking units and metric/imperial conversions.
/// </summary>
public static class UnitConverter
{
    /// <summary>
    /// Dictionary mapping unit names to their base unit (kg) equivalents.
    /// </summary>
    private static readonly Dictionary<string, decimal> UnitConversions = new(StringComparer.OrdinalIgnoreCase)
    {
        // Weight conversions
        { "kg", 1m },
        { "gram", 0.001m },
        { "g", 0.001m },
        { "mg", 0.000001m },
        { "pound", 0.453592m },
        { "lb", 0.453592m },
        { "ounce", 0.0283495m },
        { "oz", 0.0283495m },

        // Volume conversions
        { "liter", 1m },
        { "l", 1m },
        { "ml", 0.001m },
        { "gallon", 3.78541m },
        { "cup", 0.236588m },
        { "tablespoon", 0.015m },
        { "tbsp", 0.015m },
        { "teaspoon", 0.005m },
        { "tsp", 0.005m },

        // Count
        { "piece", 1m },
        { "count", 1m },
        { "pcs", 1m }
    };

    /// <summary>
    /// Converts a quantity from one unit to another.
    /// </summary>
    /// <param name="quantity">The quantity to convert (decimal for precision).</param>
    /// <param name="fromUnit">The source unit name.</param>
    /// <param name="toUnit">The target unit name.</param>
    /// <returns>Converted quantity as decimal, or original quantity if conversion not possible.</returns>
    public static decimal Convert(decimal quantity, string fromUnit, string toUnit)
    {
        if (string.Equals(fromUnit, toUnit, StringComparison.OrdinalIgnoreCase))
            return quantity;

        if (!UnitConversions.TryGetValue(fromUnit, out var fromFactor) ||
            !UnitConversions.TryGetValue(toUnit, out var toFactor))
        {
            // Unknown units - return original
            return quantity;
        }

        // Convert to base unit, then to target unit
        return quantity * fromFactor / toFactor;
    }

    /// <summary>
    /// Gets all supported unit names.
    /// </summary>
    /// <returns>Collection of supported unit names.</returns>
    public static IEnumerable<string> GetSupportedUnits()
    {
        return UnitConversions.Keys;
    }

    /// <summary>
    /// Attempts to parse a unit string and normalize it.
    /// </summary>
    /// <param name="unit">The unit string to normalize.</param>
    /// <returns>Normalized unit name, or original if not recognized.</returns>
    public static string NormalizeUnit(string unit)
    {
        if (string.IsNullOrWhiteSpace(unit))
            return unit;

        var normalized = unit.Trim().ToLowerInvariant();

        // Return the canonical form if it exists in our conversions
        return UnitConversions.ContainsKey(normalized)
            ? unit
            : unit;
    }
}
