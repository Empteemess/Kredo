using Application.Models;
using Domain.Entities;

namespace Application.TokenServices;

public interface ITokenService
{
    AuthResponse IssueToken(User user);
}
