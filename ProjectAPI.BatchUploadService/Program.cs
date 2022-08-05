using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
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
                            
                            cfg.ReceiveEndpoint("category-upload-queue", 
                                q => q.Consumer<BatchCategoriesUploadConsumer>());
                        });
                    });

                    services.AddHostedService<Worker>();
                });
    }
}

// services.AddMassTransit(x =>
// {
//     x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
//     {
//         config.Host(new Uri("rabbitmq://localhost"), h =>
//         {
//             h.Username("guest");
//             h.Password("guest");
//         });
//         
//         config.ReceiveEndpoint("category-upload-queue", q => 
//             q.Consumer<BatchCategoriesUploadConsumer>());
//     }));
// });