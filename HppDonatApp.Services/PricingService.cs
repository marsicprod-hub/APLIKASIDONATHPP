namespace HppDonatApp.Services;

using HppDonatApp.Core.Interfaces;
using HppDonatApp.Core.Models;
using HppDonatApp.Data.Repositories;

/// <summary>
/// Service for managing pricing calculations and scenario analysis.
/// </summary>
public interface IPricingService
{
    /// <summary>Calculates the cost for a batch based on a recipe.</summary>
    /// <param name="recipeId">The recipe ID.</param>
    /// <param name="parameters">Additional pricing parameters.</param>
    /// <returns>The batch result with cost calculations.</returns>
    Task<BatchResult> CalculateRecipeCostAsync(string recipeId, Dictionary<string, decimal> parameters);

    /// <summary>Calculates cost and compares multiple strategies.</summary>
    /// <param name="request">The batch request.</param>
    /// <returns>Results from all available strategies.</returns>
    Task<List<(string Strategy, decimal Price)>> CompareStrategiesAsync(BatchRequest request);

    /// <summary>Simulates "what-if" scenario with price changes.</summary>
    /// <param name="recipeId">The recipe ID.</param>
    /// <param name="priceChangePercent">Price change as decimal (0.2 = 20% increase).</param>
    /// <returns>The batch result after price adjustment simulation.</returns>
    Task<BatchResult> SimulateScenarioAsync(string recipeId, decimal priceChangePercent);
}

/// <summary>
/// Implementation of IPricingService.
/// </summary>
public class PricingService : IPricingService
{
    private readonly IPricingEngine _pricingEngine;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IIngredientRepository _ingredientRepository;

    /// <summary>
    /// Initializes a new instance of the PricingService.
    /// </summary>
    /// <param name="pricingEngine">The pricing engine.</param>
    /// <param name="recipeRepository">The recipe repository.</param>
    /// <param name="ingredientRepository">The ingredient repository.</param>
    public PricingService(
        IPricingEngine pricingEngine,
        IRecipeRepository recipeRepository,
        IIngredientRepository ingredientRepository)
    {
        _pricingEngine = pricingEngine;
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
    }

    public async Task<BatchResult> CalculateRecipeCostAsync(string recipeId, Dictionary<string, decimal> parameters)
    {
        var recipe = await _recipeRepository.GetWithItemsAsync(recipeId);
        if (recipe == null)
        {
            return new BatchResult
            {
                Errors = new List<string> { "Recipe not found" }
            };
        }

        var request = new BatchRequest
        {
            Items = recipe.Items,
            TheoreticalOutput = recipe.TheoreticalOutput,
            WastePercent = recipe.WastePercent,
            RecipeId = recipeId,
            Markup = parameters.TryGetValue("markup", out var markup) ? markup : 0.5m,
            OilUsedLiters = parameters.TryGetValue("oilUsed", out var oil) ? oil : 2m,
            OilPricePerLiter = parameters.TryGetValue("oilPrice", out var oilPrice) ? oilPrice : 18000m,
            EnergyKwh = parameters.TryGetValue("energy", out var energy) ? energy : 5m,
            EnergyRatePerKwh = parameters.TryGetValue("energyRate", out var energyRate) ? energyRate : 3000m
        };

        return _pricingEngine.CalculateBatchCost(request);
    }

    public async Task<List<(string Strategy, decimal Price)>> CompareStrategiesAsync(BatchRequest request)
    {
        var results = new List<(string, decimal)>();
        var unitCost = _pricingEngine.CalculateUnitCost(request);

        foreach (var strategy in _pricingEngine.GetAvailableStrategies())
        {
            var parameters = new Dictionary<string, object>
            {
                { "markup", request.Markup },
                { "targetMargin", 0.35m }
            };

            var price = _pricingEngine.CalculateSuggestedPrice(unitCost, strategy, parameters);
            results.Add((strategy.Name, price));
        }

        return await Task.FromResult(results);
    }

    public async Task<BatchResult> SimulateScenarioAsync(string recipeId, decimal priceChangePercent)
    {
        var recipe = await _recipeRepository.GetWithItemsAsync(recipeId);
        if (recipe == null)
        {
            return new BatchResult
            {
                Errors = new List<string> { "Recipe not found" }
            };
        }

        // Adjust ingredient prices in the items
        var adjustedItems = recipe.Items.Select(item => new RecipeItem(
            item.IngredientId,
            item.Quantity,
            item.Unit,
            item.PricePerUnit * (1m + priceChangePercent)
        )).ToList();

        var request = new BatchRequest
        {
            Items = adjustedItems,
            TheoreticalOutput = recipe.TheoreticalOutput,
            WastePercent = recipe.WastePercent,
            RecipeId = recipeId,
            Markup = 0.5m,
            OilUsedLiters = 2m,
            OilPricePerLiter = 18000m * (1m + priceChangePercent),
            EnergyKwh = 5m,
            EnergyRatePerKwh = 3000m
        };

        return _pricingEngine.CalculateBatchCost(request);
    }
}
