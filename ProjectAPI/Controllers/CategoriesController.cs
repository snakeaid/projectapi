using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.Primitives;

namespace ProjectAPI.Controllers
{
    /// <summary>
    /// API controller class which controls all HTTP requests related to operations with product categories.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<CategoryModel>))]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new GetAllCategoriesRequest());

            return Ok(result);
        }

        /// <summary>
        /// Handles the HTTP GET request to get the category with the specified identifier, invoked at
        /// /api/categories/id route.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ActionResult{TValue}"/> for
        /// <see cref="CategoryModel"/></returns>
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CategoryModel))]
        public async Task<IActionResult> Get(int id)
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
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CategoryModel))]
        public async Task<IActionResult> Delete(int id)
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
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CategoryModel))]
        public async Task<IActionResult> Post(CreateCategoryModel categoryModel)
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
        [HttpPut, Authorize(Roles = "Manager")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CategoryModel))]
        public async Task<IActionResult> Put([FromBody] UpdateCategoryModel categoryModel)
        {
            var result = await _mediator.Send(new PutCategoryRequest { CategoryModel = categoryModel });
            return Ok(result);
        }
    }
}