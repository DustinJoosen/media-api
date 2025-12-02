using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Media.Persistence
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MediaDbContext>(options =>
            {
                string connectionString = configuration.GetConnectionString("DefaultConnection")!;

                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                options.EnableDetailedErrors();
            });

            return services;
        }
    }
}
