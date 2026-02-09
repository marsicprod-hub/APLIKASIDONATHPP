namespace HppDonatApp;

using Microsoft.UI.Xaml;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using HppDonatApp.Core.Interfaces;
using HppDonatApp.Core.Services;
using HppDonatApp.Data;
using HppDonatApp.Data.Repositories;
using HppDonatApp.Services;
using Serilog;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private static IHost? _host;

    /// <summary>
    /// Initializes the singleton Application object.
    /// </summary>
    public App()
    {
        // Initialize Serilog for logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/hppdonat-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        this.InitializeComponent();

        // Setup dependency injection
        SetupServices();
    }

    /// <summary>
    /// Configures dependency injection for the application.
    /// </summary>
    private static void SetupServices()
    {
        var builder = new HostBuilder()
            .ConfigureServices(services =>
            {
                // Register DbContext
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite("Data Source=hppdonat.db"));

                // Register repositories
                services.AddScoped<IIngredientRepository, IngredientRepository>();
                services.AddScoped<IRecipeRepository, RecipeRepository>();
                services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

                // Register core services
                services.AddScoped<IPricingEngine, PricingEngine>();
                services.AddScoped<IPricingService, PricingService>();
                services.AddScoped<IFileExportService, FileExportService>();
                services.AddScoped<IScenarioService, ScenarioService>();

                // Register ViewModels
                services.AddTransient<MainWindow>();
            });

        _host = builder.Build();

        // Initialize database with seed data
        using (var scope = _host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
            SeedData.Initialize(context);
        }

        Log.Information("Application services configured successfully");
    }

    /// <summary>
    /// Gets the service provider for dependency injection.
    /// </summary>
    public static IServiceProvider Services => _host?.Services ?? throw new InvalidOperationException();

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            m_window = new MainWindow();
            m_window.Activate();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error launching application");
            throw;
        }
    }

    private Window? m_window;
}
