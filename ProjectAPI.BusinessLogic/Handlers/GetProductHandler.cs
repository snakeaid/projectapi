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
	public class GetProductHandler : IRequestHandler<GetProductRequest, ProductModel>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetProductHandler(CatalogContext context, IMapper mapper, ILogger<GetProductHandler> logger)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductModel> Handle(GetProductRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting product {request.Id}");
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (product == null)
            {
                _logger.LogWarning($"Product {request.Id} NOT FOUND");
                throw new KeyNotFoundException();
            }
            ProductModel productModel = _mapper.Map<ProductModel>(product);

            _logger.LogInformation($"Got product {request.Id} successfully");
            return productModel;
        }
    }
}