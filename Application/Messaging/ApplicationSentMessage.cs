using Domain.Entities;
using Domain.Enums;

namespace Application.Messaging;

public record ApplicationSentMessage(
    int ApplicationId,
    int UserId,
    LoanType LoanType,
    decimal Amount,
    string Currency,
    int Period,
    DateTime SentAt)
{
    public static ApplicationSentMessage From(LoanApplication application) => new(
        application.Id,
        application.UserId,
        application.LoanType,
        application.Amount,
        application.Currency,
        application.Period,
        application.SentAt ?? DateTime.UtcNow);
}
