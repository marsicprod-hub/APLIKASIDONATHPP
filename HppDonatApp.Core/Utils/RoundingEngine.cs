namespace HppDonatApp.Core.Utils;

/// <summary>
/// Utility class for applying various rounding policies to prices.
/// Supports multiple rounding strategies for different use cases.
/// </summary>
public static class RoundingEngine
{
    /// <summary>
    /// Rounding rules configurations with their definitions.
    /// </summary>
    public class RoundingRule
    {
        /// <summary>
        /// Gets or sets the unique rule identifier.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name for the rule.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the rounding increment (e.g., 100, 250).
        /// </summary>
        public decimal RoundTo { get; set; }

        /// <summary>
        /// Gets or sets whether to subtract one after rounding (e.g., 99 pricing).
        /// </summary>
        public bool SubtractOne { get; set; } // For 99 pricing

        /// <summary>
        /// Gets or sets a short description of the rule.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Gets all available rounding rules.
    /// </summary>
    public static readonly List<RoundingRule> AvailableRules = new()
    {
        new RoundingRule
        {
            Id = "round100",
            Name = "Round to 100",
            RoundTo = 100m,
            SubtractOne = false,
            Description = "Standard rounding to nearest 100"
        },
        new RoundingRule
        {
            Id = "round250",
            Name = "Round to 250",
            RoundTo = 250m,
            SubtractOne = false,
            Description = "Round to nearest 250"
        },
        new RoundingRule
        {
            Id = "round500",
            Name = "Round to 500",
            RoundTo = 500m,
            SubtractOne = false,
            Description = "Round to nearest 500"
        },
        new RoundingRule
        {
            Id = "round1k",
            Name = "Round to 1,000",
            RoundTo = 1000m,
            SubtractOne = false,
            Description = "Round to nearest 1,000"
        },
        new RoundingRule
        {
            Id = "price99",
            Name = "Price Ending in 99",
            RoundTo = 100m,
            SubtractOne = true,
            Description = "Psychological pricing: X00 - 1"
        },
        new RoundingRule
        {
            Id = "noround",
            Name = "No Rounding",
            RoundTo = 1m,
            SubtractOne = false,
            Description = "Keep exact calculated price"
        }
    };

    /// <summary>
    /// Applies a rounding rule to a price.
    /// </summary>
    /// <param name="price">The price to round (decimal for precision).</param>
    /// <param name="ruleId">The ID of the rounding rule to apply.</param>
    /// <returns>Rounded price as decimal.</returns>
    public static decimal ApplyRule(decimal price, string ruleId)
    {
        var rule = AvailableRules.FirstOrDefault(r => r.Id == ruleId);
        if (rule == null)
            return price;

        var rounded = rule.RoundTo == 1m
            ? Math.Round(price, 0)
            : Math.Round(price / rule.RoundTo) * rule.RoundTo;

        return rule.SubtractOne ? rounded - 1 : rounded;
    }

    /// <summary>
    /// Rounds price to the nearest specified amount.
    /// </summary>
    /// <param name="price">The price to round (decimal for precision).</param>
    /// <param name="roundTo">Round to this value.</param>
    /// <returns>Rounded price as decimal.</returns>
    public static decimal RoundTo(decimal price, decimal roundTo)
    {
        if (roundTo <= 0)
            return Math.Round(price, 0);

        return Math.Round(price / roundTo) * roundTo;
    }

    /// <summary>
    /// Applies psychological pricing (prices ending in 99).
    /// </summary>
    /// <param name="price">The price to adjust (decimal for precision).</param>
    /// <returns>Price adjusted for psychological effect (ending in 99).</returns>
    public static decimal ApplyPsychologicalPricing(decimal price)
    {
        // Round up to next hundred, then subtract 1
        var rounded = Math.Round(price / 100) * 100;
        return rounded > price ? rounded - 1 : Math.Round(price / 100 + 1) * 100 - 1;
    }

    /// <summary>
    /// Gets a rounding rule by its ID.
    /// </summary>
    /// <param name="ruleId">The rule ID to find.</param>
    /// <returns>The rounding rule, or null if not found.</returns>
    public static RoundingRule? GetRule(string ruleId)
    {
        return AvailableRules.FirstOrDefault(r => r.Id == ruleId);
    }
}
