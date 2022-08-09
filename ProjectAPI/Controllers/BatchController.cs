using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.Primitives;

namespace ProjectAPI.Controllers
{
    /// <summary>
    /// API controller class which controls batch upload functionality and derives from <see cref="ControllerBase"/>.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BatchController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructs an instance of <see cref="BatchController"/> using the specified mediator.
        /// </summary>
        public BatchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the HTTP POST request to upload a csv/json file with categories.
        /// </summary>
        /// <returns><see cref="IActionResult"/></returns>
        // [HttpPost("categories"), Authorize(Roles = "Manager")]
        // [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [HttpPost("categories"), AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Guid))]
        public async Task<IActionResult> UploadCategories(IFormFile file)
        {
            var result = await _mediator.Send(new BatchUploadRequest
                { File = file, Type = CatalogEntityType.Category });
            return Ok(result);
        }

        /// <summary>
        /// Handles the HTTP POST request to upload a csv/json file with products.
        /// </summary>
        /// <returns><see cref="IActionResult"/></returns>
        // [HttpPost("categories"), Authorize(Roles = "Manager")]
        // [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [HttpPost("products"), AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Guid))]
        public async Task<IActionResult> UploadProducts(IFormFile file)
        {
            var result = await _mediator.Send(new BatchUploadRequest { File = file, Type = CatalogEntityType.Product });
            return Ok(result);
        }

        /// <summary>
        /// Handles the HTTP GET request to check the batch processing status.
        /// </summary>
        /// <returns><see cref="IActionResult"/></returns>
        [HttpGet("{batchId}")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Guid))]
        public async Task<IActionResult> Status(Guid batchId)
        {
            var result = await _mediator.Send(new GetUploadStatusRequest { Id = batchId });
            return Ok(result);
        }
    }
}