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
    public class PostProductHandler : IRequestHandler<PostProductRequest, ProductModel>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IValidator<ProductModel> _validator;

        public PostProductHandler(CatalogContext context, IMapper mapper, ILogger<PostProductHandler> logger,
            IValidator<ProductModel> validator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

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