using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using AutoMapper;
using ProjectAPI.Primitives;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.BusinessLogic.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ProjectAPI.BusinessLogic.Handlers
{
    public class PutProductHandler : IRequestHandler<PutProductRequest, ProductModel>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PutProductHandler(CatalogContext context, IMapper mapper, ILogger<PutProductHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductModel> Handle(PutProductRequest request, CancellationToken cancellationToken)
        {
            //    if (!ModelState.IsValid)
            //    {
            //        _logger.LogWarning($"Given product is invalid");
            //        return BadRequest(ModelState);
            //    }
            ProductModel productModel = request.ProductModel;
            if (_context.Categories.FirstOrDefault(c => c.Id == productModel.CategoryId) == null)
            {
                _logger.LogWarning($"Category {productModel.CategoryId} NOT FOUND. Assigning category 1 to the product");
                productModel.CategoryId = 1;
            }
            if (!_context.Products.Any(p => p.Id == request.Id))
            {
                _logger.LogWarning($"Product {request.Id} NOT FOUND");
                throw new KeyNotFoundException();
            }

            Product entity = _context.Products.FirstOrDefault(p => p.Id == request.Id);
            entity.Name = productModel.Name;
            entity.Description = productModel.Description;

            if (entity.CategoryId == productModel.CategoryId)
            {
                foreach (KeyValuePair<string, string> keyValuePair in productModel.SpecificationData)
                    if (entity.SpecificationData.ContainsKey(keyValuePair.Key)) entity.SpecificationData[keyValuePair.Key] = keyValuePair.Value;
            }
            else
            {
                entity.CategoryId = productModel.CategoryId;
                Dictionary<string, string> finalSpecifications = _context.Categories.FirstOrDefault(c => c.Id == entity.CategoryId).Specifications.ToDictionary(s => s, s => "");
                foreach (KeyValuePair<string, string> keyValuePair in productModel.SpecificationData)
                    if (finalSpecifications.ContainsKey(keyValuePair.Key)) finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
                entity.SpecificationData = finalSpecifications;
            }
            _context.Products.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Updated product {request.Id} successfully");
            return _mapper.Map<ProductModel>(entity);
        }
    }
}