using Mx.Core;
using Mx.Core.Util;
using Mx.Hubs;
using Mx.Persistence;
using Mx.Persistence.Util;
using Mx.RPCServices;
using Mx.Util;
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
        
        // SignalR Hubs hinzufügen
        services.AddSignalR();
        
        // gRPC Services hinzufügen
        services.AddGrpc();
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
    
    public static void ConfigureEndpoints(this WebApplication app)
    {
        // SignalR Hub-Endpunkte konfigurieren
        app.MapHub<TrackerHub>("/hubs/tracker");
        
        // gRPC Services konfigurieren
        app.MapGrpcService<TrackerService>();
    }
}
