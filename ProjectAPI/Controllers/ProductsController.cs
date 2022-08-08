using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using ProjectAPI.Primitives;
using ProjectAPI.BusinessLogic.Requests;

namespace ProjectAPI.Controllers
{
    /// <summary>
    /// API controller class which controls all HTTP requests related to operations with products.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructs an instance of <see cref="CategoriesController"/> using the specified mediator.
        /// </summary>
        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the HTTP GET request to get all products, invoked at /api/products route.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ActionResult{TValue}"/> for
        /// <see cref="IEnumerable{T}"/> of <see cref="ProductModel"/></returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ProductModel>))]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new GetAllProductsRequest());

            return Ok(result);
        }

        /// <summary>
        /// Handles the HTTP GET request to get the product with the specified identifier, invoked at
        /// /api/products/id route.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ActionResult{TValue}"/> for
        /// <see cref="ProductModel"/></returns>
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProductModel))]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _mediator.Send(new GetProductRequest { Id = id });
            return Ok(result);
        }

        /// <summary>
        /// Handles the HTTP DELETE request to delete the product with the specified identifier, invoked at
        /// /api/products/id route.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ActionResult{TValue}"/> for
        /// <see cref="ProductModel"/></returns>
        [HttpDelete("{id}"), Authorize(Roles = "Manager")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProductModel))]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteProductRequest { Id = id });
            return Ok(result);
        }

        /// <summary>
        /// Handles the HTTP POST request to post a new product, invoked at
        /// /api/products/ route.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ActionResult{TValue}"/> for
        /// <see cref="ProductModel"/></returns>
        [HttpPost, Authorize(Roles = "Manager")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ProductModel))]
        public async Task<IActionResult> Post([FromBody] CreateProductModel ProductModel)
        {
            var result = await _mediator.Send(new PostProductRequest { ProductModel = ProductModel });
            return Ok(result);
        }

        /// <summary>
        /// Handles the HTTP PUT request to update the product with the specified identifier, invoked at
        /// /api/products/id route.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ActionResult{TValue}"/> for
        /// <see cref="ProductModel"/></returns>
        [HttpPut("{id}"), Authorize(Roles = "Manager")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProductModel))]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateProductModel productModel)
        {
            var result = await _mediator.Send(new PutProductRequest { Id = id, ProductModel = productModel });
            return Ok(result);
        }
    }
}