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
    public class DeleteProductHandler : IRequestHandler<DeleteProductRequest, ProductModel>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DeleteProductHandler(CatalogContext context, IMapper mapper, ILogger<DeleteProductHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductModel> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
        {
            Product product = _context.Products.FirstOrDefault(p => p.Id == request.Id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product {request.Id} NOT FOUND");
            }

            product.DateDeleted = DateTimeOffset.UtcNow;
            _context.Products.Attach(product);
            _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Deleted product {request.Id} successfully");
            return _mapper.Map<ProductModel>(product);
        }
    }
}