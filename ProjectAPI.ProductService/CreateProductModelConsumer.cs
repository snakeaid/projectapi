using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Primitives;

namespace ProjectAPI.ProductService
{
    public class CreateProductModelConsumerDefinition : ConsumerDefinition<CreateProductModelConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<CreateProductModelConsumer> consumerConfigurator)
        {
            endpointConfigurator.DiscardFaultedMessages();
        }
    }

    public class CreateProductModelConsumer : IConsumer<CreateProductModel>
    {
        private readonly CatalogContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateProductModel> _validator;

        /// <summary>
        /// Constructs an instance of <see cref="CreateProductModelConsumer"/> using the specified context, mapper,
        /// logger and validator.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="CreateProductModelConsumer"/>.</param>
        /// <param name="validator">An instance of <see cref="IValidator{T}"/> for <see cref="CreateProductModel"/>.</param>
        public CreateProductModelConsumer(CatalogContext context, IMapper mapper,
            ILogger<CreateProductModelConsumer> logger, IValidator<CreateProductModel> validator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        public async Task Consume(ConsumeContext<CreateProductModel> context)
        {
            var productModel = context.Message;

            var result = await _validator.ValidateAsync(productModel);
            if (!result.IsValid)
            {
                string errors = JsonSerializer.Serialize(result.ToDictionary());
                throw new ArgumentException(errors);
            }

            if (await _context.Categories.FirstOrDefaultAsync(c => c.Id == productModel.CategoryId) == null)
            {
                _logger.LogWarning(
                    $"Category {productModel.CategoryId} NOT FOUND. Assigning category 1 to the product");
                productModel.CategoryId = 1;
            }

            Dictionary<string, string> finalSpecifications = (await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == productModel.CategoryId))
                .Specifications.ToDictionary(s => s, s => "");
            foreach (KeyValuePair<string, string> keyValuePair in productModel.SpecificationData)
                if (finalSpecifications.ContainsKey(keyValuePair.Key))
                    finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
            productModel.SpecificationData = finalSpecifications;

            Product product = _mapper.Map<Product>(productModel);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Added product successfully");

            await context.RespondAsync(_mapper.Map<ProductModel>(product));
        }
    }
}