using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectAPI.DataAccess;
using ProjectAPI.Primitives;

namespace ProjectAPI.CategoryService
{
    public class DeleteCategoryModelConsumerDefinition : ConsumerDefinition<DeleteCategoryModelConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<DeleteCategoryModelConsumer> consumerConfigurator)
        {
            endpointConfigurator.DiscardFaultedMessages();
        }
    }

    public class DeleteCategoryModelConsumer : IConsumer<DeleteCategoryModel>
    {
        private readonly CatalogContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructs an instance of <see cref="DeleteCategoryModelConsumer"/> using the specified context, mapper and
        /// logger.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="DeleteCategoryModelConsumer"/>.</param>
        public DeleteCategoryModelConsumer(CatalogContext context, IMapper mapper,
            ILogger<DeleteCategoryModelConsumer> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DeleteCategoryModel> context)
        {
            var categoryModel = context.Message;
            if (categoryModel.Id == 1)
            {
                throw new IndexOutOfRangeException("Category id cannot be 1 when deleting");
            }

            var category = await _context.Categories.Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == categoryModel.Id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category {categoryModel.Id} NOT FOUND");
            }

            foreach (var product in category.Products)
            {
                product.DateDeleted = DateTimeOffset.UtcNow;
                _context.Products.Attach(product);
                _context.Entry(product).State = EntityState.Modified;
                _logger.LogInformation($"Deleted product {product.Id} in category {categoryModel.Id} successfully");
            }

            category.DateDeleted = DateTimeOffset.UtcNow;
            _context.Categories.Attach(category);
            _context.Entry(category).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Deleted category {categoryModel.Id} successfully");

            await context.RespondAsync(_mapper.Map<CategoryModel>(category));
        }
    }
}