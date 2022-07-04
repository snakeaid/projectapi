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
    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesRequest, List<CategoryModel>>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetAllCategoriesHandler(CatalogContext context, IMapper mapper, ILogger<GetAllCategoriesHandler> logger)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

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