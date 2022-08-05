using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text.Json;
using MediatR;
using AutoMapper;
using FluentValidation;
using MassTransit;
using ProjectAPI.Primitives;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.BusinessLogic.Requests;
using Microsoft.Extensions.Logging;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler to post a category and implements
    /// <see cref="IRequestHandler{TRequest, TResponse}"/> for
    /// <see cref="PostCategoryRequest"/>, <see cref="CategoryModel"/>.
    /// </summary>
    public class BatchUploadCategoriesHandler : IRequestHandler<BatchUploadCategoriesRequest, Guid>
    {
        /// <summary>
        /// An instance of <see cref="IMapper"/> which is used for mapping.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// An instance of <see cref="ILogger"/> which is used for logging.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// An instance of <see cref="IValidator{T}"/> for <see cref="CategoryModel"/>
        /// which is used for model validation.
        /// </summary>
        private readonly IValidator<CreateCategoryModel> _validator;

        private readonly ISendEndpointProvider _sendEndpointProvider;

        /// <summary>
        /// Constructs an instance of <see cref="PostCategoryHandler"/> using the specified context, mapper,
        /// logger and validator.
        /// </summary>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="PostCategoryHandler"/>.</param>
        /// <param name="validator">An instance of <see cref="IValidator{T}"/> for <see cref="CategoryModel"/>.</param>
        public BatchUploadCategoriesHandler(IMapper mapper, ILogger<PostCategoryHandler> logger,
            IValidator<CreateCategoryModel> validator, ISendEndpointProvider sendEndpointProvider)
        {
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
            _sendEndpointProvider = sendEndpointProvider;
        }

        /// <summary>
        /// Handles the specified request to post a category.
        /// </summary>
        /// <param name="request">An instance of <see cref="PostCategoryRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="CategoryModel"/></returns>
        /// <exception cref="ArgumentException">Thrown if the provided model of the category is invalid.</exception>
        public async Task<Guid> Handle(BatchUploadCategoriesRequest request, CancellationToken cancellationToken)
        {
            var file = request.File;

            if (file.Length > 0)
            {
                var fullFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fileType = fullFileName.Split('.').Last();
                string fileContents;
                using (var r = new StreamReader(file.OpenReadStream()))
                {
                    fileContents = await r.ReadToEndAsync();
                }

                _logger.Log(LogLevel.Debug, fileContents);

                string jsonContents;

                switch (fileType)
                {
                    case "json":
                        jsonContents = fileContents;
                        break;
                    case "csv":
                        jsonContents = CsvToJson(fileContents);
                        break;
                    default:
                        throw new ArgumentException("File type must be either json or csv.");
                }

                var batchId = Guid.NewGuid();
                
                var categoriesDTO = JsonSerializer.Deserialize<List<CreateCategoryModel>>(jsonContents);
                var categories = _mapper.Map<List<Category>>(categoriesDTO);

                var sendEndpoint = await _sendEndpointProvider
                    .GetSendEndpoint(new Uri("queue:category-upload-queue"));
                await sendEndpoint.Send(new BatchCategoriesUpload
                    { Id = batchId, Categories = categories});
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