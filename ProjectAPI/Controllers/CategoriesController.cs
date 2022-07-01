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
    public class CategoriesController : ControllerBase
    {
        private readonly ILogger _logger;
        CatalogContext db;
        public CategoriesController(CatalogContext context, ILogger<CategoriesController> logger)
        {
            _logger = logger;
            db = context;
            if(!db.Categories.Any())
            {
                db.Categories.Add(new Category { Name = "Uncategorized", Description = "Products without a specific category are stored here" });
                db.SaveChanges();
            }
        }

        //GET api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            _logger.LogInformation($"Getting all categories");
            var list = await db.Categories.Include(c => c.Products).ToListAsync();

            _logger.LogInformation($"Got all categories sucessfully");
            return list;
        }

        //GET api/categories/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            _logger.LogInformation($"Getting category {id}");
            Category category = await db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                _logger.LogWarning($"Category {id} NOT FOUND");
                return NotFound();
            }

            _logger.LogInformation($"Got category {id} successfully");
            return new ObjectResult(category);
        }

        //DELETE api/categories/id
        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> Delete(int id)
        {
            if (id == 1)
            {
                _logger.LogWarning($"Category 1 cannot be deleted");
                return BadRequest("Category id cannot be 1");
            }
            Category category = db.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                _logger.LogWarning($"Category {id} NOT FOUND");
                return NotFound();
            }

            //удаление всех продуктов в категории?

            category.DateDeleted = DateTimeOffset.UtcNow;
            db.Categories.Attach(category);
            db.Entry(category).State = EntityState.Modified;

            await db.SaveChangesAsync();

            _logger.LogInformation($"Deleted category {id} successfully");
            return Ok(category);
        }

        //POST api/categories/
        [HttpPost]
        public async Task<ActionResult<Category>> Post(Category category)
        {
            //if (category == null) return BadRequest();
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Given category is invalid");
                return BadRequest(ModelState);
            }
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            //string categoryJson= JsonSerializer.Serialize(category);
            _logger.LogInformation($"Added category successfully");
            return Ok(category);
        }

        //PUT api/categories/id
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> Put(int id, [FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Given category is invalid");
                return BadRequest(ModelState);
            }
            if (!db.Categories.Any(c => c.Id == id))
            {
                _logger.LogWarning($"Category {id} NOT FOUND");
                return NotFound();
            }

            Category entity = db.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == id);
            entity.Name = category.Name;
            entity.Description = category.Description;

            Dictionary<string, string> finalSpecifications;

            foreach (Product product in entity.Products)
            {
                finalSpecifications = category.Specifications.ToDictionary(s => s, s => "");
                foreach (KeyValuePair<string, string> keyValuePair in product.SpecificationData)
                    if (finalSpecifications.ContainsKey(keyValuePair.Key)) finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
                product.SpecificationData = finalSpecifications;

                db.Products.Attach(product);
                db.Entry(product).State = EntityState.Modified;
            }

            entity.Specifications = category.Specifications;

            db.Categories.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;

            await db.SaveChangesAsync();

            _logger.LogInformation($"Updated category {id} successfully");
            return Ok(entity);
        }
    }
}