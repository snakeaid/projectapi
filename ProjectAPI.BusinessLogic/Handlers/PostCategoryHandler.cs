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
    public class PostCategoryHandler : IRequestHandler<PostCategoryRequest, CategoryModel>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IValidator<CategoryModel> _validator;

        public PostCategoryHandler(CatalogContext context, IMapper mapper, ILogger<PostCategoryHandler> logger,
            IValidator<CategoryModel> validator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        public async Task<CategoryModel> Handle(PostCategoryRequest request, CancellationToken cancellationToken)
        {
            CategoryModel categoryModel = request.CategoryModel;
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