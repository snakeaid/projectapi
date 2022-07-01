using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger _logger;
        CatalogContext db;
        public ProductsController(CatalogContext context, ILogger<ProductsController> logger)
        {
            _logger = logger;
            db = context;
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
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            _logger.LogInformation($"Getting all products");
            var list = await db.Products.Include(p => p.Category).ToListAsync();

            _logger.LogInformation($"Got all products sucessfully");
            return list;
        }

        //GET api/products/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            _logger.LogInformation($"Getting product {id}");
            Product product = await db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                _logger.LogWarning($"Product {id} NOT FOUND");
                return NotFound();
            }
            _logger.LogInformation($"Got product {id} successfully");
            return new ObjectResult(product);
        }

        //DELETE api/products/id
        [HttpDelete("{id}")]
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
        public async Task<ActionResult<Product>> Post(Product product)
        {
            //if (category == null) return BadRequest();
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Given product is invalid");
                return BadRequest(ModelState);
            }
            if (db.Categories.FirstOrDefault(c => c.Id == product.CategoryId) == null)
            {
                _logger.LogWarning($"Category {product.CategoryId} NOT FOUND. Assigning category 1 to the product");
                product.CategoryId = 1;
            }

            Dictionary<string, string> finalSpecifications = db.Categories.FirstOrDefault(c => c.Id==product.CategoryId).Specifications.ToDictionary(s => s, s => "");
            foreach (KeyValuePair<string, string> keyValuePair in product.SpecificationData)
                if(finalSpecifications.ContainsKey(keyValuePair.Key)) finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
            product.SpecificationData = finalSpecifications;

            db.Products.Add(product);
            await db.SaveChangesAsync();

            //string productJson= JsonSerializer.Serialize(product);
            _logger.LogInformation($"Added product successfully");
            return Ok(product);
        }

        //PUT api/products/id
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> Put(int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Given product is invalid");
                return BadRequest(ModelState);
            }
            if (db.Categories.FirstOrDefault(c => c.Id == product.CategoryId) == null)
            {
                _logger.LogWarning($"Category {product.CategoryId} NOT FOUND. Assigning category 1 to the product");
                product.CategoryId = 1;
            }
            if (!db.Products.Any(p => p.Id == id))
            {
                _logger.LogWarning($"Product {id} NOT FOUND");
                return NotFound();
            }

            Product entity = db.Products.FirstOrDefault(p => p.Id == id);
            entity.Name = product.Name;
            entity.Description = product.Description;

            if (entity.CategoryId == product.CategoryId)
            {
                foreach (KeyValuePair<string, string> keyValuePair in product.SpecificationData)
                    if (entity.SpecificationData.ContainsKey(keyValuePair.Key)) entity.SpecificationData[keyValuePair.Key] = keyValuePair.Value;
            }
            else
            {
                entity.CategoryId = product.CategoryId;
                Dictionary<string, string> finalSpecifications = db.Categories.FirstOrDefault(c => c.Id == entity.CategoryId).Specifications.ToDictionary(s => s, s => "");
                foreach (KeyValuePair<string, string> keyValuePair in product.SpecificationData)
                    if (finalSpecifications.ContainsKey(keyValuePair.Key)) finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
                entity.SpecificationData = finalSpecifications;
            }
            db.Products.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;

            await db.SaveChangesAsync();

            _logger.LogInformation($"Updated product {id} successfully");
            return Ok(entity);
        }
    }
}