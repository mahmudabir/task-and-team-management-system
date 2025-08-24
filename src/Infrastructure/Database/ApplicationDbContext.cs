using System.Data;

using Domain.Abstractions.Database;
using Domain.Entities.TaskItems;
using Domain.Entities.Teams;
using Domain.Entities.Users;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using Task = System.Threading.Tasks.Task;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
                                         ILogger<ApplicationDbContext> logger)
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options),
      IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Team>().ToTable(Tables.Teams);
        modelBuilder.Entity<TaskItem>().ToTable(Tables.TaskItems);

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Configure all enum properties to be stored as strings
        configurationBuilder
            .Properties<Enum>()
            .HaveConversion<string>();

        base.ConfigureConventions(configurationBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // When should you publish domain events?
        //
        // 1. BEFORE calling SaveChangesAsync
        //     - domain events are part of the same transaction
        //     - immediate consistency
        // 2. AFTER calling SaveChangesAsync
        //     - domain events are a separate transaction
        //     - eventual consistency
        //     - handlers can fail

        int result;
        try
        {
            result = await base.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An error occurred during the transaction.");
            throw;
        }

        return result;
    }

    private IDbContextTransaction? _currentTransaction;

    #region SqlServer

    // For Sql Server
    public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            await operation();
            await CommitTransactionAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An error occurred during the transaction.");
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    // For Sql Server
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await operation();
            await CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An error occurred during the transaction.");
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
            throw new InvalidOperationException("A transaction is already in progress.");

        _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
    }

    private async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
            throw new InvalidOperationException("No transaction in progress.");

        await _currentTransaction.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    private async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
            throw new InvalidOperationException("No transaction in progress.");

        await _currentTransaction.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    #endregion SqlServer

    #region PostgreSql

    // For PostgreSql
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<IDbContextTransaction, Task<TResult>> operation, CancellationToken ct = default)
    {
        IExecutionStrategy strategy = Database.CreateExecutionStrategy();
        TResult response = await strategy.ExecuteAsync(async () =>
        {
            await using IDbContextTransaction transaction = await Database.BeginTransactionAsync(ct);
            try
            {
                var response = await operation(transaction);

                if (transaction.GetDbTransaction() != null! && !IsTransactionCommitted())
                {
                    await transaction.CommitAsync(ct);
                }
                return response;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An error occurred during the transaction.");
                if (transaction.GetDbTransaction() != null! && !IsTransactionCommitted())
                {
                    await transaction.RollbackAsync(ct);
                }

                throw;
            }
        });
        return response;
    }

    // For PostgreSql
    public Task ExecuteInTransactionAsync(Func<IDbContextTransaction, Task> operation, CancellationToken ct = default)
    {
        IExecutionStrategy strategy = Database.CreateExecutionStrategy();
        var task = strategy.ExecuteAsync(async () =>
        {
            await using IDbContextTransaction transaction = await Database.BeginTransactionAsync(ct);
            try
            {
                await operation(transaction);

                if (transaction.GetDbTransaction() != null! && !IsTransactionCommitted())
                {
                    await transaction.CommitAsync(ct);
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An error occurred during the transaction.");
                if (transaction.GetDbTransaction() != null! && !IsTransactionCommitted())
                {
                    await transaction.RollbackAsync(ct);
                }

                throw;
            }
        });

        return task;
    }

    private bool IsTransactionCommitted()
    {
        var connection = Database.GetDbConnection();
        var isCommitted = connection.State == ConnectionState.Closed || Database.CurrentTransaction == null;

        return isCommitted;
    }

    #endregion PostgreSql

}