namespace HppDonatApp.Views;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using HppDonatApp.ViewModels;
using HppDonatApp.Services;
using HppDonatApp.Data.Repositories;

/// <summary>
/// Recipe editor view for creating and managing recipes.
/// </summary>
public sealed partial class RecipeEditorView : Page
{
    private RecipeEditorViewModel? _viewModel;

    public RecipeEditorView()
    {
        this.InitializeComponent();
        Loaded += RecipeEditorView_Loaded;
    }

    private async void RecipeEditorView_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel = new RecipeEditorViewModel(
            App.Services.GetService<IRecipeRepository>()!,
            App.Services.GetService<IIngredientRepository>()!,
            App.Services.GetService<IPricingService>()!);

        DataContext = _viewModel;
        await _viewModel.LoadIngredientsCommand.ExecuteAsync(null);
    }

    private async void CreateRecipeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.RecipeName = RecipeNameBox.Text ?? "New Recipe";
            var outputValue = OutputBox.Value;
            _viewModel.TheoreticalOutput = (int)(double.IsNaN(outputValue) ? 100d : outputValue);

            var wasteValue = WasteBox.Value;
            _viewModel.WastePercent = (decimal)(double.IsNaN(wasteValue) ? 0.05d : wasteValue);

            await _viewModel.CreateNewRecipeCommand.ExecuteAsync(null);
            StatusText.Text = _viewModel.StatusMessage;
        }
    }

    private async void PreviewButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel != null)
        {
            await _viewModel.PreviewCostCommand.ExecuteAsync(null);
            
            if (_viewModel.PreviewResult != null)
            {
                PreviewCostText.Text = $"Unit Cost: {_viewModel.PreviewResult.UnitCost:C0}";
                PreviewPriceText.Text = $"Suggested Price: {_viewModel.PreviewResult.SuggestedPrice:C0}";
            }
            
            StatusText.Text = _viewModel.StatusMessage;
        }
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel != null)
        {
            await _viewModel.SaveRecipeCommand.ExecuteAsync(null);
            StatusText.Text = _viewModel.StatusMessage;
        }
    }
}
