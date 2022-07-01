using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ProjectAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        CatalogContext db;
        public ProductsController(CatalogContext context, ILogger<ProductsController> logger, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
            _logger = logger;
            if (!db.Products.Any())
            {
                //log?
                if (!db.Categories.Any())
                {
                    //log?
                    db.Categories.Add(new Category { Name = "Uncategorized", Description = "Products without a specific category are stored here" });
                    db.SaveChanges();
                    //log?
                }
                db.Products.Add(new Product { Name = "Default product", Description = "Product", CategoryId = 1 });
                //log?
                db.SaveChanges();
            }
        }

        //GET api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> Get()
        {
            _logger.LogInformation($"Getting all products");
            var list = await db.Products.Include(p => p.Category).ToListAsync();
            var listDTO = _mapper.Map<List<ProductDTO>>(list);

            _logger.LogInformation($"Got all products sucessfully");
            return Ok(listDTO);
        }

        //GET api/products/id
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> Get(int id)
        {
            _logger.LogInformation($"Getting product {id}");
            Product product = await db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                _logger.LogWarning($"Product {id} NOT FOUND");
                return NotFound();
            }
            ProductDTO productDTO = _mapper.Map<ProductDTO>(product);

            _logger.LogInformation($"Got product {id} successfully");
            return Ok(productDTO);
        }

        //DELETE api/products/id
        [HttpDelete("{id}"), Authorize(Roles = "Manager")]
        public async Task<ActionResult<Product>> Delete(int id)
        {
            Product product = db.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                _logger.LogWarning($"Product {id} NOT FOUND");
                return NotFound();
            }

            product.DateDeleted = DateTimeOffset.UtcNow;
            db.Products.Attach(product);
            db.Entry(product).State = EntityState.Modified;

            await db.SaveChangesAsync();

            _logger.LogInformation($"Deleted product {id} successfully");
            return Ok(product);
        }

        //POST api/products/
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> Post(ProductDTO productDTO)
        {
            //if (category == null) return BadRequest();
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Given product is invalid");
                return BadRequest(ModelState);
            }
            if (db.Categories.FirstOrDefault(c => c.Id == productDTO.CategoryId) == null)
            {
                _logger.LogWarning($"Category {productDTO.CategoryId} NOT FOUND. Assigning category 1 to the product");
                productDTO.CategoryId = 1;
            }

            Dictionary<string, string> finalSpecifications = db.Categories.FirstOrDefault(c => c.Id==productDTO.CategoryId).Specifications.ToDictionary(s => s, s => "");
            foreach (KeyValuePair<string, string> keyValuePair in productDTO.SpecificationData)
                if(finalSpecifications.ContainsKey(keyValuePair.Key)) finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
            productDTO.SpecificationData = finalSpecifications;

            Product product = _mapper.Map<Product>(productDTO);
            db.Products.Add(product);
            await db.SaveChangesAsync();

            //string productJson= JsonSerializer.Serialize(product);
            _logger.LogInformation($"Added product successfully");
            return Ok(_mapper.Map<ProductDTO>(product));
        }

        //PUT api/products/id
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDTO>> Put(int id, [FromBody] ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Given product is invalid");
                return BadRequest(ModelState);
            }
            if (db.Categories.FirstOrDefault(c => c.Id == productDTO.CategoryId) == null)
            {
                _logger.LogWarning($"Category {productDTO.CategoryId} NOT FOUND. Assigning category 1 to the product");
                productDTO.CategoryId = 1;
            }
            if (!db.Products.Any(p => p.Id == id))
            {
                _logger.LogWarning($"Product {id} NOT FOUND");
                return NotFound();
            }

            Product entity = db.Products.FirstOrDefault(p => p.Id == id);
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
                Dictionary<string, string> finalSpecifications = db.Categories.FirstOrDefault(c => c.Id == entity.CategoryId).Specifications.ToDictionary(s => s, s => "");
                foreach (KeyValuePair<string, string> keyValuePair in productDTO.SpecificationData)
                    if (finalSpecifications.ContainsKey(keyValuePair.Key)) finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
                entity.SpecificationData = finalSpecifications;
            }
            db.Products.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;

            await db.SaveChangesAsync();

            _logger.LogInformation($"Updated product {id} successfully");
            return Ok(_mapper.Map<ProductDTO>(entity));
        }
    }
}