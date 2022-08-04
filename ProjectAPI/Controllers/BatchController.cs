using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

        /// <summary>
        /// Constructs an instance of <see cref="BatchController"/> using the specified logger.
        /// </summary>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="BatchController"/>.</param>
        public BatchController(ILogger<BatchController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handles the HTTP POST request to upload a csv/json file.
        /// </summary>
        /// <returns><see cref="IActionResult"/></returns>
        [HttpPost("upload"), Authorize(Roles = "Manager")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Guid))]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            //var formCollection = await Request.ReadFormAsync();
            //var file = formCollection.Files.First();
            var folderName = Path.Combine("Uploads");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (file.Length > 0)
            {
                var fullFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fileType = fullFileName.Split('.').Last();
                if (fileType != "csv" || fileType != "json")
                {
                    return BadRequest(new { errorMessage = "File type must be either json or csv." });
                }
                var batchId = Guid.NewGuid();
                var fileName = batchId + "." + fileType;
                var fullPath = Path.Combine(pathToSave, fileName);
                //var dbPath = Path.Combine(folderName, fileName);
                await using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return Ok(batchId);
            }
        
            return BadRequest();
        }
        
        /// <summary>
        /// Handles the HTTP GET request to check the batch processing status.
        /// </summary>
        /// <returns><see cref="IActionResult"/></returns>
        [HttpGet("{batchId}")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Guid))]
        public async Task<IActionResult> Upload(int batchId)
        {
            //var formCollection = await Request.ReadFormAsync();
            //var file = formCollection.Files.First();
            
            return Ok();
        }
    }
}