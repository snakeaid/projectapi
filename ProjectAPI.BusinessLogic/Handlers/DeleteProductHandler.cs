using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using AutoMapper;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Primitives;
using ProjectAPI.BusinessLogic.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler for product deletion and implements
    /// <see cref="IRequestHandler{TRequest, TResponse}"/> for
    /// <see cref="Delete"/>, <see cref="ProductModel"/>.
    /// </summary>
    public class DeleteProductHandler : IRequestHandler<DeleteProductRequest, ProductModel>
    {
        /// <summary>
        /// An instance of <see cref="CatalogContext"/> which represents the current context.
        /// </summary>
        private readonly CatalogContext _context;

        /// <summary>
        /// An instance of <see cref="IMapper"/> which is used for mapping.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// An instance of <see cref="ILogger"/> which is used for logging.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// Constructs an instance of <see cref="DeleteProductHandler"/> using the specified context, mapper and logger.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="DeleteProductHandler"/>.</param>
        public DeleteProductHandler(CatalogContext context, IMapper mapper, ILogger<DeleteProductHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the specified request for product deletion.
        /// </summary>
        /// <param name="request">An instance of <see cref="DeleteProductRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="CategoryModel"/></returns>
        /// <exception cref="KeyNotFoundException">Thrown if there is no category found by the specified identifier.</exception>
        public async Task<ProductModel> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
        {
            Product product = _context.Products.FirstOrDefault(p => p.Id == request.Id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product {request.Id} NOT FOUND");
            }

            product.DateDeleted = DateTimeOffset.UtcNow;
            _context.Products.Attach(product);
            _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Deleted product {request.Id} successfully");
            return _mapper.Map<ProductModel>(product);
        }
    }
}