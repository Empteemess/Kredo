using Application.AuthServices;
using Application.Models;

namespace WebApi.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder api)
    {
        var auth = api.MapGroup("/auth").WithTags("Auth").AllowAnonymous();

        auth.MapPost("/register", async (RegisterModel model, IAuthService authService, CancellationToken ct) =>
                Results.Ok(await authService.RegisterAsync(model, ct)))
            .Produces<AuthResponse>()
            .ProducesProblem(StatusCodes.Status409Conflict);

        auth.MapPost("/login", async (LoginModel model, IAuthService authService, CancellationToken ct) =>
                Results.Ok(await authService.LoginAsync(model, ct)))
            .Produces<AuthResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return api;
    }
}
