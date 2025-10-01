using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;

namespace Zeus.Academia.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation for coordinating repository operations and managing transactions.
/// </summary>
public class UnitOfWork : Repositories.Interfaces.IUnitOfWork
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _transaction;
    private bool _disposed = false;

    // Repository instances
    private IAcademicRepository? _academicRepository;
    private IDepartmentRepository? _departmentRepository;
    private ISubjectRepository? _subjectRepository;

    // Generic repository cache
    private readonly Dictionary<Type, object> _repositories = new();

    /// <summary>
    /// Initializes a new instance of the UnitOfWork class.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger instance</param>
    /// <param name="academicRepository">The academic repository</param>
    /// <param name="departmentRepository">The department repository</param>
    /// <param name="subjectRepository">The subject repository</param>
    public UnitOfWork(
        AcademiaDbContext context,
        ILogger<UnitOfWork> logger,
        IAcademicRepository academicRepository,
        IDepartmentRepository departmentRepository,
        ISubjectRepository subjectRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _academicRepository = academicRepository ?? throw new ArgumentNullException(nameof(academicRepository));
        _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
        _subjectRepository = subjectRepository ?? throw new ArgumentNullException(nameof(subjectRepository));
    }

    /// <inheritdoc/>
    public IAcademicRepository Academics
    {
        get
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UnitOfWork));
            return _academicRepository!;
        }
    }

    /// <inheritdoc/>
    public IDepartmentRepository Departments
    {
        get
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UnitOfWork));
            return _departmentRepository!;
        }
    }

    /// <inheritdoc/>
    public ISubjectRepository Subjects
    {
        get
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UnitOfWork));
            return _subjectRepository!;
        }
    }

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Saving changes to database");
            var result = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Successfully saved {ChangeCount} changes to database", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                _logger.LogWarning("Transaction already in progress");
                return;
            }

            _logger.LogDebug("Beginning database transaction");
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Database transaction started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error beginning database transaction");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction == null)
            {
                _logger.LogWarning("No transaction to commit");
                return;
            }

            _logger.LogDebug("Committing database transaction");
            await _transaction.CommitAsync(cancellationToken);
            _logger.LogDebug("Database transaction committed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing database transaction");
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    /// <inheritdoc/>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction == null)
            {
                _logger.LogWarning("No transaction to rollback");
                return;
            }

            _logger.LogDebug("Rolling back database transaction");
            await _transaction.RollbackAsync(cancellationToken);
            _logger.LogDebug("Database transaction rolled back successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back database transaction");
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        try
        {
            await BeginTransactionAsync(cancellationToken);
            var result = await operation();
            await SaveChangesAsync(cancellationToken);
            await CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing operation in transaction");
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        try
        {
            await BeginTransactionAsync(cancellationToken);
            await operation();
            await SaveChangesAsync(cancellationToken);
            await CommitTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing operation in transaction");
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc/>
    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(UnitOfWork));

        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            // Create a generic repository using the basic Repository<T> implementation
            var repositoryType = typeof(Repository<>).MakeGenericType(type);

            // Create a null logger for the repository
            var loggerType = typeof(ILogger<>).MakeGenericType(repositoryType);
            var logger = NullLoggerFactory.Instance.CreateLogger(repositoryType);

            var repository = Activator.CreateInstance(repositoryType, _context, logger);
            _repositories[type] = repository!;
        }

        return (IRepository<T>)_repositories[type];
    }

    /// <summary>
    /// Disposes the current transaction.
    /// </summary>
    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the unit of work and its resources.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _academicRepository = null;
            _departmentRepository = null;
            _subjectRepository = null;
            _repositories.Clear();
            _disposed = true;
        }
    }
}