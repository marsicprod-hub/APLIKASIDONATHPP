namespace HppDonatApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HppDonatApp.Core.Models;
using HppDonatApp.Services;

/// <summary>
/// Main view model for the dashboard page.
/// Displays summary information and recent calculations.
/// </summary>
public partial class DashboardViewModel : ObservableObject
{
    private readonly IPricingService _pricingService;

    [ObservableProperty]
    private List<Recipe> recipes = [];

    [ObservableProperty]
    private Recipe? selectedRecipe;

    [ObservableProperty]
    private BatchResult? lastResult;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string statusMessage = "Ready";

    /// <summary>
    /// Initializes a new instance of the DashboardViewModel.
    /// </summary>
    /// <param name="pricingService">The pricing service.</param>
    public DashboardViewModel(IPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    /// <summary>
    /// Loads recipes for the dashboard.
    /// </summary>
    [RelayCommand]
    public async Task LoadRecipes()
    {
        IsLoading = true;
        StatusMessage = "Loading recipes...";

        try
        {
            // Simulated recipe loading - in real app would use repository
            await Task.Delay(500);
            StatusMessage = "Recipes loaded successfully";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading recipes: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Calculates cost for the selected recipe.
    /// </summary>
    [RelayCommand]
    public async Task CalculateCost()
    {
        if (SelectedRecipe == null)
        {
            StatusMessage = "Please select a recipe";
            return;
        }

        IsLoading = true;
        StatusMessage = "Calculating cost...";

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

            LastResult = await _pricingService.CalculateRecipeCostAsync(SelectedRecipe.Id, parameters);
            StatusMessage = $"Cost calculated. Unit price: {LastResult?.UnitCost:C0}";
        }
        catch (Exception ex)
        {
            LastResult = new BatchResult { Errors = new List<string> { ex.Message } };
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
