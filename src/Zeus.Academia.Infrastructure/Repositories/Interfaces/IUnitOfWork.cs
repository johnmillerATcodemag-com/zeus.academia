using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;

namespace Zeus.Academia.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Unit of Work pattern interface for coordinating repository operations and managing transactions.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the Academic repository.
    /// </summary>
    IAcademicRepository Academics { get; }

    /// <summary>
    /// Gets the Department repository.
    /// </summary>
    IDepartmentRepository Departments { get; }

    /// <summary>
    /// Gets the Subject repository.
    /// </summary>
    ISubjectRepository Subjects { get; }

    /// <summary>
    /// Saves all changes made in this unit of work to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the async operation</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the async operation</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the async operation</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a function within a database transaction asynchronously.
    /// </summary>
    /// <typeparam name="T">The return type of the function</typeparam>
    /// <param name="operation">The operation to execute within the transaction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the operation</returns>
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action within a database transaction asynchronously.
    /// </summary>
    /// <param name="operation">The operation to execute within the transaction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the async operation</returns>
    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a generic repository for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type that inherits from BaseEntity</typeparam>
    /// <returns>A generic repository for the specified entity type</returns>
    IRepository<T> Repository<T>() where T : BaseEntity;
}