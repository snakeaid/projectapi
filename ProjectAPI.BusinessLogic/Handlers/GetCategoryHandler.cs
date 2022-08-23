using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    /// This class represents a MediatR request handler to get a category and implements
    /// <see cref="IRequestHandler{TRequest, TResponse}"/> for
    /// <see cref="GetCategoryRequest"/>, <see cref="CategoryModel"/>.
    /// </summary>
	public class GetCategoryHandler : IRequestHandler<GetCategoryRequest, CategoryModel>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructs an instance of <see cref="GetCategoryHandler"/> using the specified context, mapper and logger.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="GetCategoryHandler"/>.</param>
        public GetCategoryHandler(CatalogContext context, IMapper mapper, ILogger<GetCategoryHandler> logger)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the specified request to get a category.
        /// </summary>
        /// <param name="request">An instance of <see cref="GetCategoryRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="CategoryModel"/></returns>
        /// <exception cref="KeyNotFoundException">Thrown if there is no category found by the specified identifier.</exception>
        public async Task<CategoryModel> Handle(GetCategoryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting category {request.Id}");
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category {request.Id} NOT FOUND");
            }
            CategoryModel categoryModel = _mapper.Map<CategoryModel>(category);

            _logger.LogInformation($"Got category {request.Id} successfully");
            return categoryModel;
        }
    }
}