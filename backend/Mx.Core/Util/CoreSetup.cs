using Mx.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mx.Core.Util;

public static class CoreSetup
{
    public static void ConfigureCore(this IServiceCollection services)
    {
        services.AddSingleton<IClock>(SystemClock.Instance);
        
        services.AddScoped<ITrackService, TrackService>();
        services.AddScoped<IMotorcycleService, MotorcycleService>();
        services.AddScoped<IUserService, UserService>();
    }
}
