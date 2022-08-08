using System;
using ProjectAPI.Primitives;

namespace ProjectAPI.DataAccess.Primitives
{
    /// <summary>
    /// Upload request entity class.
    /// </summary>
    public class UploadRequest
    {
        /// <summary>
        /// Gets and sets the unique identifier of the product.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets and sets the type of model in the uploaded file.
        /// </summary>
        public CatalogEntityType Type { get; set; }
        
        /// <summary>
        /// Gets and sets file contents encoded to a base64 string.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Gets and sets the status of the upload request.
        /// </summary>
        public string Status { get; set; }
    }
}