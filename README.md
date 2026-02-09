# HPP Donat Calculator - WinUI 3 Application

A modern, feature-rich desktop application for calculating HPP (Harga Pokok Penjualan/Cost of Goods) for donuts using WinUI 3 and .NET 10.

## Overview

This application provides a comprehensive toolkit for donut manufacturers to:
- **Calculate accurate HPP** with support for multiple cost factors
- **Manage recipes** with detailed ingredient tracking
- **Analyze pricing scenarios** using what-if analysis
- **Compare pricing strategies** (markup-based, target margin, psychological pricing)
- **Export/Import data** for business integration
- **Visualize cost breakdowns** with modern charts and analytics

## Features

### 🧮 Core Pricing Engine
- **Smart Calculations**: Ingredient cost, oil amortization, energy consumption, labor allocation, overhead distribution, packaging costs
- **Multiple Cost Factors**: Supports complex scenarios with 8+ cost components
- **Decimal Precision**: All calculations use `decimal` type for accurate monetary values
- **Pluggable Strategies**: Switch between different pricing strategies at runtime
  - Fixed Markup Strategy
  - Target Margin Strategy  
  - Competitive Rounding Strategy

### 📊 Recipe Management
- Create and edit donut recipes with multiple ingredients
- Track theoretical output and waste percentages
- Version control for recipe changes
- Batch size multiplier support

### 🎯 Scenario Analysis
- Simulate "what-if" scenarios with price changes
- Compare multiple pricing strategies
- Save and manage scenarios for future reference
- Trend-based forecasting

### 💾 Data Management
- SQLite database with EF Core
- CSV import/export functionality
- Recipe and ingredient versioning
- Price history tracking (30+ days of historical data)

### 🎨 Modern UI
- **WinUI 3** with responsive design
- **MVVM Pattern** using CommunityToolkit.Mvvm
- **Navigation View** for intuitive page navigation
- Multiple views: Dashboard, Recipe Editor, Reports
- Real-time calculation preview
- Dark/Light theme support

## Technical Stack

### Framework & Language
- **.NET 10** with C# 13
- **WinUI 3** for modern Windows UI
- **x64 Windows 10 Build 22621+** targeting

### Libraries & Tools
- **CommunityToolkit.Mvvm** - MVVM helpers
- **Entity Framework Core** - ORM with SQLite
- **CsvHelper** - CSV I/O operations
- **Serilog** - Structured logging
- **WinUIEx** - Additional WinUI utilities
- **AutoMapper** - Object mapping
- **FluentValidation** - Validation rules
- **xUnit** - Unit testing framework

## Project Structure

```
HppDonatApp.sln
├── HppDonatApp/                    # WinUI Application
│   ├── Views/                      # XAML Pages
│   ├── ViewModels/                 # MVVM ViewModels
│   ├── Controls/                   # Custom Controls
│   ├── App.xaml, MainWindow.xaml
│   └── Program.cs                  # Entry point
│
├── HppDonatApp.Core/               # Core Business Logic
│   ├── Models/                     # Domain Models
│   ├── Interfaces/                 # Service Contracts
│   ├── Services/                   # PricingEngine & Strategies
│   └── Utils/                      # Utilities
│
├── HppDonatApp.Data/               # Data Access Layer
│   ├── AppDbContext.cs
│   ├── SeedData.cs
│   └── Repositories/
│
├── HppDonatApp.Services/           # Application Services
│   ├── PricingService.cs
│   ├── FileExportService.cs
│   └── ScenarioService.cs
│
└── HppDonatApp.Tests/              # Unit Tests

.github/workflows/build.yml         # CI/CD Pipeline
```

## Installation & Setup

### Prerequisites
- .NET 10 SDK or later
- Visual Studio 2022 (17.9+) with WinUI 3 workload
- Windows 10 Build 22621 or later

### Building

```bash
git clone https://github.com/marsicprod-hub/HppDonatApp.git
cd HppDonatApp

dotnet restore HppDonatApp.sln
dotnet build HppDonatApp.sln
dotnet test HppDonatApp.sln
dotnet run --project HppDonatApp/HppDonatApp.csproj
```

## Database Seed Data

### Recipes (2)
- Donat Original - Classic vanilla donut
- Donat Cokelat - Chocolate donut

### Ingredients (9)
- Tepung Terigu (Wheat Flour)
- Gula Pasir (Sugar)
- Minyak Goreng (Cooking Oil)
- Telur Ayam (Eggs)
- Cokelat Bubuk (Cocoa Powder)
- Baking Powder
- Garam (Salt)
- Susu Bubuk (Milk Powder)
- Kemasan Plastik (Packaging)

### Price History
- 30+ days of historical pricing for trend analysis

## Unit Tests

```bash
# Run all tests
dotnet test HppDonatApp.sln

# With coverage report
dotnet test HppDonatApp.sln /p:CollectCoverage=true
```

**Test Coverage**: 18+ unit tests covering:
- Pricing engine calculations (10+ tests)
- Utility functions (8+ tests)
- All monetary calculations verified to 2 decimal places

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

MIT License - see [LICENSE](LICENSE)

## Changelog

See [CHANGELOG.md](CHANGELOG.md)

---

**Version**: 1.0.0 | **.NET**: 10.0 | **WinUI**: 3.0+ | **Database**: SQLite

Created 2026-02-09 by marsicprod-hub