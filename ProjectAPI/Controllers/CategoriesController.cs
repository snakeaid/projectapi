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
        ApplicationContext db;
        public CategoriesController(ApplicationContext context)
        {
            db = context;
            if(!db.Categories.Any())
            {
                db.Categories.Add(new Category { Name="Uncategorized", Description="Products without a specific category are stored here" });
                db.SaveChanges();
            }
        }

        //GET api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            return await db.Categories.ToListAsync();
        }

        //GET api/categories/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            Category category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            return new ObjectResult(category);
        }

        //DELETE api/categories/id
        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> Delete(int id)
        {
            Category category = db.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            db.Categories.Remove(category);
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

        //PUT api/categories/
        [HttpPut]
        public async Task<ActionResult<Category>> Put(Category category)
        {
            //if (category == null) return BadRequest();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!db.Categories.Any(c => c.Id == category.Id)) return NotFound();

            db.Update(category);
            await db.SaveChangesAsync();
            return Ok(category);
        }
    }
}

