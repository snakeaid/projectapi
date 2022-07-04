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
	public class GetCategoryHandler : IRequestHandler<GetCategoryRequest, CategoryModel>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetCategoryHandler(CatalogContext context, IMapper mapper, ILogger<GetCategoryHandler> logger)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<CategoryModel> Handle(GetCategoryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting category {request.Id}");
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id);
            if (category == null)
            {
                _logger.LogWarning($"Category {request.Id} NOT FOUND");
                throw new KeyNotFoundException();
            }
            CategoryModel categoryModel = _mapper.Map<CategoryModel>(category);

            _logger.LogInformation($"Got category {request.Id} successfully");
            return categoryModel;
        }
    }
}