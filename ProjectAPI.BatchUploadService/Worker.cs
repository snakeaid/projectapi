using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectAPI.BusinessLogic;
using ProjectAPI.Primitives;

namespace ProjectAPI.BatchUploadService
{
    public class BatchCategoriesUploadConsumer : IConsumer<BatchCategoriesUpload>
    {
        public Task Consume(ConsumeContext<BatchCategoriesUpload> context)
        {

            return Task.CompletedTask;
        }
    }
    
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}