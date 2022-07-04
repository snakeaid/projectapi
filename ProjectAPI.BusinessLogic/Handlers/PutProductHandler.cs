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

namespace ProjectAPI.BusinessLogic.Handlers
{
    public class PutProductHandler : IRequestHandler<PutProductRequest, ProductDTO>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;

        public PutProductHandler(CatalogContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductDTO> Handle(PutProductRequest request, CancellationToken cancellationToken)
        {
            //    if (!ModelState.IsValid)
            //    {
            //        _logger.LogWarning($"Given product is invalid");
            //        return BadRequest(ModelState);
            //    }
            ProductDTO productDTO = request.ProductDTO;
            if (_context.Categories.FirstOrDefault(c => c.Id == productDTO.CategoryId) == null)
            {
                //_logger.LogWarning($"Category {productDTO.CategoryId} NOT FOUND. Assigning category 1 to the product");
                productDTO.CategoryId = 1;
            }
            if (!_context.Products.Any(p => p.Id == request.Id))
            {
                //_logger.LogWarning($"Product {id} NOT FOUND");
                throw new KeyNotFoundException();
            }

            Product entity = _context.Products.FirstOrDefault(p => p.Id == request.Id);
            entity.Name = productDTO.Name;
            entity.Description = productDTO.Description;

            if (entity.CategoryId == productDTO.CategoryId)
            {
                foreach (KeyValuePair<string, string> keyValuePair in productDTO.SpecificationData)
                    if (entity.SpecificationData.ContainsKey(keyValuePair.Key)) entity.SpecificationData[keyValuePair.Key] = keyValuePair.Value;
            }
            else
            {
                entity.CategoryId = productDTO.CategoryId;
                Dictionary<string, string> finalSpecifications = _context.Categories.FirstOrDefault(c => c.Id == entity.CategoryId).Specifications.ToDictionary(s => s, s => "");
                foreach (KeyValuePair<string, string> keyValuePair in productDTO.SpecificationData)
                    if (finalSpecifications.ContainsKey(keyValuePair.Key)) finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
                entity.SpecificationData = finalSpecifications;
            }
            _context.Products.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            //_logger.LogInformation($"Updated product {id} successfully");
            return _mapper.Map<ProductDTO>(entity);
        }
    }
}