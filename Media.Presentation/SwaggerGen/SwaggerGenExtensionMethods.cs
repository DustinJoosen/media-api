using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Media.Presentation.SwaggerGen
{
    /// <summary>
    /// Add all swagger information to the interface.
    /// </summary>
    public static class SwaggerGenConfig
    {

        /// <summary>
        /// Add configuration for the swagger interface.
        /// </summary>
        /// <returns>The servicecollection self, to allow method-chaining</returns>
        public static IServiceCollection AddSwaggerGeneration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Media API",
                    Description = "An API to manage file uploads and retrieval",
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter authtoken",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }       
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            return services;
        }
    }
}