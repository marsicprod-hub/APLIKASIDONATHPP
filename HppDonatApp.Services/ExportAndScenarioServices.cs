namespace HppDonatApp.Services;

using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using HppDonatApp.Core.Models;
using HppDonatApp.Data.Repositories;

/// <summary>
/// Service for exporting and importing data to/from CSV files.
/// </summary>
public interface IFileExportService
{
    /// <summary>Exports ingredients to CSV format.</summary>
    /// <param name="ingredients">The ingredients to export.</param>
    /// <param name="filePath">The output file path.</param>
    Task ExportIngredientsAsync(IEnumerable<Ingredient> ingredients, string filePath);

    /// <summary>Imports ingredients from a CSV file.</summary>
    /// <param name="filePath">The input file path.</param>
    /// <returns>Collection of imported ingredients.</returns>
    Task<IEnumerable<Ingredient>> ImportIngredientsAsync(string filePath);

    /// <summary>Exports recipe summary to CSV.</summary>
    /// <param name="recipes">The recipes to export.</param>
    /// <param name="filePath">The output file path.</param>
    Task ExportRecipesAsync(IEnumerable<Recipe> recipes, string filePath);
}

/// <summary>
/// Implementation of IFileExportService using CsvHelper.
/// </summary>
public class FileExportService : IFileExportService
{
    public async Task ExportIngredientsAsync(IEnumerable<Ingredient> ingredients, string filePath)
    {
        ArgumentNullException.ThrowIfNull(ingredients);
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        await Task.Run(() =>
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(ingredients);
            }
        });
    }

    public async Task<IEnumerable<Ingredient>> ImportIngredientsAsync(string filePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var ingredients = new List<Ingredient>();

        await Task.Run(() =>
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                ingredients = csv.GetRecords<Ingredient>().ToList();
            }
        });

        return ingredients;
    }

    public async Task ExportRecipesAsync(IEnumerable<Recipe> recipes, string filePath)
    {
        ArgumentNullException.ThrowIfNull(recipes);
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        var recordList = new List<dynamic>();

        foreach (var recipe in recipes)
        {
            recordList.Add(new
            {
                recipe.Id,
                recipe.Name,
                recipe.Description,
                recipe.TheoreticalOutput,
                recipe.WastePercent,
                ItemCount = recipe.Items.Count,
                recipe.Version,
                recipe.IsActive
            });
        }

        await Task.Run(() =>
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(recordList);
            }
        });
    }
}

/// <summary>
/// Service for managing pricing scenarios and "what-if" analysis.
/// </summary>
public interface IScenarioService
{
    /// <summary>Gets all saved scenarios.</summary>
    /// <param name="recipeId">The recipe ID.</param>
    /// <returns>Collection of scenarios for the recipe.</returns>
    Task<IEnumerable<PricingScenario>> GetScenariosAsync(string recipeId);

    /// <summary>Saves a pricing scenario.</summary>
    /// <param name="scenario">The scenario to save.</param>
    Task SaveScenarioAsync(PricingScenario scenario);

    /// <summary>Compares two scenarios.</summary>
    /// <param name="scenario1Id">First scenario ID.</param>
    /// <param name="scenario2Id">Second scenario ID.</param>
    /// <returns>Comparison result.</returns>
    Task<ScenarioComparison?> CompareScenarios(string scenario1Id, string scenario2Id);
}

/// <summary>
/// Represents a pricing scenario for what-if analysis.
/// </summary>
public class PricingScenario
{
    /// <summary>Gets or sets the unique identifier.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the scenario name (e.g., "Oil Price Up 20%").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the recipe ID this scenario applies to.</summary>
    public string RecipeId { get; set; } = string.Empty;

    /// <summary>Gets or sets the description of what parameters changed.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the parameters adjusted in this scenario.</summary>
    public Dictionary<string, decimal> AdjustedParameters { get; set; } = [];

    /// <summary>Gets or sets the pricing result for this scenario.</summary>
    public BatchResult? Result { get; set; }

    /// <summary>Gets or sets creation timestamp.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Comparison result between two scenarios.
/// </summary>
public class ScenarioComparison
{
    public string Scenario1Name { get; set; } = string.Empty;
    public string Scenario2Name { get; set; } = string.Empty;
    public decimal Price1 { get; set; }
    public decimal Price2 { get; set; }
    public decimal PriceDifference { get; set; }
    public decimal PercentDifference { get; set; }
}

/// <summary>
/// In-memory implementation of IScenarioService for scenario management.
/// </summary>
public class ScenarioService : IScenarioService
{
    private readonly Dictionary<string, PricingScenario> _scenarios = [];

    public Task<IEnumerable<PricingScenario>> GetScenariosAsync(string recipeId)
    {
        var relevant = _scenarios.Values
            .Where(s => s.RecipeId == recipeId)
            .OrderByDescending(s => s.CreatedAt);

        return Task.FromResult(relevant.AsEnumerable());
    }

    public Task SaveScenarioAsync(PricingScenario scenario)
    {
        ArgumentNullException.ThrowIfNull(scenario);

        if (string.IsNullOrEmpty(scenario.Id))
            scenario.Id = Guid.NewGuid().ToString();

        _scenarios[scenario.Id] = scenario;
        return Task.CompletedTask;
    }

    public Task<ScenarioComparison?> CompareScenarios(string scenario1Id, string scenario2Id)
    {
        if (!_scenarios.TryGetValue(scenario1Id, out var s1) || !_scenarios.TryGetValue(scenario2Id, out var s2))
            return Task.FromResult<ScenarioComparison?>(null);

        if (s1.Result?.SuggestedPrice is null || s2.Result?.SuggestedPrice is null)
            return Task.FromResult<ScenarioComparison?>(null);

        var price1 = s1.Result.SuggestedPrice;
        var price2 = s2.Result.SuggestedPrice;
        var diff = Math.Abs(price2 - price1);
        var pctDiff = price1 > 0 ? (diff / price1) * 100 : 0;

        var comparison = new ScenarioComparison
        {
            Scenario1Name = s1.Name,
            Scenario2Name = s2.Name,
            Price1 = price1,
            Price2 = price2,
            PriceDifference = diff,
            PercentDifference = pctDiff
        };

        return Task.FromResult<ScenarioComparison?>(comparison);
    }
}
