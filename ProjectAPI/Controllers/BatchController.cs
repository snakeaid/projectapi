using System;
using System.Net;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        /// <summary>
        /// An instance of <see cref="ILogger"/> which is used for logging.
        /// </summary>
        private readonly ILogger _logger;

        private readonly IMediator _mediator;

        /// <summary>
        /// Constructs an instance of <see cref="BatchController"/> using the specified logger.
        /// </summary>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="BatchController"/>.</param>
        public BatchController(ILogger<BatchController> logger, IMediator mediator)
        {
            _logger = logger;
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
            var result = await _mediator.Send(new BatchUploadRequest { File = file, Type = CatalogEntityType.Category});
            return Ok(result);
        }

        /// <summary>
        /// Handles the HTTP GET request to check the batch processing status.
        /// </summary>
        /// <returns><see cref="IActionResult"/></returns>
        [HttpGet("{batchId}")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Guid))]
        public async Task<IActionResult> Status(Guid batchId)
        {
            //TODO add status check
            var result = await _mediator.Send(new GetUploadStatusRequest { Id = batchId } );
            
            return Ok(result);
        }
    }
}