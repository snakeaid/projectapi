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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler to update a category and implements
    /// <see cref="IRequestHandler{TRequest, TResponse}"/> for
    /// <see cref="PutCategoryRequest"/>, <see cref="CategoryModel"/>.
    /// </summary>
    public class PutCategoryHandler : IRequestHandler<PutCategoryRequest, CategoryModel>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IValidator<UpdateCategoryModel> _validator;

        /// <summary>
        /// Constructs an instance of <see cref="PutCategoryHandler"/> using the specified context, mapper,
        /// logger and validator.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="PostCategoryHandler"/>.</param>
        /// <param name="validator">An instance of <see cref="IValidator{T}"/> for <see cref="CategoryModel"/>.</param>
        public PutCategoryHandler(CatalogContext context, IMapper mapper, ILogger<PutCategoryHandler> logger,
            IValidator<UpdateCategoryModel> validator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        /// <summary>
        /// Handles the specified request to update a category.
        /// </summary>
        /// <param name="request">An instance of <see cref="PutCategoryRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="CategoryModel"/></returns>
        /// <exception cref="ArgumentException">Thrown if the provided model of the category is invalid.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if there is no category found by the specified identifier.</exception>
        public async Task<CategoryModel> Handle(PutCategoryRequest request, CancellationToken cancellationToken)
        {
            UpdateCategoryModel categoryModel = request.CategoryModel;
            if (!_context.Categories.Any(c => c.Id == request.Id))
            {
                throw new KeyNotFoundException($"Category {request.Id} NOT FOUND");
            }
            ValidationResult result = await _validator.ValidateAsync(categoryModel);
            if (!result.IsValid)
            {
                string errors = JsonSerializer.Serialize(result.ToDictionary());
                throw new ArgumentException(errors);
            }

            Category entity = _context.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == request.Id);
            entity.Name = categoryModel.Name;
            entity.Description = categoryModel.Description;

            Dictionary<string, string> finalSpecifications;

            foreach (Product product in entity.Products)
            {
                finalSpecifications = categoryModel.Specifications.ToDictionary(s => s, s => "");
                foreach (KeyValuePair<string, string> keyValuePair in product.SpecificationData)
                    if (finalSpecifications.ContainsKey(keyValuePair.Key)) finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
                product.SpecificationData = finalSpecifications;

                _context.Products.Attach(product);
                _context.Entry(product).State = EntityState.Modified;
            }

            entity.Specifications = categoryModel.Specifications;

            _context.Categories.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Updated category {request.Id} successfully");
            return _mapper.Map<CategoryModel>(entity);
        }
    }
}