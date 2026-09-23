using System.Security.Claims;
using Application.ApplicationServices;
using Application.Models;
using WebApi.Configurations;

namespace WebApi.Endpoints;

public static class ApplicationEndpoints
{
    public static RouteGroupBuilder MapApplicationEndpoints(this RouteGroupBuilder api)
    {
        var applications = api.MapGroup("/applications")
            .WithTags("Applications")
            .RequireAuthorization(Services.UserPolicy)
            .ProducesProblem(StatusCodes.Status404NotFound);

        applications.MapGet("/", async (ClaimsPrincipal user, IApplicationService service, CancellationToken ct) =>
                Results.Ok(await service.GetUserApplicationsAsync(user.FindFirstValue(ClaimTypes.Email)!, ct)))
            .Produces<IReadOnlyList<ApplicationResponse>>();

        applications.MapPost("/", async (AddApplicationModel model, ClaimsPrincipal user,
                    IApplicationService service, CancellationToken ct) =>
                Results.Ok(await service.CreateAsync(user.FindFirstValue(ClaimTypes.Email)!, model, ct)))
            .Produces<ApplicationResponse>();

        applications.MapPut("/{id:int}", async (int id, UpdateApplicationModel model, ClaimsPrincipal user,
                    IApplicationService service, CancellationToken ct) =>
                Results.Ok(await service.UpdateAsync(user.FindFirstValue(ClaimTypes.Email)!, id, model, ct)))
            .Produces<ApplicationResponse>()
            .ProducesProblem(StatusCodes.Status409Conflict);

        applications.MapDelete("/{id:int}", async (int id, ClaimsPrincipal user, IApplicationService service,
                    CancellationToken ct) =>
                {
                    await service.DeleteAsync(user.FindFirstValue(ClaimTypes.Email)!, id, ct);
                    return Results.NoContent();
                })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status409Conflict);

        applications.MapPost("/{id:int}/send", async (int id, ClaimsPrincipal user, IApplicationService service,
                    CancellationToken ct) =>
                Results.Ok(await service.SendAsync(user.FindFirstValue(ClaimTypes.Email)!, id, ct)))
            .Produces<ApplicationResponse>()
            .ProducesProblem(StatusCodes.Status409Conflict);

        return api;
    }
}
