using System.Text.Json.Serialization;
using Application;
using Domain.Enums;
using Infrastructure;

namespace WebApi.Configurations;

public static class Services
{
    public const string UserPolicy = "UserPolicy";
    public const string ApproverPolicy = "ApproverPolicy";

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddApplicationLayer();

        services.AddAuthorizationBuilder()
            .AddPolicy(UserPolicy, policy => policy.RequireRole(nameof(Roles.User)))
            .AddPolicy(ApproverPolicy, policy => policy.RequireRole(nameof(Roles.Approver)));
        
        //Google ის AI გამოვიყენე 
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        // ------ //

        return services;
    }
}
