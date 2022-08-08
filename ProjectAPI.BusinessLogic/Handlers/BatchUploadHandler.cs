using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
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
    /// This class represents a MediatR request handler to batch upload categroies or products and implements
    /// <see cref="IRequestHandler{TRequest,TResponse}"/> for
    /// <see cref="PostCategoryRequest"/>, <see cref="CategoryModel"/>.
    /// </summary>
    public class BatchUploadHandler : IRequestHandler<BatchUploadRequest, Guid>
    {
        /// <summary>
        /// An instance of <see cref="CatalogContext"/> which represents the current context.
        /// </summary>
        private readonly CatalogContext _context;

        /// <summary>
        /// An instance of <see cref="ILogger"/> which is used for logging.
        /// </summary>
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

            if (file.Length > 0)
            {
                //TODO CSV HANDLING
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
                var uploadRequest = new UploadRequest { Id = batchId, Type = request.Type, File = fileContents, 
                                                            Status = "Created"};

                await _context.Requests.AddAsync(uploadRequest);
                await _context.SaveChangesAsync();

                var sendEndpoint = await _sendEndpointProvider
                    .GetSendEndpoint(new Uri("queue:batch-upload-queue"));
                await sendEndpoint.Send(uploadRequest);
                
                return batchId;
            }

            return Guid.Empty;
        }
        
        private string CsvToJson(string csvText)
        {
            var csv = new List<string[]>(); 
            //var lines = System.IO.File.ReadAllLines(@"D:\Orders.csv"); // csv file location
            var lines = csvText.Split(Environment.NewLine);
            _logger.Log(LogLevel.Debug, lines.Length.ToString());

            // loop through all lines and add it in list as string
            foreach (string line in lines)
                csv.Add(line.Split(','));

            //split string to get first line, header line as JSON properties
            var properties = lines[0].Split(',');

            var listObjResult = new List<Dictionary<string, string>>();

            //loop all remaining lines, except header so starting it from 1
            // instead of 0
            for (int i = 1; i < lines.Length; i++)
            {
                var objResult = new Dictionary<string, string>();
                for (int j = 0; j < properties.Length; j++)
                    objResult.Add(properties[j], csv[i][j]);

                listObjResult.Add(objResult);
            }

            // convert dictionary into JSON
            var json = JsonSerializer.Serialize(listObjResult);

            return json;
        }
    }
}