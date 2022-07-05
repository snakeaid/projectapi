using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ProjectAPI.BusinessLogic.Extensions
{
    public static class ApplicationBuilderExtensions
    {
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
    }

}

