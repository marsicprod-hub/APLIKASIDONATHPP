# Architecture Overview

## System Architecture

HppDonatApp follows a **Clean Architecture** pattern with clear separation of concerns across 4 layers:

```
┌─────────────────────────────────────────────────────────────────┐
│                    Presentation Layer                           │
│                  (HppDonatApp - WinUI 3)                        │
│  ┌────────────┬──────────────────┬──────────────┐              │
│  │ Dashboard  │  Recipe Editor   │  Reports     │              │
│  │ View       │  View            │  View        │              │
│  └────────────┴──────────────────┴──────────────┘              │
│  ┌────────────┬──────────────────┬──────────────┐              │
│  │ Dashboard  │  Recipe Editor   │  Reports     │              │
│  │ ViewModel  │  ViewModel       │  ViewModel   │              │
│  └────────────┴──────────────────┴──────────────┘              │
└──────────────────────┬────────────────────────────────────────┘
                       │ ↓ Dependencies
┌──────────────────────▼────────────────────────────────────────┐
│                  Application Services Layer                   │
│              (HppDonatApp.Services)                           │
│  ┌─────────────────┬──────────────┬──────────────┐           │
│  │ PricingService  │ FileExport   │ ScenarioSvc  │           │
│  └─────────────────┴──────────────┴──────────────┘           │
└──────────────────────┬────────────────────────────────────────┘
                       │ ↓ Dependencies
┌──────────────────────▼────────────────────────────────────────┐
│                    Data Access Layer                          │
│               (HppDonatApp.Data)                              │
│  ┌────────────────────┬──────────────────────────┐           │
│  │   AppDbContext     │   Repositories           │           │
│  │  (EF Core/SQLite)  │  (IRepository<T>)        │           │
│  ├────────────────────┼──────────────────────────┤           │
│  │ - Migrations       │ - IngredientRepository   │           │
│  │ - Entity Config    │ - RecipeRepository       │           │
│  │ - Relationships    │ - PriceHistoryRepository │           │
│  └────────────────────┴──────────────────────────┘           │
└──────────────────────┬────────────────────────────────────────┘
                       │ ↓ Dependencies
┌──────────────────────▼────────────────────────────────────────┐
│                  Core Business Logic Layer                    │
│              (HppDonatApp.Core)                               │
│  ┌──────────────────┬─────────────────────────┐              │
│  │     Models       │   Services & Engines    │              │
│  ├──────────────────┼─────────────────────────┤              │
│  │ - Ingredient     │ - IPricingEngine        │              │
│  │ - Recipe         │ - PricingEngine         │              │
│  │ - RecipeItem     │ - IPricingStrategy      │              │
│  │ - PriceHistory   │ - Pricing Strategies    │              │
│  │ - BatchRequest   │ - UnitConverter         │              │
│  │ - BatchResult    │ - RoundingEngine        │              │
│  └──────────────────┴─────────────────────────┘              │
│                 (Core layer has NO dependencies)             │
└─────────────────────────────────────────────────────────────────┘
```

## Layer Responsibilities

### Core Layer (HppDonatApp.Core)
**Purpose:** Pure business logic with no external dependencies

**Components:**
- **Models:** Domain entities that represent core concepts
  - `Ingredient` - Product specifications, units, categories
  - `Recipe` - Collection of ingredients with quantities
  - `RecipeItem` - Owned entity linking ingredients to recipes
  - `PriceHistory` - Historical price tracking per ingredient
  - `BatchRequest` & `BatchResult` - Input/output for calculations
  
- **Interfaces:** Contracts for pluggable implementations
  - `IPricingEngine` - Core pricing calculation engine
  - `IPricingStrategy` - Strategy pattern for pricing algorithms

- **Services:**
  - `PricingEngine` (~287 lines) - Implements complex batch cost calculations
    - Ingredient cost aggregation
    - Oil cost with amortization
    - Energy cost per kwh
    - Labor cost with multiple roles
    - Overhead and packaging costs
    - Waste percentage calculation
    - Margin and VAT application
    
  - `PricingStrategies` (~125 lines) - 3 pluggable pricing strategies
    - `MarkupBasedStrategy` - Price = UnitCost × (1 + Markup%)
    - `TargetMarginStrategy` - Price = UnitCost ÷ (1 - TargetMargin%)
    - `CompetitiveRoundingStrategy` - Psychological pricing with rounding

- **Utilities:**
  - `UnitConverter` - Support for kg, gram, liter, ml, cup, oz, tbsp conversions
  - `RoundingEngine` - 6 rounding rules: round100, round250, round500, round1k, price99, noround

### Data Layer (HppDonatApp.Data)

**Purpose:** Database persistence and repository pattern

**Components:**
- **AppDbContext** - EF Core configuration
  - DbSets for Ingredients, Recipes, PriceHistories
  - Entity configuration with decimal(18,4) precision
  - Owned entity pattern for RecipeItems
  - Indexes on frequently queried fields
  - Seed data initialization

- **Repository Interfaces**
  - `IRepository<T>` - Generic CRUD operations
  - `IIngredientRepository` - Ingredient-specific queries
  - `IRecipeRepository` - Recipe-specific queries

- **Repository Implementations**
  - Generic repository base class with common CRUD
  - Specialized methods:
    - `GetByCategoryAsync()`, `GetBySupplierAsync()`
    - `GetCurrentPriceAsync()`, `UpdatePriceAsync()`
    - `GetActiveRecipesAsync()`, `GetWithItemsAsync()`

- **Seed Data** (~155 lines)
  - 2 complete recipes (Donat Original, Donat Cokelat)
  - 9 ingredients with suppliers and categories
  - 30+ price history entries for realistic pricing evolution

### Services Layer (HppDonatApp.Services)

**Purpose:** Business logic orchestration and external integrations

**Components:**
- **PricingService** (~92 lines)
  - `CalculateRecipeCostAsync()` - Full cost analysis
  - `CompareStrategiesAsync()` - Side-by-side strategy comparison
  - `SimulateScenarioAsync()` - What-if analysis engine

- **FileExportService** (~110 lines)
  - `ExportIngredientsAsync()` - CSV export to `ingredients.csv`
  - `ImportIngredientsAsync()` - CSV import with validation
  - `ExportRecipesAsync()` - Recipe export to CSV

- **ScenarioService** (~90 lines)
  - In-memory scenario storage and retrieval
  - `CompareScenarios()` - Comparative analysis
  - Data classes for `PricingScenario` and `ScenarioComparison`

### Presentation Layer (HppDonatApp - WinUI 3)

**Purpose:** User interface and view-model binding

**Components:**
- **App.xaml & App.xaml.cs** - Application bootstrap
  - Dependency injection configuration (IHost builder)
  - DbContext registration with SQLite
  - Repository and service registration
  - Serilog structured logging setup

- **MainWindow** - Navigation shell
  - NavigationView with menu items
  - Page container for routing
  - Status bar and logging integration

- **Views** - XAML pages with C# code-behind
  - `DashboardView` - Recipe selection and cost display
  - `RecipeEditorView` - Create/edit recipes interactively
  - `ReportsView` - Analytics placeholder (LiveCharts2 ready)

- **ViewModels** - MVVM binding layer
  - `DashboardViewModel` (~71 lines)
    - `LoadRecipesCommand` - Fetch from database
    - `CalculateCostCommand` - Trigger pricing engine
    - `INotifyPropertyChanged` binding for UI updates
    
  - `RecipeEditorViewModel` (~127 lines)
    - `CreateNewRecipeCommand` - Recipe creation
    - `LoadIngredientsCommand` - Populate selection
    - `PreviewCostCommand` - Live cost preview
    - `SaveRecipeCommand` - Persist changes

### Test Layer (HppDonatApp.Tests)

**Purpose:** Verification of all business logic and edge cases

**Components:**
- **PricingEngineTests** (~398 lines, 16 test methods)
  - Ingredient cost calculation
  - All cost components integration
  - Unit cost division
  - Margin percentage verification
  - Rounding rule application (5 variants)
  - Batch multiplier scaling
  - Waste percentage calculation (4 percentages)
  - Available strategies enumeration
  - VAT application (15%)
  - Invalid input handling

- **UtilityTests** (~128 lines, 14 test methods)
  - Unit conversion: kg↔gram, liter↔ml, etc.
  - Rounding engine with multiple rules
  - RecipeItem cost calculation
  - Recipe sellable units with waste
  - Edge cases: zero quantities, unknown units

## Design Patterns

### 1. Clean Architecture
- Concentric layers with inward dependencies only
- Core layer is completely independent
- Data and Services layers depend only on Core
- Presentation layer imports all lower layers

### 2. Repository Pattern
- `IRepository<T>` generic interface for CRUD
- Specialized repositories extend base functionality
- Database abstraction via EF Core

### 3. Strategy Pattern
- `IPricingStrategy` interface for pricing algorithms
- 3 concrete implementations (Markup, TargetMargin, Competitive)
- Runtime strategy selection

### 4. Dependency Injection
- Microsoft.Extensions.DependencyInjection
- Configuration in App.xaml.cs
- Service lifetimes: Transient (services), Scoped (DbContext), Singleton (engines)

### 5. MVVM (Model-View-ViewModel)
- CommunityToolkit.Mvvm for automatic binding
- RelayCommand for button/menu actions
- ObservableProperty for reactive updates
- Separation of concerns: Views → ViewModels → Services → Data

### 6. Owned Entities (EF Core)
- `RecipeItem` owned by `Recipe`
- No separate join table required
- Automatic cascade delete

## Data Flow Examples

### Scenario 1: Calculate Recipe Cost
```
User clicks "Calculate Cost" in Dashboard
         ↓
    DashboardViewModel.CalculateCostCommand
         ↓
    PricingService.CalculateRecipeCostAsync(recipeId)
         ↓
    RecipeRepository.GetWithItemsAsync(recipeId)
         ↓
    AppDbContext.Recipes
         ↓ (EF Core retrieves from SQLite)
    Database returns Recipe + nested RecipeItems
         ↓
    PricingEngine.CalculateBatchCost(recipe)
         ↓ (Execute complex calculations)
    Returns BatchResult with UnitCost, Margins, etc.
         ↓
    DashboardViewModel updates ObservableProperty
         ↓
    Binding system updates UI Label controls
         ↓
    User sees: UnitCost, SuggestedPrice, Margin%, PriceWithVAT
```

### Scenario 2: Import Ingredients from CSV
```
User selects CSV file in ReportsView
         ↓
    FileExportService.ImportIngredientsAsync(stream)
         ↓ (CsvHelper parses CSV)
    Validate each row (FluentValidation)
         ↓
    IngredientRepository.AddAsync(ingredient)
         ↓
    IngredientRepository.SaveChangesAsync()
         ↓
    AppDbContext pushes to SQLite database
         ↓
    UI notifies user of success
```

## Decimal Precision Strategy

All monetary calculations use `decimal(18,4)`:
- 18 digits total precision (supports up to billions)
- 4 decimal places (supports IDR currency ±0.0001)
- Prevents floating-point rounding errors
- Example: 1,250,000.5050 IDR

## Error Handling

- Services return `Result<T>` pattern (StatusCode + message)
- Domain models throw `ArgumentException` on invalid state
- Repositories propagate EF Core `DbException`
- ViewModels catch exceptions and update Status binding for user feedback
- Serilog logs all errors to `logs/hppdonat-{date}.txt`

---
*Architecture designed for maintainability, testability, and extensibility.*
