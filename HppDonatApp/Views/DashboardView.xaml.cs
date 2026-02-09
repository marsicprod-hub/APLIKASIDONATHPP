namespace HppDonatApp.Views;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using HppDonatApp.ViewModels;
using HppDonatApp.Services;

/// <summary>
/// Dashboard view for displaying calculation results.
/// </summary>
public sealed partial class DashboardView : Page
{
    private DashboardViewModel? _viewModel;

    public DashboardView()
    {
        this.InitializeComponent();
        Loaded += DashboardView_Loaded;
    }

    private async void DashboardView_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel = new DashboardViewModel(App.Services.GetService<IPricingService>()!);
        DataContext = _viewModel;
        await _viewModel.LoadRecipesCommand.ExecuteAsync(null);
    }

    private async void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel?.SelectedRecipe != null)
        {
            await _viewModel.CalculateCostCommand.ExecuteAsync(null);

            if (_viewModel.LastResult != null)
            {
                UnitCostText.Text = $"Unit Cost: {_viewModel.LastResult.UnitCost:C0}";
                SuggestedPriceText.Text = $"Suggested Price: {_viewModel.LastResult.SuggestedPrice:C0}";
                MarginText.Text = $"Margin: {(_viewModel.LastResult.MarginPercent * 100):F1}%";
                PriceWithVatText.Text = $"Price (with VAT): {_viewModel.LastResult.PriceIncludingVat:C0}";
            }
        }
    }
}
