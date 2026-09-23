using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Applications;

public class ApplicationRepository : IApplicationRepository
{
    private readonly KredoDb _context;

    public ApplicationRepository(KredoDb context)
    {
        _context = context;
    }

    public Task<LoanApplication?> GetByIdAsync(int applicationId, CancellationToken cancellationToken = default)
    {
        return _context.Applications
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);
    }

    public async Task<IReadOnlyList<LoanApplication>> GetByUserIdAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Applications
            .AsNoTracking()
            .Include(a => a.User)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }
    public async Task AddAsync(LoanApplication application, CancellationToken cancellationToken = default)
    {
        await _context.Applications.AddAsync(application, cancellationToken);
    }

    public void Remove(LoanApplication application)
    {
        _context.Applications.Remove(application);
    }
}