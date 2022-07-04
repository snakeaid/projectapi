using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using ProjectAPI.DataAccess;
using ProjectAPI.Primitives;
using ProjectAPI.BusinessLogic.Requests;
using Microsoft.EntityFrameworkCore;

namespace ProjectAPI.BusinessLogic.Handlers
{
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsRequest, List<ProductDTO>>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;

        public GetAllProductsHandler(CatalogContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProductDTO>> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            //_logger.LogInformation($"Getting all products");
            var list = await _context.Products.ToListAsync();
            var listDTO = _mapper.Map<List<ProductDTO>>(list);

            //_logger.LogInformation($"Got all products sucessfully");

            return listDTO;
        }
    }
}