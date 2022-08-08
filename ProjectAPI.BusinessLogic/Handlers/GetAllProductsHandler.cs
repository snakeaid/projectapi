using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using ProjectAPI.DataAccess;
using ProjectAPI.Primitives;
using ProjectAPI.BusinessLogic.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler to get all product and implements
    /// <see cref="IRequestHandler{TRequest, TResponse}"/> for
    /// <see cref="GetAllProductsRequest"/>, <see cref="List{T}"/> of <see cref="ProductModel"/>.
    /// </summary>
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsRequest, List<ProductModel>>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;


        /// <summary>
        /// Constructs an instance of <see cref="GetAllProductsHandler"/> using the specified context, mapper and logger.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="GetAllProductsHandler"/>.</param>
        public GetAllProductsHandler(CatalogContext context, IMapper mapper, ILogger<GetAllProductsHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the specified request to get all products.
        /// </summary>
        /// <param name="request">An instance of <see cref="GetAllProductsRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="List{T}"/> of <see cref="ProductModel"/></returns>
        public async Task<List<ProductModel>> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting all products");
            var list = await _context.Products.ToListAsync();
            var listModel = _mapper.Map<List<ProductModel>>(list);

            _logger.LogInformation($"Got all products sucessfully");

            return listModel;
        }
    }
}