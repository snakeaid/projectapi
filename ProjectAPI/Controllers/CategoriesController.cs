using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using ProjectAPI.Primitives;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.BusinessLogic.Requests;

namespace ProjectAPI.Controllers
{
    /// <summary>
    /// API controller class which controls all HTTP requests related to operations with product categories.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        /// <summary>
        /// An instance of <see cref="IMediator"/> which is used for handling the incoming requests.
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructs an instance of <see cref="CategoriesController"/> using the specified mediator.
        /// </summary>
        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the HTTP GET request to get all categories, invoked at /api/categories route.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ActionResult{TValue}"/> for
        /// <see cref="IEnumerable{T}"/> of <see cref="CategoryModel"/></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryModel>>> Get()
        {
            var result = await _mediator.Send(new GetAllCategoriesRequest());

            return result;
        }

        /// <summary>
        /// Handles the HTTP GET request to get the category with the specified identifier, invoked at
        /// /api/categories/id route.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ActionResult{TValue}"/> for
        /// <see cref="CategoryModel"/></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryModel>> Get(int id)
        {
            var result = await _mediator.Send(new GetCategoryRequest { Id = id });
            return Ok(result);
        }

        /// <summary>
        /// Handles the HTTP DELETE request to delete the category with the specified identifier, invoked at
        /// /api/categories/id route.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ActionResult{TValue}"/> for
        /// <see cref="CategoryModel"/></returns>
        [HttpDelete("{id}"), Authorize(Roles = "Manager")]
        public async Task<ActionResult<Category>> Delete(int id)
        {

            var result = await _mediator.Send(new DeleteCategoryRequest { Id = id });
            return Ok(result);
        }

        /// <summary>
        /// Handles the HTTP POST request to post a new category, invoked at
        /// /api/categories/ route.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ActionResult{TValue}"/> for
        /// <see cref="CategoryModel"/></returns>
        [HttpPost, Authorize(Roles = "Manager")]
        public async Task<ActionResult<CategoryModel>> Post(CreateCategoryModel categoryModel)
        {
            var result = await _mediator.Send(new PostCategoryRequest { CategoryModel = categoryModel });
            return Ok(result);
        }

        /// <summary>
        /// Handles the HTTP PUT request to update the category with the specified identifier, invoked at
        /// /api/categories/id route.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ActionResult{TValue}"/> for
        /// <see cref="CategoryModel"/></returns>
        [HttpPut("{id}"), Authorize(Roles = "Manager")]
        public async Task<ActionResult<CategoryModel>> Put(int id, [FromBody] UpdateCategoryModel categoryModel)
        {
            var result = await _mediator.Send(new PutCategoryRequest { Id = id, CategoryModel = categoryModel });
            return Ok(result);
        }
    }
}