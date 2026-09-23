using Application.Models;
using Application.TokenServices;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Database;
using Infrastructure.Helper;
using Infrastructure.Repositories.Users;

namespace Application.AuthServices;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterModel model, CancellationToken cancellationToken = default)
    {
        var email = LoweCaseEmail(model.Email);
        var personalId = model.PersonalId.Trim();

        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new InvalidOperationException("User with this email already exists.");

        if (await _userRepository.ExistsByPersonalIdAsync(personalId, cancellationToken))
            throw new InvalidOperationException("User with this personal id already exists.");

        var user = new User
        {
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            PersonalId = personalId,
            BirthDate = model.BirthDate.Date,
            Email = email,
            Password = HashHelper.HashPassword(model.Password),
            Role = Roles.User
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _tokenService.IssueToken(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginModel model, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(LoweCaseEmail(model.Email), cancellationToken);

        if (user is null || !HashHelper.VerifyPassword(model.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return _tokenService.IssueToken(user);
    }

    private static string LoweCaseEmail(string email) => email.Trim().ToLowerInvariant();
}
