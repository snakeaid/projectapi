using System;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Logging;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Primitives;

namespace ProjectAPI.CategoryService
{
    public class CreateCategoryModelConsumerDefinition : ConsumerDefinition<CreateCategoryModelConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<CreateCategoryModelConsumer> consumerConfigurator)
        {
            endpointConfigurator.DiscardFaultedMessages();
        }
    }

    public class CreateCategoryModelConsumer : IConsumer<CreateCategoryModel>
    {
        private readonly CatalogContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateCategoryModel> _validator;

        /// <summary>
        /// Constructs an instance of <see cref="CreateCategoryModelConsumer"/> using the specified context, mapper,
        /// logger and validator.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="CreateCategoryModelConsumer"/>.</param>
        /// <param name="validator">An instance of <see cref="IValidator{T}"/> for <see cref="CreateCategoryModel"/>.</param>
        public CreateCategoryModelConsumer(CatalogContext context, IMapper mapper,
            ILogger<CreateCategoryModelConsumer> logger, IValidator<CreateCategoryModel> validator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        public async Task Consume(ConsumeContext<CreateCategoryModel> context)
        {
            var categoryModel = context.Message;

            var result = await _validator.ValidateAsync(categoryModel);
            if (!result.IsValid)
            {
                string errors = JsonSerializer.Serialize(result.ToDictionary());
                throw new ArgumentException(errors);
            }

            var category = _mapper.Map<Category>(categoryModel);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Added category successfully");

            await context.RespondAsync(_mapper.Map<CategoryModel>(category));
        }
    }
}