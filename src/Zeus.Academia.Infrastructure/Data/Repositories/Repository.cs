using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Zeus.Academia.Infrastructure.Data.Repositories;

/// <summary>
/// Generic repository base implementation using Entity Framework.
/// Task 6: Repository Pattern Implementation - Generic repository with EF Core.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AcademiaDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<Repository<T>> _logger;

    /// <summary>
    /// Initializes a new instance of the Repository class.
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="logger">Logger instance</param>
    public Repository(AcademiaDbContext context, ILogger<Repository<T>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbSet = _context.Set<T>();
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting entity {EntityType} with ID {Id}", typeof(T).Name, id);
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Finding entities of type {EntityType} with predicate", typeof(T).Name);
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding entities of type {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting first entity of type {EntityType} with predicate", typeof(T).Name);
            return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting first entity of type {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<T> SingleAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting single entity of type {EntityType} with predicate", typeof(T).Name);
            return await _dbSet.SingleAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting single entity of type {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting single or default entity of type {EntityType} with predicate", typeof(T).Name);
            return await _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting single or default entity of type {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _logger.LogDebug("Adding entity of type {EntityType}", typeof(T).Name);
            var entry = await _dbSet.AddAsync(entity, cancellationToken);
            return entry.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entityList = entities.ToList();
            _logger.LogDebug("Adding {Count} entities of type {EntityType}", entityList.Count, typeof(T).Name);
            await _dbSet.AddRangeAsync(entityList, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding range of entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual T Update(T entity)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _logger.LogDebug("Updating entity of type {EntityType}", typeof(T).Name);
            var entry = _dbSet.Update(entity);
            return entry.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        try
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entityList = entities.ToList();
            _logger.LogDebug("Updating {Count} entities of type {EntityType}", entityList.Count, typeof(T).Name);
            _dbSet.UpdateRange(entityList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating range of entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual void Remove(T entity)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _logger.LogDebug("Removing entity of type {EntityType}", typeof(T).Name);
            _dbSet.Remove(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        try
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

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

    /// <inheritdoc />
    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if any entity of type {EntityType} matches predicate", typeof(T).Name);
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if any entity of type {EntityType} matches predicate", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Counting entities of type {EntityType} with predicate", typeof(T).Name);
            return await _dbSet.CountAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting entities of type {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting paged entities of type {EntityType} (skip: {Skip}, take: {Take})", typeof(T).Name, skip, take);
            return await _dbSet.Skip(skip).Take(take).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int skip, int take, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting paged entities of type {EntityType} with predicate (skip: {Skip}, take: {Take})", typeof(T).Name, skip, take);
            return await _dbSet.Where(predicate).Skip(skip).Take(take).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged entities of type {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }
}