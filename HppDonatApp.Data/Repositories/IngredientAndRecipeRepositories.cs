namespace HppDonatApp.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using HppDonatApp.Core.Models;

/// <summary>
/// Specialized repository for Ingredient entities with additional query methods.
/// </summary>
public interface IIngredientRepository : IRepository<Ingredient>
{
    /// <summary>Gets ingredients by category.</summary>
    /// <param name="category">The category name.</param>
    /// <returns>Collection of ingredients in the category.</returns>
    Task<IEnumerable<Ingredient>> GetByCategoryAsync(string category);

    /// <summary>Gets ingredients by supplier.</summary>
    /// <param name="supplier">The supplier name.</param>
    /// <returns>Collection of ingredients from the supplier.</returns>
    Task<IEnumerable<Ingredient>> GetBySupplierAsync(string supplier);

    /// <summary>Gets the current price of an ingredient.</summary>
    /// <param name="ingredientId">The ingredient ID.</param>
    /// <returns>The current price, or 0 if not found.</returns>
    Task<decimal> GetCurrentPriceAsync(string ingredientId);

    /// <summary>Updates the current price of an ingredient.</summary>
    /// <param name="ingredientId">The ingredient ID.</param>
    /// <param name="newPrice">The new price (decimal for precision).</param>
    Task UpdatePriceAsync(string ingredientId, decimal newPrice);
}

/// <summary>
/// Implementation of IIngredientRepository.
/// </summary>
public class IngredientRepository : Repository<Ingredient>, IIngredientRepository
{
    /// <summary>
    /// Initializes a new instance of the IngredientRepository.
    /// </summary>
    /// <param name="context">The database context.</param>
    public IngredientRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Ingredient>> GetByCategoryAsync(string category)
    {
        return await DbSet
            .Where(i => i.Category == category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ingredient>> GetBySupplierAsync(string supplier)
    {
        return await DbSet
            .Where(i => i.Supplier == supplier)
            .ToListAsync();
    }

    public async Task<decimal> GetCurrentPriceAsync(string ingredientId)
    {
        var ingredient = await DbSet.FirstOrDefaultAsync(i => i.Id == ingredientId);
        return ingredient?.CurrentPrice ?? 0m;
    }

    public async Task UpdatePriceAsync(string ingredientId, decimal newPrice)
    {
        var ingredient = await DbSet.FirstOrDefaultAsync(i => i.Id == ingredientId);
        if (ingredient != null)
        {
            ingredient.CurrentPrice = newPrice;
            ingredient.UpdatedAt = DateTime.UtcNow;
            await SaveChangesAsync();
        }
    }
}

/// <summary>
/// Specialized repository for Recipe entities with additional query methods.
/// </summary>
public interface IRecipeRepository : IRepository<Recipe>
{
    /// <summary>Gets all active recipes.</summary>
    /// <returns>Collection of active recipes.</returns>
    Task<IEnumerable<Recipe>> GetActiveRecipesAsync();

    /// <summary>Gets a recipe with its ingredients loaded.</summary>
    /// <param name="recipeId">The recipe ID.</param>
    /// <returns>The recipe with ingredients, or null if not found.</returns>
    Task<Recipe?> GetWithItemsAsync(string recipeId);
}

/// <summary>
/// Implementation of IRecipeRepository.
/// </summary>
public class RecipeRepository : Repository<Recipe>, IRecipeRepository
{
    /// <summary>
    /// Initializes a new instance of the RecipeRepository.
    /// </summary>
    /// <param name="context">The database context.</param>
    public RecipeRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Recipe>> GetActiveRecipesAsync()
    {
        return await DbSet
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Recipe?> GetWithItemsAsync(string recipeId)
    {
        return await DbSet
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == recipeId);
    }
}
