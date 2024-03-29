﻿using System.Collections.Generic;
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
    /// This class represents a MediatR request handler to get a product and implements
    /// <see cref="IRequestHandler{TRequest,TResponse}"/> for
    /// <see cref="GetProductRequest"/>, <see cref="ProductModel"/>.
    /// </summary>
    public class GetProductHandler : IRequestHandler<GetProductRequest, ProductModel>
    {
        private readonly CatalogContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructs an instance of <see cref="GetProductHandler"/> using the specified context, mapper and logger.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="GetProductHandler"/>.</param>
        public GetProductHandler(CatalogContext context, IMapper mapper, ILogger<GetProductHandler> logger)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the specified request to get a product.
        /// </summary>
        /// <param name="request">An instance of <see cref="GetProductRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="CategoryModel"/></returns>
        /// <exception cref="KeyNotFoundException">Thrown if there is no product found by the specified identifier.</exception>
        public async Task<ProductModel> Handle(GetProductRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting product {request.Id}");
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product {request.Id} NOT FOUND");
            }

            ProductModel productModel = _mapper.Map<ProductModel>(product);

            _logger.LogInformation($"Got product {request.Id} successfully");
            return productModel;
        }
    }
}