using Domain.Enums;

namespace Application.Models;

public abstract class ApplicationModelBase
{
    public LoanType LoanType { get; set; }
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public int Period { get; set; }
}
