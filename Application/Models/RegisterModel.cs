namespace Application.Models;

public class RegisterModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PersonalId { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Password { get; set; } = string.Empty;
}
