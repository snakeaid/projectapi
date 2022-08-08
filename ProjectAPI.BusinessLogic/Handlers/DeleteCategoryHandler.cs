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
    /// <summary>
    /// This class represents a MediatR request handler for category deletion and implements
    /// <see cref="IRequestHandler{TRequest, TResponse}"/> for
    /// <see cref="DeleteCategoryRequest"/>, <see cref="CategoryModel"/>.
    /// </summary>
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryRequest, CategoryModel>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructs an instance of <see cref="DeleteCategoryHandler"/> using the specified context, mapper and logger.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="DeleteCategoryHandler"/>.</param>
        public DeleteCategoryHandler(CatalogContext context, IMapper mapper, ILogger<DeleteCategoryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the specified request for category deletion.
        /// </summary>
        /// <param name="request">An instance of <see cref="DeleteCategoryRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="CategoryModel"/></returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the category identifier is equal 1.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if there is no category found by the specified identifier.</exception>
        public async Task<CategoryModel> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
        {
            if (request.Id == 1)
            {
                throw new IndexOutOfRangeException("Category id cannot be 1 when deleting");
            }
            Category category = _context.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == request.Id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category {request.Id} NOT FOUND");
            }

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