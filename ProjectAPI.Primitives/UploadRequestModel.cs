using System;

namespace ProjectAPI.Primitives
{
    /// <summary>
    /// Model class for the Upload Request entity which is used to display the request information.
    /// </summary>
    public class UploadRequestModel
    {
        /// <summary>
        /// Gets and sets the displayed unique identifier of the product.
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets and sets the displayed type of model in the uploaded file.
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Gets and sets the displayed status of the upload request.
        /// </summary>
        public string Status { get; set; }
    }
}