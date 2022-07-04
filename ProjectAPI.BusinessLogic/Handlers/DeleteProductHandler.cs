using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.BusinessLogic.Requests;
using Microsoft.EntityFrameworkCore;

namespace ProjectAPI.BusinessLogic.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductRequest, Product>
    {
        private readonly CatalogContext _context;

        public DeleteProductHandler(CatalogContext context)
        {
            _context = context;
        }

        public async Task<Product> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
        {
            Product product = _context.Products.FirstOrDefault(p => p.Id == request.Id);
            if (product == null)
            {
                //_logger.LogWarning($"Product {id} NOT FOUND");
                throw new KeyNotFoundException();
            }

            product.DateDeleted = DateTimeOffset.UtcNow;
            _context.Products.Attach(product);
            _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return product;
            //_logger.LogInformation($"Deleted product {id} successfully");
        }
    }
}