using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using ProjectAPI.Primitives;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.BusinessLogic.Requests;

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public CategoriesController(ILogger<CategoriesController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
            //здесь было добавление категории по умолчанию
        }

        //GET api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryModel>>> Get()
        {
            var result = await _mediator.Send(new GetAllCategoriesRequest());

            return result;
        }

        //GET api/categories/id
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryModel>> Get(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetCategoryRequest { Id = id });
                return Ok(result);
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }
        }

        //DELETE api/categories/id
        [HttpDelete("{id}"), Authorize(Roles = "Manager")]
        public async Task<ActionResult<Category>> Delete(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteCategoryRequest { Id = id });
                return Ok(result);
            }
            catch(IndexOutOfRangeException)
            {
                return BadRequest("Category id cannot be 1");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        //POST api/categories/
        [HttpPost, Authorize(Roles = "Manager")]
        public async Task<ActionResult<CategoryModel>> Post(CategoryModel categoryModel)
        {
            try
            {
                var result = await _mediator.Send(new PostCategoryRequest { CategoryModel = categoryModel });
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(System.Text.RegularExpressions.Regex.Unescape(ex.Message));
            }
        }

        //PUT api/categories/id
        [HttpPut("{id}"), Authorize(Roles = "Manager")]
        public async Task<ActionResult<CategoryModel>> Put(int id, [FromBody] CategoryModel categoryModel)
        {
            try
            {
                var result = await _mediator.Send(new PutCategoryRequest { Id = id, CategoryModel = categoryModel });
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(System.Text.RegularExpressions.Regex.Unescape(ex.Message));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}