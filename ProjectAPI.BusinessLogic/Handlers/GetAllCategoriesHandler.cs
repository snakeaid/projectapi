using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using ProjectAPI.DataAccess;
using ProjectAPI.Primitives;
using ProjectAPI.BusinessLogic.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler to get all categories and implements
    /// <see cref="IRequestHandler{TRequest, TResponse}"/> for
    /// <see cref="GetAllCategoriesRequest"/>, <see cref="List{T}"/> of <see cref="CategoryModel"/>.
    /// </summary>
    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesRequest, List<CategoryModel>>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructs an instance of <see cref="GetAllCategoriesHandler"/> using the specified context, mapper and logger.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="GetAllCategoriesHandler"/>.</param>
        public GetAllCategoriesHandler(CatalogContext context, IMapper mapper, ILogger<GetAllCategoriesHandler> logger)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the specified request to get all categories.
        /// </summary>
        /// <param name="request">An instance of <see cref="GetAllCategoriesRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="List{T}"/> of <see cref="CategoryModel"/></returns>
        public async Task<List<CategoryModel>> Handle(GetAllCategoriesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting all categories");
            var list = await _context.Categories.Include(c => c.Products).ToListAsync();
            var listModel = _mapper.Map<List<CategoryModel>>(list);

            _logger.LogInformation($"Got all categories sucessfully");
            return listModel;
        }
    }
}