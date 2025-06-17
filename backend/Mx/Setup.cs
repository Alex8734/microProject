using Mx.Core;
using Mx.Core.Util;
using Mx.Persistence;
using Mx.Persistence.Util;
using Mx.Util;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;

namespace Mx;

public static class Setup
{
    public const string CorsPolicyName = "DefaultCorsPolicy";

    public static void AddApplicationServices(this IServiceCollection services,
                                              IConfigurationManager configurationManager,
                                              bool isDev)
    {
        services.ConfigurePersistence(configurationManager, isDev);
        services.ConfigureCore();
    }

    public static Settings LoadAndConfigureSettings(this IServiceCollection services, IConfigurationManager configurationManager)
    {
        var configSection = configurationManager.GetSection(Settings.SectionKey);

        services.Configure<Settings>(s => configSection.Bind(s));

        // different instance, but the same values - used for startup config outside of DI context
        var settings = Activator.CreateInstance<Settings>();
        configSection.Bind(settings);

        return settings;
    }

    public static void AddLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Host.UseSerilog((_, _, config) =>
        {
            config
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        });
    }

    public static void AddCors(this IServiceCollection services, Settings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.ClientOrigin))
        {
            throw new InvalidOperationException("Client origin has to be configured");
        }

        services.AddCors(o => o.AddPolicy(CorsPolicyName, builder =>
        {
            builder.WithOrigins(settings.ClientOrigin)
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        }));

        Log.Logger.Debug("Added CORS policy with client origin {ClientOrigin}", settings.ClientOrigin);
    }

    public static void ConfigureAdditionalRouteConstraints(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options =>
        {
            options.ConstraintMap.Add(nameof(LocalDate), typeof(LocalDateRouteConstraint));
        });
    }

    public static void AddOApi(this IServiceCollection services)
    {
        // Register the NSwag services
        services.AddSwaggerDocument(config =>
        {
            config.Title = "Mx API";
            config.Description = "API Documentation for Mx Application";
            config.Version = "v1";

            // Add JWT authentication support to Swagger UI
            config.AddSecurity("JWT", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Enter 'Bearer {token}' to authenticate"
            });

            config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
        });
    }

    public static void MapOpenApi(this WebApplication app)
    {
        // Register the Swagger generator and the Swagger UI middleware
        app.UseOpenApi(); // Serves the OpenAPI/Swagger documents
        app.UseSwaggerUi(); // Serves the Swagger UI
        
    }
}
