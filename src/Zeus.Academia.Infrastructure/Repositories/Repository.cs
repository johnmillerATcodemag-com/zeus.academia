using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;

namespace Zeus.Academia.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation providing common CRUD operations for entities.
/// </summary>
/// <typeparam name="T">The entity type that inherits from BaseEntity</typeparam>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AcademiaDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<Repository<T>> _logger;

    /// <summary>
    /// Initializes a new instance of the Repository class.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger instance</param>
    public Repository(AcademiaDbContext context, ILogger<Repository<T>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbSet = context.Set<T>();
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all entities of type {EntityType}", typeof(T).Name);
            return await _dbSet.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate);
            _logger.LogDebug("Finding entities of type {EntityType} with predicate", typeof(T).Name);
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate);
            _logger.LogDebug("Getting single entity of type {EntityType} with predicate", typeof(T).Name);
            return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting single entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);
            _logger.LogDebug("Adding entity of type {EntityType}", typeof(T).Name);

            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;

            var entry = await _dbSet.AddAsync(entity, cancellationToken);
            return entry.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities);
            var entityList = entities.ToList();
            _logger.LogDebug("Adding {Count} entities of type {EntityType}", entityList.Count, typeof(T).Name);

            var now = DateTime.UtcNow;
            foreach (var entity in entityList)
            {
                entity.CreatedDate = now;
                entity.ModifiedDate = now;
            }

            await _dbSet.AddRangeAsync(entityList, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding range of entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);
            _logger.LogDebug("Updating entity of type {EntityType}", typeof(T).Name);

            entity.ModifiedDate = DateTime.UtcNow;
            _dbSet.Update(entity);

            return await Task.FromResult(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);
            _logger.LogDebug("Removing entity of type {EntityType}", typeof(T).Name);

            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task RemoveByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Removing entity of type {EntityType} with ID {Id}", typeof(T).Name, id);

            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                await RemoveAsync(entity, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities);
            var entityList = entities.ToList();
            _logger.LogDebug("Removing {Count} entities of type {EntityType}", entityList.Count, typeof(T).Name);

            _dbSet.RemoveRange(entityList);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing range of entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Counting entities of type {EntityType}", typeof(T).Name);

            if (predicate == null)
                return await _dbSet.CountAsync(cancellationToken);

            return await _dbSet.CountAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate);
            _logger.LogDebug("Checking existence of entities of type {EntityType}", typeof(T).Name);

            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1) throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));
            if (pageSize < 1) throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

            _logger.LogDebug("Getting paged entities of type {EntityType}, page {PageNumber}, size {PageSize}",
                typeof(T).Name, pageNumber, pageSize);

            return await _dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(predicate, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual T Update(T entity)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);
            _logger.LogDebug("Updating entity of type {EntityType}", typeof(T).Name);

            entity.ModifiedDate = DateTime.UtcNow;
            _dbSet.Update(entity);

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual void Remove(T entity)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);
            _logger.LogDebug("Removing entity of type {EntityType}", typeof(T).Name);

            _dbSet.Remove(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities);
            var entityList = entities.ToList();
            _logger.LogDebug("Updating {Count} entities of type {EntityType}", entityList.Count, typeof(T).Name);

            var now = DateTime.UtcNow;
            foreach (var entity in entityList)
            {
                entity.ModifiedDate = now;
            }

            _dbSet.UpdateRange(entityList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating range of entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities);
            var entityList = entities.ToList();
            _logger.LogDebug("Removing {Count} entities of type {EntityType}", entityList.Count, typeof(T).Name);

            _dbSet.RemoveRange(entityList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing range of entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }
}