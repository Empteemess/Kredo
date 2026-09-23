using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class UnitOfWork : IUnitOfWork
{
    private readonly KredoDb _context;

    public UnitOfWork(KredoDb context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        if (!_context.Database.IsRelational())
        {
            await operation(cancellationToken);
            return;
        }

        // Disposing without commit rolls the transaction back.
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        await operation(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
