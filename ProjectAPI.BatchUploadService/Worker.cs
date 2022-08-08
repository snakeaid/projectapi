using System.Threading.Tasks;
using MassTransit;
using ProjectAPI.DataAccess.Primitives;

namespace ProjectAPI.BatchUploadService
{
    public class BatchUploadConsumer : IConsumer<UploadRequest>
    {
        public Task Consume(ConsumeContext<UploadRequest> context)
        {
            var request = context.Message;
            
            return Task.CompletedTask;
        }
    }
}