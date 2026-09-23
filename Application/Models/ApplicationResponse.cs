using Domain.Entities;
using Domain.Enums;

namespace Application.Models;

public record ApplicationResponse(
    int Id,
    int UserId,
    string? ApplicantFullName,
    LoanType LoanType,
    decimal Amount,
    string Currency,
    int Period,
    Status Status,
    bool CanBeModified,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? SentAt,
    DateTime? ClosedAt)
{
    public static ApplicationResponse From(LoanApplication application) => new(
        application.Id,
        application.UserId,
        application.User is null ? null : $"{application.User.FirstName} {application.User.LastName}",
        application.LoanType,
        application.Amount,
        application.Currency,
        application.Period,
        application.Status,
        application.CanBeModified,
        application.CreatedAt,
        application.UpdatedAt,
        application.SentAt,
        application.ClosedAt);
}
