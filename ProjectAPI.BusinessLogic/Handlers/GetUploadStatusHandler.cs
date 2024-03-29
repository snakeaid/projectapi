using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.DataAccess;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler to get a category and implements
    /// <see cref="IRequestHandler{TRequest,TResponse}"/> for
    /// <see cref="GetUploadStatusRequest"/>, <see cref="UploadRequestModel"/>.
    /// </summary>
    public class GetUploadStatusHandler : IRequestHandler<GetUploadStatusRequest, UploadRequestModel>
    {
        private readonly CatalogContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructs an instance of <see cref="GetUploadStatusHandler"/> using the specified context, mapper and logger.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="GetUploadStatusHandler"/>.</param>
        public GetUploadStatusHandler(CatalogContext context, IMapper mapper, ILogger<GetUploadStatusHandler> logger)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the specified request to get a request status.
        /// </summary>
        /// <param name="request">An instance of <see cref="GetUploadStatusRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="UploadRequestModel"/></returns>
        /// <exception cref="KeyNotFoundException">Thrown if there is no request found by the specified identifier.</exception>
        public async Task<UploadRequestModel> Handle(GetUploadStatusRequest request,
            CancellationToken cancellationToken)
        {
            var uploadRequest = await _context.Requests.FirstOrDefaultAsync(r => r.Id == request.Id);
            if (uploadRequest == null)
            {
                throw new KeyNotFoundException($"Request {request.Id} NOT FOUND");
            }

            var requestModel = _mapper.Map<UploadRequestModel>(uploadRequest);
            _logger.Log(LogLevel.Information, $"Got upload status for request {request.Id} successfully.");
            return requestModel;
        }
    }
}