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

namespace ProjectAPI.CategoryService
{
    /// <summary>
    /// This class represents a MassTransit definition class which defines the behaviour
    /// of <see cref="UpdateCategoryModelConsumer"/> and implements <see cref="ConsumerDefinition{TConsumer}"/>
    /// for <see cref="UpdateCategoryModelConsumer"/>.
    /// </summary>
    public class UpdateCategoryModelConsumerDefinition : ConsumerDefinition<UpdateCategoryModelConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<UpdateCategoryModelConsumer> consumerConfigurator)
        {
            endpointConfigurator.DiscardFaultedMessages();
        }
    }

    /// <summary>
    /// This class represents a MassTransit consumer class which consumes messages to update a category and implements
    /// <see cref="IConsumer{TMessage}"/> for
    /// <see cref="UpdateCategoryModel"/>.
    /// </summary>
    public class UpdateCategoryModelConsumer : IConsumer<UpdateCategoryModel>
    {
        private readonly CatalogContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateCategoryModel> _validator;

        /// <summary>
        /// Constructs an instance of <see cref="UpdateCategoryModelConsumer"/> using the specified context, mapper,
        /// logger and validator.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="UpdateCategoryModelConsumer"/>.</param>
        /// <param name="validator">An instance of <see cref="IValidator{T}"/> for <see cref="UpdateCategoryModel"/>.</param>
        public UpdateCategoryModelConsumer(CatalogContext context, IMapper mapper,
            ILogger<UpdateCategoryModelConsumer> logger, IValidator<UpdateCategoryModel> validator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        /// <summary>
        /// Consumes the message to update a category.
        /// </summary>
        /// <param name="context">An instance of <see cref="ConsumeContext{T}"/> for <see cref="UpdateCategoryModel"/>.</param>
        /// <exception cref="ArgumentException">Thrown if the provided model of the category is invalid.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if there is no category found by the specified identifier.</exception>
        public async Task Consume(ConsumeContext<UpdateCategoryModel> context)
        {
            var categoryModel = context.Message;
            if (!_context.Categories.Any(c => c.Id == categoryModel.Id))
            {
                throw new KeyNotFoundException($"Category {categoryModel.Id} NOT FOUND");
            }

            var result = await _validator.ValidateAsync(categoryModel);
            if (!result.IsValid)
            {
                string errors = JsonSerializer.Serialize(result.ToDictionary());
                throw new ArgumentException(errors);
            }

            var category = await _context.Categories.Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == categoryModel.Id);
            category.Name = categoryModel.Name;
            category.Description = categoryModel.Description;

            Dictionary<string, string> finalSpecifications;

            foreach (var product in category.Products)
            {
                finalSpecifications = categoryModel.Specifications.ToDictionary(s => s, s => "");
                foreach (KeyValuePair<string, string> keyValuePair in product.SpecificationData)
                    if (finalSpecifications.ContainsKey(keyValuePair.Key))
                        finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
                product.SpecificationData = finalSpecifications;

                _context.Products.Attach(product);
                _context.Entry(product).State = EntityState.Modified;
            }

            category.Specifications = categoryModel.Specifications;

            _context.Categories.Attach(category);
            _context.Entry(category).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Updated category {categoryModel.Id} successfully");

            await context.RespondAsync(_mapper.Map<CategoryModel>(category));
        }
    }
}