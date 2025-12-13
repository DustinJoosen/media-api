using Media.Abstractions.Interfaces;
using Media.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Media.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthTokenService, AuthTokenService>();
            services.AddScoped<IMediaItemService, MediaItemService>();

#if DEBUG
            services.AddScoped<IFileService, DebugFileService>();
#else
            services.AddScoped<IFileService, ReleaseFileService>();
#endif

            return services;
        }
    }
}
