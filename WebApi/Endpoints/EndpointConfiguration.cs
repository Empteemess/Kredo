namespace WebApi.Endpoints;

public static class EndpointConfiguration
{
    public static WebApplication AddEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("/api");

        api.MapAuthEndpoints();
        api.MapApplicationEndpoints();

        return app;
    }
}
