using System;
using System.Linq;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using NSwag.Generation.Processors.Security;
using ProjectAPI.DataAccess;
using ProjectAPI.Mapping;
using ProjectAPI.ModelValidation;

namespace ProjectAPI.BusinessLogic.Extensions
{
    /// <summary>
    /// Provides extensions for the services collection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Swagger to the services collection.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            var swaggerEnabled = configuration.GetValue<bool>("EnvironmentOptions:SwaggerEnabled");

            if (swaggerEnabled)
            {
                services.AddOpenApiDocument(document =>
                {
                    document.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Type into the textbox: Bearer {your JWT token}."
                    });

                    document.OperationProcessors.Add(
                        new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                });
            }

            return services;
        }

        /// <summary>
        /// Adds database context to the services collection.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddCatalogContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CatalogContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        /// <summary>
        /// Disables automatic validation.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection DisableAutomaticValidation(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            return services;
        }
        
        /// <summary>
        /// Adds FluentValidation.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateCategoryModelValidator>();

            return services;
        }
        
        /// <summary>
        /// Adds AutoMapper.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddMapping(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AllMappersProfile));

            return services;
        }

        /// <summary>
        /// Adds logging to file.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddLoggingToFile(this IServiceCollection services)
        {
            services.AddLogging(loggingBuilder => {
                loggingBuilder.AddFile("Logs/app_{0:yyyy}-{0:MM}-{0:dd}.log", fileLoggerOpts => {
                    fileLoggerOpts.FormatLogFileName = fName => {
                        return String.Format(fName, DateTime.UtcNow);
                    };
                });
            });

            return services;
        }
    }
}