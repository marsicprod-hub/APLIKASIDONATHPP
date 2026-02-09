namespace HppDonatApp.Data.Repositories;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Generic repository interface for basic CRUD operations on entities.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>Gets an entity by its primary key.</summary>
    /// <param name="id">The primary key value.</param>
    /// <returns>The entity or null if not found.</returns>
    Task<TEntity?> GetByIdAsync(object id);

    /// <summary>Gets all entities.</summary>
    /// <returns>Collection of all entities.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync();

    /// <summary>Finds entities matching a predicate.</summary>
    /// <param name="predicate">The predicate filter.</param>
    /// <returns>Collection of matching entities.</returns>
    Task<IEnumerable<TEntity>> FindAsync(Func<TEntity, bool> predicate);

    /// <summary>Adds a new entity.</summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The added entity.</returns>
    Task<TEntity> AddAsync(TEntity entity);

    /// <summary>Updates an existing entity.</summary>
    /// <param name="entity">The entity to update.</param>
    Task UpdateAsync(TEntity entity);

    /// <summary>Deletes an entity.</summary>
    /// <param name="id">The primary key of the entity to delete.</param>
    Task DeleteAsync(object id);

    /// <summary>Saves all pending changes.</summary>
    Task SaveChangesAsync();
}

/// <summary>
/// Generic repository implementation for database access with EF Core.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    /// <summary>
    /// Initializes a new instance of the Repository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(object id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Func<TEntity, bool> predicate)
    {
        return await Task.FromResult(DbSet.Where(predicate).ToList());
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await DbSet.AddAsync(entity);
        await SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        DbSet.Update(entity);
        await SaveChangesAsync();
    }

    public async Task DeleteAsync(object id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            DbSet.Remove(entity);
            await SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }
}
