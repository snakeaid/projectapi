﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using ProjectAPI.Middleware;

namespace ProjectAPI.BusinessLogic.Extensions
{
    /// <summary>
    /// Provides extensions for the application builder.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds custom Swagger to the application.
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns><see cref="IApplicationBuilder"/></returns>
        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder appBuilder, IConfiguration configuration)
        {
            var swaggerEnabled = configuration.GetValue<bool>("EnvironmentOptions:SwaggerEnabled");

            if (swaggerEnabled)
            {
                appBuilder.UseOpenApi();
                appBuilder.UseSwaggerUi3();

                //appBuilder.Use(async (context, next) =>
                //{
                //    string currentUrl = context.Request.Path.Value;
                //    if (currentUrl == "/")
                //    {
                //        context.Response.Redirect("/swagger/index.html", permanent: true);
                //    }

                //    await Task.CompletedTask;
                //});
            }

            return appBuilder;
        }

        /// <summary>
        /// Adds custom exception handling middleware to the application.
        /// </summary>
        /// /// <param name="appBuilder">The application builder.</param>
        /// <returns><see cref="IApplicationBuilder"/></returns>
        public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<ExceptionHandlerMiddleware>();

            return appBuilder;
        }
    }
}