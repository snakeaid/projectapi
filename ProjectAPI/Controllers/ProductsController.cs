using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using ProjectAPI.Primitives;
using ProjectAPI.BusinessLogic.Requests;

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public ProductsController(ILogger<ProductsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
            //тут было автоматическое добавление категории и продукта если БД пустая
        }

        //GET api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> Get()
        {
            var result = await _mediator.Send(new GetAllProductsRequest());

            return Ok(result);
        }

        //GET api/products/id
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> Get(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetProductRequest { Id = id });
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        //DELETE api/products/id
        [HttpDelete("{id}"), Authorize(Roles = "Manager")]
        public async Task<ActionResult<ProductModel>> Delete(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteProductRequest { Id = id });
                return Ok(result);
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }
        }

        //POST api/products/
        [HttpPost, Authorize(Roles = "Manager")]
        public async Task<ActionResult<ProductModel>> Post([FromBody]ProductModel ProductModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Given product is invalid");
                return BadRequest(ModelState);
            }
            var result = await _mediator.Send(new PostProductRequest { ProductModel = ProductModel });

            return Ok(result);
        }

        //PUT api/products/id
        [HttpPut("{id}"), Authorize(Roles = "Manager")]
        public async Task<ActionResult<ProductModel>> Put(int id, [FromBody] ProductModel productModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Given product is invalid");
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _mediator.Send(new PutProductRequest { Id = id, ProductModel = productModel });
                return Ok(result);
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}