using Application;

using Domain;

using Infrastructure;

using Shared;
using Shared.Settings;

using Softoverse.CqrsKit.Extensions;

using WebApi.Infrastructure.Extensions;
using WebApi.Infrastructure.Middlewares;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<AdminSettings>(configuration.GetSection(AdminSettings.SectionName));
        services.Configure<ManagerSettings>(configuration.GetSection(ManagerSettings.SectionName));
        services.Configure<EmployeeSettings>(configuration.GetSection(EmployeeSettings.SectionName));

        services.AddOpenApi();
        services.AddEndpointsApiExplorer();

        services.AddSwagger(configuration)
                .AddOpenApiAuthentication();

        services.AddApiControllers()
                .AddApiControllers()
                .AddJsonOptions() ;

        // Shows UseCors with CorsPolicyBuilder.
        services.AddCors(options =>
                             options.AddPolicy("CorsPolicy", builder =>
                             {
                                 builder.AllowAnyOrigin()
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .SetIsOriginAllowed((host) => true)
                                        .AllowCredentials();

                                 builder.WithOrigins("*")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .SetIsOriginAllowed((host) => true)
                                        .AllowCredentials();
                             })
                        );

        services.AddCqrsKit(config =>
        {
            config.RegisterServicesFromAssemblyContaining<IDomainMarker>();
            config.RegisterServicesFromAssemblyContaining<ISharedMarker>();
            config.RegisterServicesFromAssemblyContaining<IInfrastructureMarker>();
            config.RegisterServicesFromAssemblyContaining<IApplicationMarker>();
            config.RegisterServicesFromAssemblyContaining<IWebApiMarker>();
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}