using System.Text.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using ProjectAPI.Primitives;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.BusinessLogic.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler to update a product and implements
    /// <see cref="IRequestHandler{TRequest, TResponse}"/> for
    /// <see cref="PutProductRequest"/>, <see cref="ProductModel"/>.
    /// </summary>
    public class PutProductHandler : IRequestHandler<PutProductRequest, ProductModel>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IValidator<UpdateProductModel> _validator;

        /// <summary>
        /// Constructs an instance of <see cref="PutProductHandler"/> using the specified context, mapper,
        /// logger and validator.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="PutProductHandler"/>.</param>
        /// <param name="validator">An instance of <see cref="IValidator{T}"/> for <see cref="ProductModel"/>.</param>
        public PutProductHandler(CatalogContext context, IMapper mapper, ILogger<PutProductHandler> logger,
            IValidator<UpdateProductModel> validator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        /// <summary>
        /// Handles the specified request to update a product.
        /// </summary>
        /// <param name="request">An instance of <see cref="PutProductRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ProductModel"/></returns>
        /// <exception cref="ArgumentException">Thrown if the provided model of the product is invalid.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if there is no product found by the specified identifier.</exception>
        public async Task<ProductModel> Handle(PutProductRequest request, CancellationToken cancellationToken)
        {
            UpdateProductModel productModel = request.ProductModel;

            if (!_context.Products.Any(p => p.Id == request.Id))
            {
                throw new KeyNotFoundException($"Product {request.Id} NOT FOUND");
            }

            ValidationResult result = await _validator.ValidateAsync(productModel);
            if (!result.IsValid)
            {
                string errors = JsonSerializer.Serialize(result.ToDictionary());
                throw new ArgumentException(errors);
            }

            if (_context.Categories.FirstOrDefault(c => c.Id == productModel.CategoryId) == null)
            {
                _logger.LogWarning($"Category {productModel.CategoryId} NOT FOUND. Assigning category 1 to the product");
                productModel.CategoryId = 1;
            }

            Product entity = _context.Products.FirstOrDefault(p => p.Id == request.Id);
            entity.Name = productModel.Name;
            entity.Description = productModel.Description;

            if (entity.CategoryId == productModel.CategoryId)
            {
                foreach (KeyValuePair<string, string> keyValuePair in productModel.SpecificationData)
                    if (entity.SpecificationData.ContainsKey(keyValuePair.Key)) entity.SpecificationData[keyValuePair.Key] = keyValuePair.Value;
            }
            else
            {
                entity.CategoryId = productModel.CategoryId;
                Dictionary<string, string> finalSpecifications = _context.Categories.FirstOrDefault(c => c.Id == entity.CategoryId).Specifications.ToDictionary(s => s, s => "");
                foreach (KeyValuePair<string, string> keyValuePair in productModel.SpecificationData)
                    if (finalSpecifications.ContainsKey(keyValuePair.Key)) finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
                entity.SpecificationData = finalSpecifications;
            }
            _context.Products.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Updated product {request.Id} successfully");
            return _mapper.Map<ProductModel>(entity);
        }
    }
}