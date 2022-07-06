using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    /// <summary>
    /// This class represents a MediatR request to post a product and implements
    /// <see cref="IRequest{TResponse}"/> for <see cref="Primitives.ProductModel"/>.
    /// </summary>
    public class PostProductRequest : IRequest<ProductModel>
    {
        /// <summary>
        /// Gets and sets the <see cref="Primitives.ProductModel"/> model instance for the product which is to be posted.
        /// </summary>
        public ProductModel ProductModel { get; set; }
    }
}