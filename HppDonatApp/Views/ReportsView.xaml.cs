namespace HppDonatApp.Views;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using HppDonatApp.Services;

/// <summary>
/// Reports view for displaying analytics and data export options.
/// </summary>
public sealed partial class ReportsView : Page
{
    private IFileExportService? _exportService;

    public ReportsView()
    {
        this.InitializeComponent();
        Loaded += ReportsView_Loaded;
    }

    private void ReportsView_Loaded(object sender, RoutedEventArgs e)
    {
        _exportService = App.Services.GetService<IFileExportService>();
    }

    private async void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        ExportStatusText.Text = "Export feature will be available in full version";
        await Task.Delay(1000);
        ExportStatusText.Text = "Ready";
    }
}
