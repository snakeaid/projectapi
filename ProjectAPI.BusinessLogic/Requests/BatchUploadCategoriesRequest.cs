using System;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ProjectAPI.BusinessLogic.Requests
{
    public class BatchUploadCategoriesRequest : IRequest<Guid>
    {
        public IFormFile File { get; set; }
    }
}