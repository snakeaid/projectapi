using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ProjectAPI
{
    /// <summary>
    /// This class is the entry point of the application and is responsible for registering <see cref="Startup"/>
    /// and create a host.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Builds the host and runs the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates a host builder.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns><see cref="IHostBuilder"/></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(services =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.UsingRabbitMq();
                    });
                });
    }
}