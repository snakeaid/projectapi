using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        CatalogContext db;
        public CategoriesController(CatalogContext context)
        {
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
            return await db.Categories.Include(c => c.Products).ToListAsync();
        }

        //GET api/categories/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            Category category = await db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            return new ObjectResult(category);
        }

        //DELETE api/categories/id
        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> Delete(int id)
        {
            if (id == 1) return BadRequest("Category id cannot be 1");
            Category category = db.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();

            category.DateDeleted = DateTimeOffset.UtcNow;
            db.Categories.Attach(category);
            db.Entry(category).State = EntityState.Modified;

            await db.SaveChangesAsync();
            return Ok(category);
        }

        //POST api/categories/
        [HttpPost]
        public async Task<ActionResult<Category>> Post(Category category)
        {
            //if (category == null) return BadRequest();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Categories.Add(category);
            await db.SaveChangesAsync();
            return Ok(category);
        }

        //PUT api/categories/id
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> Put(int id, [FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!db.Categories.Any(c => c.Id == id)) return NotFound();

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
            return Ok(entity);
        }
    }
}