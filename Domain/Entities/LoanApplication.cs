using Domain.Enums;

namespace Domain.Entities;

public class LoanApplication
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public LoanType LoanType { get; set; }
    public decimal Amount { get; set; }
    public required string Currency { get; set; }

    /// <summary>
    /// Loan period in months.
    /// </summary>
    public int Period { get; set; }

    public Status Status { get; set; } = Status.InProcess;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// Only applications that were not sent yet may be edited, deleted or sent.
    /// </summary>
    public bool CanBeModified => Status == Status.InProcess;

    public static LoanApplication Create(int userId, LoanType loanType, decimal amount, string currency, int period,
        DateTime now)
    {
        return new LoanApplication
        {
            UserId = userId,
            LoanType = loanType,
            Amount = amount,
            Currency = currency,
            Period = period,
            Status = Status.InProcess,
            CreatedAt = now
        };
    }

    public void Update(LoanType loanType, decimal amount, string currency, int period, DateTime now)
    {
        EnsureModifiable("edited");

        LoanType = loanType;
        Amount = amount;
        Currency = currency;
        Period = period;
        UpdatedAt = now;
    }

    public void EnsureCanBeDeleted() => EnsureModifiable("deleted");

    public void Send(DateTime now)
    {
        EnsureModifiable("sent");

        Status = Status.Sent;
        SentAt = now;
        UpdatedAt = now;
    }

    public void Approve(DateTime now) => Close(Status.Approved, now);

    public void Reject(DateTime now) => Close(Status.Rejected, now);

    private void Close(Status status, DateTime now)
    {
        if (Status != Status.Sent)
            throw new ApplicationException($"Only applications with status '{Status.Sent}' can be reviewed. Current status: '{Status}'.");

        Status = status;
        ClosedAt = now;
        UpdatedAt = now;
    }

    private void EnsureModifiable(string action)
    {
        if (!CanBeModified)
            throw new ApplicationException($"Application with status '{Status}' can not be {action}.");
    }
}
