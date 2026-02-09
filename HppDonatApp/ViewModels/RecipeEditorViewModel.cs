namespace HppDonatApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HppDonatApp.Core.Models;
using HppDonatApp.Data.Repositories;
using HppDonatApp.Services;

/// <summary>
/// View model for the recipe editor page.
/// Handles recipe creation, editing, and ingredient management.
/// </summary>
public partial class RecipeEditorViewModel : ObservableObject
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IPricingService _pricingService;

    [ObservableProperty]
    private Recipe? currentRecipe;

    [ObservableProperty]
    private List<Ingredient> availableIngredients = [];

    [ObservableProperty]
    private BatchResult? previewResult;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private string recipeName = "New Recipe";

    [ObservableProperty]
    private int theoreticalOutput = 100;

    [ObservableProperty]
    private decimal wastePercent = 0.05m;

    /// <summary>
    /// Initializes a new instance of the RecipeEditorViewModel.
    /// </summary>
    public RecipeEditorViewModel(
        IRecipeRepository recipeRepository,
        IIngredientRepository ingredientRepository,
        IPricingService pricingService)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _pricingService = pricingService;
    }

    /// <summary>
    /// Loads available ingredients for selection.
    /// </summary>
    [RelayCommand]
    public async Task LoadIngredients()
    {
        IsLoading = true;
        StatusMessage = "Loading ingredients...";

        try
        {
            AvailableIngredients = (await _ingredientRepository.GetAllAsync()).ToList();
            StatusMessage = $"Loaded {AvailableIngredients.Count} ingredients";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading ingredients: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Creates a new recipe.
    /// </summary>
    [RelayCommand]
    public async Task CreateNewRecipe()
    {
        CurrentRecipe = new Recipe
        {
            Id = Guid.NewGuid().ToString(),
            Name = RecipeName,
            Description = "New donut recipe",
            TheoreticalOutput = TheoreticalOutput,
            WastePercent = WastePercent,
            Items = new List<RecipeItem>()
        };

        StatusMessage = $"Created new recipe: {RecipeName}";
        await Task.CompletedTask;
    }

    /// <summary>
    /// Previews the cost for the current recipe.
    /// </summary>
    [RelayCommand]
    public async Task PreviewCost()
    {
        if (CurrentRecipe == null || !CurrentRecipe.Items.Any())
        {
            StatusMessage = "Recipe must have at least one ingredient";
            return;
        }

        IsLoading = true;
        StatusMessage = "Calculating preview...";

        try
        {
            var parameters = new Dictionary<string, decimal>
            {
                { "markup", 0.5m },
                { "oilUsed", 2m },
                { "oilPrice", 18000m },
                { "energy", 5m },
                { "energyRate", 3000m }
            };

            PreviewResult = await _pricingService.CalculateRecipeCostAsync(CurrentRecipe.Id, parameters);
            StatusMessage = "Preview calculated successfully";
        }
        catch (Exception ex)
        {
            PreviewResult = new BatchResult { Errors = new List<string> { ex.Message } };
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Saves the current recipe.
    /// </summary>
    [RelayCommand]
    public async Task SaveRecipe()
    {
        if (CurrentRecipe == null)
        {
            StatusMessage = "No recipe to save";
            return;
        }

        IsLoading = true;
        StatusMessage = "Saving recipe...";

        try
        {
            CurrentRecipe.UpdatedAt = DateTime.UtcNow;
            await _recipeRepository.UpdateAsync(CurrentRecipe);
            StatusMessage = $"Recipe '{CurrentRecipe.Name}' saved successfully";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving recipe: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
