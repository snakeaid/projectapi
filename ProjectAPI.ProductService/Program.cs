using System.IO;
using System.Reflection;
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectAPI.BusinessLogic.Extensions;
using ProjectAPI.Mapping;
using ProjectAPI.ModelValidation;

namespace ProjectAPI.ProductService
{
    public class Program
    {
        public static IConfiguration Configuration => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddCatalogContext(Configuration);

                    services.AddLoggingToFile();

                    services.AddAutoMapper(typeof(AllMappersProfile));

                    services.AddValidatorsFromAssemblyContaining<CreateProductModelValidator>();

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumers(Assembly.GetExecutingAssembly());

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("localhost", "/", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            cfg.ConfigureEndpoints(context);
                        });
                    });
                });
    }
}