namespace HppDonatApp.Data;

using HppDonatApp.Core.Models;

/// <summary>
/// Provides seed data initialization for the application database.
/// Includes sample ingredients and recipes for testing and demonstration.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Initializes the database with seed data if it's empty.
    /// </summary>
    /// <param name="context">The database context.</param>
    public static void Initialize(AppDbContext context)
    {
        // Check if data already exists
        if (context.Ingredients.Any())
            return;

        // Create seed ingredients
        var ingredients = CreateSeedIngredients();
        context.Ingredients.AddRange(ingredients);
        context.SaveChanges();

        // Create seed recipes
        var recipes = CreateSeedRecipes(ingredients);
        context.Recipes.AddRange(recipes);
        context.SaveChanges();

        // Add price history  
        var priceHistories = CreateSeedPriceHistories(ingredients);
        context.PriceHistories.AddRange(priceHistories);
        context.SaveChanges();
    }

    /// <summary>
    /// Creates sample ingredients for donut recipes.
    /// </summary>
    /// <returns>List of seed ingredients.</returns>
    private static List<Ingredient> CreateSeedIngredients()
    {
        return new List<Ingredient>
        {
            new Ingredient
            {
                Id = "flour-1",
                Name = "Tepung Terigu",
                Unit = "kg",
                CurrentPrice = 12500m,
                Category = "Flour",
                Supplier = "PT Segara Raya",
                MinimumStock = 5m,
                CreatedAt = DateTime.UtcNow
            },
            new Ingredient
            {
                Id = "sugar-1",
                Name = "Gula Pasir",
                Unit = "kg",
                CurrentPrice = 14000m,
                Category = "Sugar",
                Supplier = "PT Nusantara Sugar",
                MinimumStock = 2m,
                CreatedAt = DateTime.UtcNow
            },
            new Ingredient
            {
                Id = "oil-1",
                Name = "Minyak Goreng",
                Unit = "liter",
                CurrentPrice = 18000m,
                Category = "Oil",
                Supplier = "PT Subur Makmur",
                MinimumStock = 10m,
                CreatedAt = DateTime.UtcNow
            },
            new Ingredient
            {
                Id = "eggs-1",
                Name = "Telur Ayam",
                Unit = "kg",
                CurrentPrice = 29000m,
                Category = "Protein",
                Supplier = "Peternakan Jaya",
                MinimumStock = 1m,
                CreatedAt = DateTime.UtcNow
            },
            new Ingredient
            {
                Id = "cocoa-1",
                Name = "Cokelat Bubuk",
                Unit = "kg",
                CurrentPrice = 85000m,
                Category = "Flavoring",
                Supplier = "PT Cacao Indonesia",
                MinimumStock = 0.5m,
                CreatedAt = DateTime.UtcNow
            },
            new Ingredient
            {
                Id = "baking-powder-1",
                Name = "Baking Powder",
                Unit = "kg",
                CurrentPrice = 65000m,
                Category = "Leavening",
                Supplier = "Import Chemical",
                MinimumStock = 0.2m,
                CreatedAt = DateTime.UtcNow
            },
            new Ingredient
            {
                Id = "salt-1",
                Name = "Garam",
                Unit = "kg",
                CurrentPrice = 8500m,
                Category = "Seasoning",
                Supplier = "Garam Jaya",
                MinimumStock = 0.5m,
                CreatedAt = DateTime.UtcNow
            },
            new Ingredient
            {
                Id = "milk-powder-1",
                Name = "Susu Bubuk",
                Unit = "kg",
                CurrentPrice = 125000m,
                Category = "Dairy",
                Supplier = "PT Kraft Indo",
                MinimumStock = 0.3m,
                CreatedAt = DateTime.UtcNow
            },
            new Ingredient
            {
                Id = "packaging-1",
                Name = "Kemasan Plastik Donat",
                Unit = "piece",
                CurrentPrice = 500m,
                Category = "Packaging",
                Supplier = "PT Kemasan Maju",
                MinimumStock = 100m,
                CreatedAt = DateTime.UtcNow
            }
        };
    }

    /// <summary>
    /// Creates sample donut recipes.
    /// </summary>
    /// <param name="ingredients">The seed ingredients.</param>
    /// <returns>List of seed recipes.</returns>
    private static List<Recipe> CreateSeedRecipes(List<Ingredient> ingredients)
    {
        return new List<Recipe>
        {
            // Donat Original Recipe
            new Recipe
            {
                Id = "recipe-original",
                Name = "Donat Original",
                Description = "Donat klasik rasa vanila dengan tekstur empuk dan gurih",
                TheoreticalOutput = 100,
                WastePercent = 0.05m,
                Version = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Items = new List<RecipeItem>
                {
                    new("flour-1", 3m, "kg", 12500m),
                    new("sugar-1", 1.5m, "kg", 14000m),
                    new("eggs-1", 0.8m, "kg", 29000m),
                    new("milk-powder-1", 0.3m, "kg", 125000m),
                    new("baking-powder-1", 0.05m, "kg", 65000m),
                    new("salt-1", 0.02m, "kg", 8500m)
                }
            },
            // Donat Cokelat Recipe
            new Recipe
            {
                Id = "recipe-chocolate",
                Name = "Donat Cokelat",
                Description = "Donat dengan rasa cokelat kaya dan daging empuk",
                TheoreticalOutput = 100,
                WastePercent = 0.05m,
                Version = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Items = new List<RecipeItem>
                {
                    new("flour-1", 2.8m, "kg", 12500m),
                    new("sugar-1", 1.8m, "kg", 14000m),
                    new("cocoa-1", 0.4m, "kg", 85000m),
                    new("eggs-1", 0.9m, "kg", 29000m),
                    new("milk-powder-1", 0.4m, "kg", 125000m),
                    new("baking-powder-1", 0.06m, "kg", 65000m),
                    new("salt-1", 0.02m, "kg", 8500m)
                }
            }
        };
    }

    /// <summary>
    /// Creates sample price history data for trend analysis.
    /// </summary>
    /// <param name="ingredients">The seed ingredients.</param>
    /// <returns>List of seed price history entries.</returns>
    private static List<PriceHistory> CreateSeedPriceHistories(List<Ingredient> ingredients)
    {
        var histories = new List<PriceHistory>();
        var now = DateTime.UtcNow;

        // Create 30 days of price history for key ingredients
        foreach (var ingredient in ingredients.Take(5))
        {
            var basePrice = ingredient.CurrentPrice;
            var trends = new[] { 0.98m, 0.99m, 1.00m, 1.01m, 1.02m };

            for (int i = 30; i >= 0; i--)
            {
                var trend = trends[Random.Shared.Next(trends.Length)];
                var historicalPrice = basePrice * trend;

                histories.Add(new PriceHistory
                {
                    Id = $"{ingredient.Id}-history-{30 - i}",
                    IngredientId = ingredient.Id,
                    Price = historicalPrice,
                    Date = now.AddDays(-i),
                    Notes = i == 0 ? "Current price" : null
                });
            }
        }

        return histories;
    }
}
