using Domain.Entities;

namespace Infrastructure.Repositories.Applications;

public interface IApplicationRepository
{
    Task<LoanApplication?> GetByIdAsync(int applicationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LoanApplication>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task AddAsync(LoanApplication application, CancellationToken cancellationToken = default);
    void Remove(LoanApplication application);
}
