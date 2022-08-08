using MassTransit;
using Microsoft.Extensions.Hosting;

namespace ProjectAPI.BatchUploadService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        // elided...

                        x.UsingRabbitMq((context,cfg) =>
                        {
                            cfg.Host("localhost", "/", h => {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            cfg.ConfigureEndpoints(context);
                            
                            cfg.ReceiveEndpoint("batch-upload-queue", 
                                q => q.Consumer<BatchUploadConsumer>());
                        });
                    });
                });
    }
}