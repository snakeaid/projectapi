using System;
using MediatR;
using Microsoft.AspNetCore.Http;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    /// <summary>
    /// This class represents a MediatR request to make a batch upload and implements
    /// <see cref="IRequest{TResponse}"/> for <see cref="Guid"/>.
    /// </summary>
    public class BatchUploadRequest : IRequest<Guid>
    {
        /// <summary>
        /// Gets and sets the provided file.
        /// </summary>
        public IFormFile File { get; set; }
        
        /// <summary>
        /// Gets and sets the type of entities to be uploaded.
        /// </summary>
        public BatchUploadType Type { get; set; }
    }
}