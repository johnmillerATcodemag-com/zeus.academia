using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data.Repositories;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Data;

/// <summary>
/// Unit of Work implementation for managing transactions and repository coordination.
/// Task 6: Repository Pattern Implementation - Unit of Work with transaction management.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AcademiaDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly Dictionary<Type, object> _repositories;
    private bool _disposed;

    private IAcademicRepository? _academics;
    private IDepartmentRepository? _departments;
    private ISubjectRepository? _subjects;

    public UnitOfWork(
        AcademiaDbContext context,
        IServiceProvider serviceProvider,
        ILogger<UnitOfWork> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _repositories = new Dictionary<Type, object>();
    }

    /// <inheritdoc />
    public IAcademicRepository Academics =>
        _academics ??= _serviceProvider.GetRequiredService<IAcademicRepository>();

    /// <inheritdoc />
    public IDepartmentRepository Departments =>
        _departments ??= _serviceProvider.GetRequiredService<IDepartmentRepository>();

    /// <inheritdoc />
    public ISubjectRepository Subjects =>
        _subjects ??= _serviceProvider.GetRequiredService<ISubjectRepository>();

    /// <inheritdoc />
    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);

        if (_repositories.TryGetValue(type, out var existingRepository))
        {
            return (IRepository<T>)existingRepository;
        }

        var repository = _serviceProvider.GetRequiredService<IRepository<T>>();
        _repositories[type] = repository;
        return repository;
    }

    /// <inheritdoc />
    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Beginning database transaction");
            var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            return new DbTransactionWrapper(transaction, _logger);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error beginning database transaction");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Saving changes to database");
            UpdateAuditFields();
            var result = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Saved {RecordCount} records to database", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Saving changes to database with user {UserId}", userId);
            UpdateAuditFields(userId);
            var result = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Saved {RecordCount} records to database", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database with user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Updates audit fields for tracked entities.
    /// </summary>
    /// <param name="userId">User identifier for audit</param>
    private void UpdateAuditFields(string? userId = null)
    {
        var entities = _context.ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        var now = DateTime.UtcNow;
        var auditUserId = userId ?? "System";

        foreach (var entity in entities)
        {
            if (entity.State == EntityState.Added)
            {
                entity.Entity.CreatedDate = now;
                entity.Entity.CreatedBy = auditUserId;
            }

            entity.Entity.ModifiedDate = now;
            entity.Entity.ModifiedBy = auditUserId;
        }
    }

    /// <summary>
    /// Disposes the Unit of Work and its resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the Unit of Work and its resources.
    /// </summary>
    /// <param name="disposing">True if disposing</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context?.Dispose();
            _repositories.Clear();
            _disposed = true;
        }
    }
}

/// <summary>
/// Wrapper for Entity Framework database transaction.
/// </summary>
internal class DbTransactionWrapper : IDbTransaction
{
    private readonly IDbContextTransaction _transaction;
    private readonly ILogger _logger;
    private bool _disposed;

    public DbTransactionWrapper(IDbContextTransaction transaction, ILogger logger)
    {
        _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Committing database transaction");
            await _transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing database transaction");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Rolling back database transaction");
            await _transaction.RollbackAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back database transaction");
            throw;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the transaction wrapper.
    /// </summary>
    /// <param name="disposing">True if disposing</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _disposed = true;
        }
    }

    /// <summary>
    /// Asynchronously disposes the transaction wrapper.
    /// </summary>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (!_disposed)
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
            _disposed = true;
        }
    }
}