using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler to batch upload categories or products implements
    /// <see cref="IRequestHandler{TRequest,TResponse}"/> for
    /// <see cref="PostCategoryRequest"/>, <see cref="CategoryModel"/>.
    /// </summary>
    public class BatchUploadHandler : IRequestHandler<BatchUploadRequest, Guid>
    {
        private readonly CatalogContext _context;
        private readonly ILogger _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        /// <summary>
        /// Constructs an instance of <see cref="PostCategoryHandler"/> using the specified context, mapper,logger and validator.
        /// </summary>
        /// /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{BatchUploadHandler}"/>
        /// for <see cref="BatchUploadHandler"/>.</param>
        /// <param name="sendEndpointProvider">An instance of <see cref="ISendEndpointProvider"/></param>
        public BatchUploadHandler(CatalogContext context, ILogger<BatchUploadHandler> logger,
            ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _context = context;
        }

        /// <summary>
        /// Handles the specified request to make batch upload.
        /// </summary>
        /// <param name="request">An instance of <see cref="BatchUploadRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="Guid"/></returns>
        public async Task<Guid> Handle(BatchUploadRequest request, CancellationToken cancellationToken)
        {
            var file = request.File;

            if (file.Length <= 0)
            {
                throw new ArgumentException("File must not be empty.");
            }

            var fullFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileType = fullFileName.Split('.').Last();

            string fileContents;

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var fileBytes = ms.ToArray();
                fileContents = Convert.ToBase64String(fileBytes);
            }

            var batchId = Guid.NewGuid();
            var uploadRequest = new UploadRequest
            {
                Id = batchId, Type = request.Type, File = fileContents,
                FileType = fileType, Status = "Uploaded the file"
            };
            
            _logger.Log(LogLevel.Information, $"Uploaded the file {fullFileName} successfully.");

            await _context.Requests.AddAsync(uploadRequest);
            await _context.SaveChangesAsync();

            ISendEndpoint sendEndpoint;

            switch (request.Type)
            {
                case CatalogEntityType.Product:
                    sendEndpoint = await _sendEndpointProvider
                        .GetSendEndpoint(new Uri("queue:products-upload-queue"));
                    break;
                case CatalogEntityType.Category:
                    sendEndpoint = await _sendEndpointProvider
                        .GetSendEndpoint(new Uri("queue:categories-upload-queue"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await sendEndpoint.Send(uploadRequest);

            _logger.Log(LogLevel.Information, $"Sent request {batchId} successfully.");

            return batchId;
        }
    }
}