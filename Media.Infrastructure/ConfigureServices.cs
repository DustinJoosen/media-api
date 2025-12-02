using Media.Abstractions.Interfaces;
using Media.Infrastructure.Interfaces;
using Media.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Media.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthTokenService, AuthTokenService>();
            services.AddScoped<IMediaItemService, MediaItemService>();

#if DEBUG
            services.AddScoped<IUploadService, DebugUploadService>();
#else
            services.AddScoped<IUploadService, ReleaseUploadService>();
#endif

            return services;
        }
    }
}
