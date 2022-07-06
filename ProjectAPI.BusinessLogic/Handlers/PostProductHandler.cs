using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using MediatR;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using ProjectAPI.Primitives;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.BusinessLogic.Requests;
using Microsoft.Extensions.Logging;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler to post a product and implements
    /// <see cref="IRequestHandler{TRequest, TResponse}"/> for
    /// <see cref="PostProductHandler"/>, <see cref="ProductModel"/>.
    /// </summary>
    public class PostProductHandler : IRequestHandler<PostProductRequest, ProductModel>
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
        /// An instance of <see cref="IValidator{T}"/> for <see cref="ProductModel"/>
        /// which is used for model validation.
        /// </summary>
        private readonly IValidator<ProductModel> _validator;

        /// <summary>
        /// Constructs an instance of <see cref="PostProductHandler"/> using the specified context, mapper,
        /// logger and validator.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="PostProductHandler"/>.</param>
        /// <param name="validator">An instance of <see cref="IValidator{T}"/> for <see cref="ProductModel"/>.</param>
        public PostProductHandler(CatalogContext context, IMapper mapper, ILogger<PostProductHandler> logger,
            IValidator<ProductModel> validator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        /// <summary>
        /// Handles the specified request to post a product.
        /// </summary>
        /// <param name="request">An instance of <see cref="PostProductRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ProductModel"/></returns>
        /// <exception cref="ArgumentException">Thrown if the provided model of the product is invalid.</exception>
        public async Task<ProductModel> Handle(PostProductRequest request, CancellationToken cancellationToken)
        {
            ProductModel productModel = request.ProductModel;

            ValidationResult result = await _validator.ValidateAsync(productModel);
            if(!result.IsValid)
            {
                string errors = JsonSerializer.Serialize(result.ToDictionary());
                throw new ArgumentException(errors);
            }

            if (_context.Categories.FirstOrDefault(c => c.Id == productModel.CategoryId) == null)
            {
                _logger.LogWarning($"Category {productModel.CategoryId} NOT FOUND. Assigning category 1 to the product");
                productModel.CategoryId = 1;
            }

            Dictionary<string, string> finalSpecifications = _context.Categories.FirstOrDefault(c => c.Id == productModel.CategoryId).Specifications.ToDictionary(s => s, s => "");
            foreach (KeyValuePair<string, string> keyValuePair in productModel.SpecificationData)
                if (finalSpecifications.ContainsKey(keyValuePair.Key)) finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
            productModel.SpecificationData = finalSpecifications;

            Product product = _mapper.Map<Product>(productModel);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            //string productJson= JsonSerializer.Serialize(product);
            _logger.LogInformation($"Added product successfully");
            return _mapper.Map<ProductModel>(product);
        }
    }
}