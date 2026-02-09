namespace HppDonatApp.Data;

using Microsoft.EntityFrameworkCore;
using HppDonatApp.Core.Models;

/// <summary>
/// Application DbContext for HPP Donat data persistence using SQLite.
/// Configures entity mappings and relationships for all core domain models.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the AppDbContext.
    /// </summary>
    /// <param name="options">DbContext options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>Gets or sets the ingredients DbSet.</summary>
    public DbSet<Ingredient> Ingredients { get; set; } = null!;

    /// <summary>Gets or sets the price history DbSet.</summary>
    public DbSet<PriceHistory> PriceHistories { get; set; } = null!;

    /// <summary>Gets or sets the recipes DbSet.</summary>
    public DbSet<Recipe> Recipes { get; set; } = null!;

    /// <summary>Gets or sets the recipe items DbSet.</summary>
    public DbSet<RecipeItem> RecipeItems { get; set; } = null!;

    /// <summary>
    /// Configures entity relationships and constraints.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Ingredient entity
        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Unit).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CurrentPrice).HasPrecision(18, 4); // decimal(18,4)
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Supplier).HasMaxLength(200);
            entity.Property(e => e.MinimumStock).HasPrecision(18, 4);

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Category);
        });

        // Configure PriceHistory entity
        modelBuilder.Entity<PriceHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.IngredientId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Price).HasPrecision(18, 4);
            entity.Property(e => e.Notes).HasMaxLength(500);

            // Configure foreign key relationship
            entity.HasOne(e => e.Ingredient)
                .WithMany()
                .HasForeignKey(e => e.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.IngredientId, e.Date });
        });

        // Configure Recipe entity
        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.WastePercent).HasPrecision(5, 4); // 0.0000 - 1.0000
            entity.Property(e => e.TheoreticalOutput);

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
        });

        // Configure RecipeItem entity - as owned type (nested without FK)
        modelBuilder.Entity<Recipe>()
            .OwnsMany(r => r.Items, navigationBuilder =>
            {
                navigationBuilder.Property(ri => ri.Quantity).HasPrecision(18, 4);
                navigationBuilder.Property(ri => ri.PricePerUnit).HasPrecision(18, 4);
                navigationBuilder.Property(ri => ri.Unit).HasMaxLength(50);
                navigationBuilder.Property(ri => ri.IngredientId).HasMaxLength(50);
            });
    }
}
