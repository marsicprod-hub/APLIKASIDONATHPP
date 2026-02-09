# Changelog

All notable changes to this project are documented in this file.

## [1.0.0] - 2026-02-09

### Added
- **Initial Release**
- Complete pricing engine with 8+ cost components
- Support for decimal precision in all monetary calculations
- Three pluggable pricing strategies:
  - Fixed Markup Strategy
  - Target Margin Strategy
  - Competitive Rounding/Psychological Pricing Strategy
- Recipe management system with versioning
- Ingredient database with price history (30+ days)
- Batch/scale calculations with multiplier support
- Oil amortization calculations
- Labor cost allocation with multiple roles
- Energy consumption tracking
- Overhead allocation per batch
- Packaging cost per unit
- VAT support (configurable)
- Multiple rounding rules (100, 250, 500, 1000, psychological)

### Database
- SQLite integration with Entity Framework Core
- Two seed recipes:
  - Donat Original (vanilla)
  - Donat Cokelat (chocolate)
- Nine sample ingredients with prices
- 30+ days of price history data
- Migrations support

### Services Layer
- PricingService for high-level cost calculations
- Scenario service for "what-if" analysis
- FileExportService for CSV import/export
- Repository pattern with IngredientRepository and RecipeRepository
- Caching support with MemoryCache

### UI/WinUI3
- Modern WinUI 3 application shell
- Dashboard view with calculation results
- Recipe Editor view for recipe management
- Reports view for analytics
- Navigation between pages
- MVVM pattern with CommunityToolkit.Mvvm
- Responsive layouts

### Testing
- 18+ comprehensive unit tests
  - 10+ PricingEngine tests
  - 8+ Utility tests
- Tests verify monetary calculations to 2 decimal places
- Tests cover edge cases and error scenarios
- xUnit test framework

### Development & CI/CD
- GitHub Actions workflow for build and test
- Supports Windows latest runner
- Automated NuGet restore and build
- Test execution and coverage reporting
- .NET 10 SDK configuration

### Documentation
- Comprehensive README with examples
- CONTRIBUTING guidelines
- CHANGELOG (this file)
- MIT License
- Architecture overview
- Usage examples with code snippets
- Database schema documentation
- API documentation via XML comments

### Core Library Files
- PricingEngine.cs (~250 lines, fully documented)
- Three strategy implementations (~120 lines)
- UnitConverter utility (~100 lines)
- RoundingEngine utility (~100 lines)
- Models: Ingredient, Recipe, RecipeItem, PriceHistory, BatchRequest, BatchResult
- Interfaces: IPricingEngine, IPricingStrategy
- Total Core: 600+ lines of business logic

### Data Access
- AppDbContext with EF Core configuration
- SeedData initialization
- Generic Repository pattern
- Specialized IngredientRepository
- Specialized RecipeRepository
- EF Core migrations ready
- Owned entity configuration for RecipeItems

### Services
- 300+ lines across PricingService, FileExportService, ScenarioService
- Async/await throughout
- Comprehensive error handling
- Logging integration

### Total Generated Code
- **500+ lines** across core business logic
- **1000+ lines** across all layers
- **250+ XML documentation comments**
- **18+ unit tests** with comprehensive assertions

## [Unreleased / Roadmap]

### Planned Features
- [ ] LiveCharts2 integration for visual cost breakdown
- [ ] Advanced trend forecasting with trend analysis
- [ ] Ingredient substitution suggestions based on price spikes
- [ ] Multi-user support with role-based access control
- [ ] Cloud synchronization and backup
- [ ] REST API for third-party integrations
- [ ] Mobile application (MAUI)
- [ ] Advanced analytics dashboard
- [ ] Recipe templates library
- [ ] Batch production history tracking
- [ ] Profitability analysis tools
- [ ] Supplier comparison tools
- [ ] Email notifications for price changes
- [ ] Internationalization (i18n) support
- [ ] Performance metrics and KPI tracking

### Known Limitations
- WinUI 3 platform (Windows-only)
- SQLite single-user limitation
- Basic charting placeholders (ready for LiveCharts2 integration)
- No multi-currency support yet
- No recipe cloning feature yet

---

## Release Notes Format

Each release follows this structure:
- **Added** - New features
- **Changed** - Changes to existing functionality  
- **Deprecated** - Features marked for removal
- **Removed** - Removed features
- **Fixed** - Bug fixes
- **Security** - Security improvements

## Version Numbering

This project uses [Semantic Versioning](https://semver.org/):
- Major.Minor.Patch
- MAJOR = breaking changes
- MINOR = new features (backward compatible)
- PATCH = bug fixes (backward compatible)

## Support Policy

- v1.0.x - Current version, actively supported
- v0.x.x - Legacy, no longer supported

---

**Last Updated**: 2026-02-09  
**Next Release Target**: v1.1.0 (with LiveCharts2 integration)
