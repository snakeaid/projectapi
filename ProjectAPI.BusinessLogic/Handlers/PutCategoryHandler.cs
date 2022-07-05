﻿using System;
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
    public class PutCategoryHandler : IRequestHandler<PutCategoryRequest, CategoryModel>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IValidator<CategoryModel> _validator;

        public PutCategoryHandler(CatalogContext context, IMapper mapper, ILogger<PutCategoryHandler> logger,
            IValidator<CategoryModel> validator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        public async Task<CategoryModel> Handle(PutCategoryRequest request, CancellationToken cancellationToken)
        {
            CategoryModel categoryModel = request.CategoryModel;
            if (!_context.Categories.Any(c => c.Id == request.Id))
            {
                _logger.LogWarning($"Category {request.Id} NOT FOUND");
                throw new KeyNotFoundException();
            }
            ValidationResult result = await _validator.ValidateAsync(categoryModel);
            if (!result.IsValid)
            {
                _logger.LogWarning($"Given category is invalid");
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