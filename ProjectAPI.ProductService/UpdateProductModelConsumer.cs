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
using ProjectAPI.Primitives;

namespace ProjectAPI.ProductService
{
    /// <summary>
    /// This class represents a MassTransit definition class which defines the behaviour
    /// of <see cref="UpdateProductModelConsumer"/> and implements <see cref="ConsumerDefinition{TConsumer}"/>
    /// for <see cref="UpdateProductModelConsumer"/>.
    /// </summary>
    public class UpdateProductModelConsumerDefinition : ConsumerDefinition<UpdateProductModelConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<UpdateProductModelConsumer> consumerConfigurator)
        {
            endpointConfigurator.DiscardFaultedMessages();
        }
    }

    /// <summary>
    /// This class represents a MassTransit consumer class which consumes messages to update a product and implements
    /// <see cref="IConsumer{TMessage}"/> for
    /// <see cref="UpdateProductModel"/>.
    /// </summary>
    public class UpdateProductModelConsumer : IConsumer<UpdateProductModel>
    {
        private readonly CatalogContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateProductModel> _validator;

        /// <summary>
        /// Constructs an instance of <see cref="UpdateProductModelConsumer"/> using the specified context, mapper,
        /// logger and validator.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="UpdateProductModelConsumer"/>.</param>
        /// <param name="validator">An instance of <see cref="IValidator{T}"/> for <see cref="UpdateProductModel"/>.</param>
        public UpdateProductModelConsumer(CatalogContext context, IMapper mapper,
            ILogger<UpdateProductModelConsumer> logger, IValidator<UpdateProductModel> validator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        /// <summary>
        /// Consumes the message to update a product.
        /// </summary>
        /// <param name="context">An instance of <see cref="ConsumeContext{T}"/> for <see cref="UpdateProductModel"/>.</param>
        /// <exception cref="KeyNotFoundException">Thrown if there is no product found by the specified identifier.</exception>
        /// <exception cref="ArgumentException">Thrown if the provided model of the product is invalid.</exception>
        public async Task Consume(ConsumeContext<UpdateProductModel> context)
        {
            var productModel = context.Message;
            if (!_context.Products.Any(p => p.Id == productModel.Id))
            {
                throw new KeyNotFoundException($"Product {productModel.Id} NOT FOUND");
            }

            var result = await _validator.ValidateAsync(productModel);
            if (!result.IsValid)
            {
                string errors = JsonSerializer.Serialize(result.ToDictionary());
                throw new ArgumentException(errors);
            }

            if (_context.Categories.FirstOrDefault(c => c.Id == productModel.CategoryId) == null)
            {
                _logger.LogWarning(
                    $"Category {productModel.CategoryId} NOT FOUND. Assigning category 1 to the product");
                productModel.CategoryId = 1;
            }

            //TODO: use mapping?
            var product = _context.Products.FirstOrDefault(p => p.Id == productModel.Id);
            product.Name = productModel.Name;
            product.Description = productModel.Description;

            if (product.CategoryId == productModel.CategoryId)
            {
                foreach (KeyValuePair<string, string> keyValuePair in productModel.SpecificationData)
                    if (product.SpecificationData.ContainsKey(keyValuePair.Key))
                        product.SpecificationData[keyValuePair.Key] = keyValuePair.Value;
            }
            else
            {
                product.CategoryId = productModel.CategoryId;
                Dictionary<string, string> finalSpecifications = _context.Categories
                    .FirstOrDefault(c => c.Id == product.CategoryId).Specifications.ToDictionary(s => s, s => "");
                foreach (KeyValuePair<string, string> keyValuePair in productModel.SpecificationData)
                    if (finalSpecifications.ContainsKey(keyValuePair.Key))
                        finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
                product.SpecificationData = finalSpecifications;
            }

            _context.Products.Attach(product);
            _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Updated product {productModel.Id} successfully");

            await context.RespondAsync(_mapper.Map<ProductModel>(product));
        }
    }
}