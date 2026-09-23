using Application.Models;

namespace Application.ApplicationServices;

/// <summary>
/// Loan application operations for the application owner.
/// </summary>
public interface IApplicationService
{
    Task<IReadOnlyList<ApplicationResponse>> GetUserApplicationsAsync(string email, CancellationToken cancellationToken = default);
    Task<ApplicationResponse> CreateAsync(string email, AddApplicationModel model, CancellationToken cancellationToken = default);
    Task<ApplicationResponse> UpdateAsync(string email, int applicationId, UpdateApplicationModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(string email, int applicationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends the application for review (status changes to <see cref="Domain.Enums.Status.Sent"/>).
    /// </summary>
    Task<ApplicationResponse> SendAsync(string email, int applicationId, CancellationToken cancellationToken = default);
}
