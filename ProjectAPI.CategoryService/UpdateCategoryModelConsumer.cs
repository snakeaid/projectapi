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
    public class UpdateCategoryModelConsumerDefinition : ConsumerDefinition<UpdateCategoryModelConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<UpdateCategoryModelConsumer> consumerConfigurator)
        {
            endpointConfigurator.DiscardFaultedMessages();
        }
    }

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