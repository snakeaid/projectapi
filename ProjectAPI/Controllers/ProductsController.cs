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
    public class ProductsController : ControllerBase
    {
        ApplicationContext db;
        public ProductsController(ApplicationContext context)
        {
            db = context;
            if (!db.Products.Any())
            {
                if (!db.Categories.Any())
                {
                    db.Categories.Add(new Category { Name = "Uncategorized", Description = "Products without a specific category are stored here" });
                    db.SaveChanges();
                }
                db.Products.Add(new Product { Name = "Default product", Description = "Product", CategoryId = 1 });
                db.SaveChanges();
            }
        }

        //GET api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            return await db.Products.Include(p => p.Category).ToListAsync();
        }

        //GET api/products/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            Product product = await db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return new ObjectResult(product);
        }

        //DELETE api/products/id
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> Delete(int id)
        {
            Product product = db.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            product.DateDeleted = DateTimeOffset.UtcNow;
            db.Products.Attach(product);
            db.Entry(product).State = EntityState.Modified;

            await db.SaveChangesAsync();
            return Ok(product);
        }

        //POST api/products/
        [HttpPost]
        public async Task<ActionResult<Product>> Post(Product product)
        {
            //if (category == null) return BadRequest();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (db.Categories.FirstOrDefault(c => c.Id == product.CategoryId) == null) product.CategoryId = 1;
            db.Products.Add(product);
            await db.SaveChangesAsync();
            return Ok(product);
        }

        //PUT api/categories/
        [HttpPut]
        public async Task<ActionResult<Product>> Put(Product product)
        {
            //if (category == null) return BadRequest();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (db.Categories.FirstOrDefault(c => c.Id == product.CategoryId) == null) product.CategoryId = 1;
            if (!db.Products.Any(p => p.Id == product.Id)) return NotFound();

            db.Update(product);
            await db.SaveChangesAsync();
            return Ok(product);
        }
    }
}

