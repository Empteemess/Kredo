using Domain.Enums;

namespace Domain.Entities;

public class User
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PersonalId { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required DateTime BirthDate { get; set; }
    public Roles Role { get; set; }

    public ICollection<LoanApplication> Applications { get; set; } = new List<LoanApplication>();
}
