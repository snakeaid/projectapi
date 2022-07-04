using System;
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

namespace ProjectAPI.BusinessLogic.Handlers
{
    public class PostProductHandler : IRequestHandler<PostProductRequest, ProductDTO>
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;

        public PostProductHandler(CatalogContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductDTO> Handle(PostProductRequest request, CancellationToken cancellationToken)
        {
            ProductDTO productDTO = request.ProductDTO;
            //if (!ModelState.IsValid)
            //{
            //    //_logger.LogWarning($"Given product is invalid");
            //    return BadRequest(ModelState);
            //}
            if (_context.Categories.FirstOrDefault(c => c.Id == productDTO.CategoryId) == null)
            {
                //_logger.LogWarning($"Category {productDTO.CategoryId} NOT FOUND. Assigning category 1 to the product");
                productDTO.CategoryId = 1;
            }

            Dictionary<string, string> finalSpecifications = _context.Categories.FirstOrDefault(c => c.Id == productDTO.CategoryId).Specifications.ToDictionary(s => s, s => "");
            foreach (KeyValuePair<string, string> keyValuePair in productDTO.SpecificationData)
                if (finalSpecifications.ContainsKey(keyValuePair.Key)) finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
            productDTO.SpecificationData = finalSpecifications;

            Product product = _mapper.Map<Product>(productDTO);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            //string productJson= JsonSerializer.Serialize(product);
            //_logger.LogInformation($"Added product successfully");
            return _mapper.Map<ProductDTO>(product);
        }
    }
}