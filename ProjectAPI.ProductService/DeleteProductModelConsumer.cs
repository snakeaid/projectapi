using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectAPI.DataAccess;
using ProjectAPI.Primitives;

namespace ProjectAPI.ProductService
{
    /// <summary>
    /// This class represents a MassTransit definition class which defines the behaviour
    /// of <see cref="DeleteProductModelConsumer"/> and implements <see cref="ConsumerDefinition{TConsumer}"/>
    /// for <see cref="DeleteProductModelConsumer"/>.
    /// </summary>
    public class DeleteProductModelConsumerDefinition : ConsumerDefinition<DeleteProductModelConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<DeleteProductModelConsumer> consumerConfigurator)
        {
            endpointConfigurator.DiscardFaultedMessages();
        }
    }

    /// <summary>
    /// This class represents a MassTransit consumer class which consumes messages to delete a product and implements
    /// <see cref="IConsumer{TMessage}"/> for
    /// <see cref="DeleteProductModel"/>.
    /// </summary>
    public class DeleteProductModelConsumer : IConsumer<DeleteProductModel>
    {
        private readonly CatalogContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructs an instance of <see cref="DeleteProductModelConsumer"/> using the specified context, mapper and
        /// logger.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="DeleteProductModelConsumer"/>.</param>
        public DeleteProductModelConsumer(CatalogContext context, IMapper mapper,
            ILogger<DeleteProductModelConsumer> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Consumes the message to delete a product.
        /// </summary>
        /// <param name="context">An instance of <see cref="ConsumeContext{T}"/> for <see cref="DeleteProductModel"/>.</param>
        /// /// <exception cref="KeyNotFoundException">Thrown if there is no product found by the specified identifier.</exception>
        public async Task Consume(ConsumeContext<DeleteProductModel> context)
        {
            var productModel = context.Message;
            var product = _context.Products.FirstOrDefault(p => p.Id == productModel.Id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product {productModel.Id} NOT FOUND");
            }

            product.DateDeleted = DateTimeOffset.UtcNow;
            _context.Products.Attach(product);
            _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Deleted product {productModel.Id} successfully");

            await context.RespondAsync(_mapper.Map<ProductModel>(product));
        }
    }
}