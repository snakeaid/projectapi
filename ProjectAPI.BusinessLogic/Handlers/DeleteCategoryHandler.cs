using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using AutoMapper;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Primitives;
using ProjectAPI.BusinessLogic.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ProjectAPI.BusinessLogic.Handlers
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryRequest, CategoryModel>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DeleteCategoryHandler(CatalogContext context, IMapper mapper, ILogger<DeleteCategoryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CategoryModel> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
        {
            //TODO: продумать категорию Uncategorized - ???
            if (request.Id == 1)
            {
                _logger.LogWarning($"Category id cannot be 1 when deleting");
                //TODO: нельзя кидать этот эксепшен!
                throw new IndexOutOfRangeException();
            }
            Category category = _context.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == request.Id);
            if (category == null)
            {
                _logger.LogWarning($"Category {request.Id} NOT FOUND");
                throw new KeyNotFoundException();
            }

            //удаление всех продуктов категории
            foreach(Product product in category.Products)
            {
                product.DateDeleted = DateTimeOffset.UtcNow;
                _context.Products.Attach(product);
                _context.Entry(product).State = EntityState.Modified;
                _logger.LogInformation($"Deleted product {product.Id} in category {request.Id} successfully");
            }

            category.DateDeleted = DateTimeOffset.UtcNow;
            _context.Categories.Attach(category);
            _context.Entry(category).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Deleted category {request.Id} successfully");
            return _mapper.Map<CategoryModel>(category);
        }
    }
}