using System;
using System.Text.Json;
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
using Microsoft.Extensions.Logging;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler to post a category and implements
    /// <see cref="IRequestHandler{TRequest, TResponse}"/> for
    /// <see cref="PostCategoryRequest"/>, <see cref="CategoryModel"/>.
    /// </summary>
    public class PostCategoryHandler : IRequestHandler<PostCategoryRequest, CategoryModel>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IValidator<CreateCategoryModel> _validator;

        /// <summary>
        /// Constructs an instance of <see cref="PostCategoryHandler"/> using the specified context, mapper,
        /// logger and validator.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="PostCategoryHandler"/>.</param>
        /// <param name="validator">An instance of <see cref="IValidator{T}"/> for <see cref="CategoryModel"/>.</param>
        public PostCategoryHandler(CatalogContext context, IMapper mapper, ILogger<PostCategoryHandler> logger,
            IValidator<CreateCategoryModel> validator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        /// <summary>
        /// Handles the specified request to post a category.
        /// </summary>
        /// <param name="request">An instance of <see cref="PostCategoryRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="CategoryModel"/></returns>
        /// <exception cref="ArgumentException">Thrown if the provided model of the category is invalid.</exception>
        public async Task<CategoryModel> Handle(PostCategoryRequest request, CancellationToken cancellationToken)
        {
            CreateCategoryModel categoryModel = request.CategoryModel;
            ValidationResult result = await _validator.ValidateAsync(categoryModel);
            if (!result.IsValid)
            {
                string errors = JsonSerializer.Serialize(result.ToDictionary());
                throw new ArgumentException(errors);
            }

            Category category = _mapper.Map<Category>(categoryModel);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            //string categoryJson= JsonSerializer.Serialize(category);
            _logger.LogInformation($"Added category successfully");
            return _mapper.Map<CategoryModel>(category);
        }
    }
}