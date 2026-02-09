namespace HppDonatApp;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

/// <summary>
/// Main application window with navigation.
/// </summary>
public sealed partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the MainWindow class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();

        // Set window properties
        Title = "HPP Donat Calculator - .NET 10 WinUI 3";
        
        // Initialize navigation
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    /// <summary>
    /// Handles navigation view selection change.
    /// </summary>
    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item)
        {
            var tag = item.Tag?.ToString() ?? string.Empty;
            NavigateToPage(tag);
        }
    }

    /// <summary>
    /// Navigates to the specified page.
    /// </summary>
    /// <param name="tag">The page tag.</param>
    private void NavigateToPage(string tag)
    {
        Type? pageType = tag switch
        {
            "dashboard" => typeof(Views.DashboardView),
            "recipe" => typeof(Views.RecipeEditorView),
            "reports" => typeof(Views.ReportsView),
            _ => typeof(Views.DashboardView)
        };

        if (pageType != null)
        {
            ContentFrame.Navigate(pageType);
        }
    }
}
