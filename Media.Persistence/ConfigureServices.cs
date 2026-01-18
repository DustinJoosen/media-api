using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Media.Persistence
{
	public static class ConfigureServices
	{
		public static IServiceCollection ConfigurePersistence(this IServiceCollection services, 
			IConfiguration configuration)
		{
			services.AddDbContext<MediaDbContext>(options =>
			{
				var connString = configuration.GetConnectionString("DefaultConnection");
				options.UseMySql(connString, ServerVersion.AutoDetect(connString));
			});

			return services;
		}
	}
}
