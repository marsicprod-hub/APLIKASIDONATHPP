# Contributing to HPP Donat Calculator

Thank you for your interest in contributing! This document provides guidelines and instructions for contributing to this project.

## Code of Conduct

Please be respectful and professional in all interactions. We're committed to providing a welcoming and inclusive environment.

## Getting Started

### Prerequisites
- .NET 10 SDK
- Visual Studio 2022 (17.9+) or VS Code
- Git

### Development Setup

```bash
# Clone your fork
git clone https://github.com/YourUsername/HppDonatApp.git
cd HppDonatApp

# Create a feature branch
git switch -c feature/your-feature-name

# Build and test
dotnet restore && dotnet build && dotnet test
```

## Development Guidelines

### Code Style
- Follow Microsoft's [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use `var` for obvious types, explicit types otherwise
- Use meaningful variable names
- Keep methods small and focused

### Commits
- Use conventional commit format:
  - `feat: description` for new features
  - `fix: description` for bug fixes
  - `docs: description` for documentation
  - `test: description` for tests
  - `chore: description` for maintenance

Example:
```
feat(core): implement ingredient substitution heuristics
test: add unit tests for new substitution logic
docs: update pricing engine documentation
```

### Testing Requirements
- Write unit tests for all new features
- Ensure all existing tests pass: `dotnet test`
- Target minimum 80% code coverage for new code
- Test edge cases and error conditions
- Use meaningful test names describing the scenario

### Documentation
- Add XML doc comments to all public methods
- Update README.md if adding features
- Document breaking changes
- Provide usage examples for new functionality

## Pull Request Process

1. **Create a feature branch** from `develop`:
   ```bash
   git switch develop
   git pull origin develop
   git switch -c feature/my-feature
   ```

2. **Make your changes** with tests and documentation

3. **Verify everything works**:
   ```bash
   dotnet restore && dotnet build && dotnet test
   ```

4. **Commit with meaningful messages**:
   ```bash
   git add .
   git commit -m "feat(feature): add new capability"
   ```

5. **Push to your fork**:
   ```bash
   git push origin feature/my-feature
   ```

6. **Create a Pull Request**:
   - Title: Brief description of changes
   - Description: Explain what, why, and how
   - Link related issues
   - Include before/after examples if applicable

7. **Address review feedback** - maintain conversation and iterate

## Areas for Contribution

### High Priority
- [ ] Unit test expansion (edge cases, error scenarios)
- [ ] Documentation improvements
- [ ] Performance optimizations
- [ ] UI/UX enhancements

### Medium Priority
- [ ] LiveCharts2 visualization integration
- [ ] Advanced forecasting algorithms
- [ ] Additional pricing strategies
- [ ] Price import/export improvements

### Low Priority
- [ ] Code refactoring for clarity
- [ ] Additional utility functions
- [ ] Example projects
- [ ] Localization

## Reporting Issues

### Before Reporting
- Check existing issues to avoid duplicates
- Test with the latest main branch code
- Verify the issue reproduces consistently

### Bug Reports
Include:
- Description of the bug
- Steps to reproduce
- Expected vs actual behavior
- Environment (OS, .NET version, etc.)
- Screenshots/logs if applicable

### Feature Requests
Include:
- Use case and motivation
- Proposed solution
- Alternative approaches considered
- Any potential breaking changes

## Project Structure

Key folders:
- `/HppDonatApp.Core` - Core business logic
- `/HppDonatApp.Data` - Data access layer
- `/HppDonatApp.Services` - Application services
- `/HppDonatApp` - WinUI3 application
- `/HppDonatApp.Tests` - Unit tests

## Review Process

1. Automated checks (CI/CD pipeline)
2. Code review by maintainers
3. Approval and merge to develop
4. Integration testing
5. Release to main branch

## Performance Considerations

- Minimize database queries (use efficient filters)
- Cache frequently accessed data
- Use decimal for financial calculations
- Profile before optimizing

## Architecture Principles

- **SOLID Principles**: Single responsibility, Open/closed, Liskov, Interface segregation, Dependency inversion
- **Dependency Injection**: Use DI for all services
- **MVVM Pattern**: WinUI templates follow MVVM
- **Repository Pattern**: Abstract data access
- **Domain-Driven Design**: Core business logic isolated

## Questions?

- Check [README.md](README.md) for project overview
- Review existing code and tests
- Open a discussion issue
- Contact maintainers

---

**Thank you for contributing!** Every contribution makes this project better. 🎉
