using Zeus.Academia.Infrastructure.Data.Repositories;

namespace Zeus.Academia.Infrastructure.Data;

/// <summary>
/// Unit of Work interface for managing transactions and repository coordination.
/// Task 6: Repository Pattern Implementation - Unit of Work contract.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the academic repository.
    /// </summary>
    IAcademicRepository Academics { get; }

    /// <summary>
    /// Gets the department repository.
    /// </summary>
    IDepartmentRepository Departments { get; }

    /// <summary>
    /// Gets the subject repository.
    /// </summary>
    ISubjectRepository Subjects { get; }

    /// <summary>
    /// Gets a generic repository for the specified entity type.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <returns>Generic repository instance</returns>
    IRepository<T> Repository<T>() where T : class;

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Database transaction</returns>
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of affected records</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes to the database with user information for audit fields.
    /// </summary>
    /// <param name="userId">User identifier for audit</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of affected records</returns>
    Task<int> SaveChangesAsync(string userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Database transaction interface for Unit of Work.
/// </summary>
public interface IDbTransaction : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Commits the transaction.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the transaction.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}