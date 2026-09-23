using Domain.Enums;

namespace Application.Models;

public record AuthResponse(string Token, DateTime ExpiresAt, Roles Role);
