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
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsRequest, List<ProductModel>>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetAllProductsHandler(CatalogContext context, IMapper mapper, ILogger<GetAllProductsHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProductModel>> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting all products");
            var list = await _context.Products.ToListAsync();
            var listModel = _mapper.Map<List<ProductModel>>(list);

            _logger.LogInformation($"Got all products sucessfully");

            return listModel;
        }
    }
}