using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    /// <summary>
    /// This class represents a MediatR request for product deletion and implements
    /// <see cref="IRequest{TResponse}"/> for <see cref="ProductModel"/>.
    /// </summary>
    public class DeleteProductRequest : IRequest<ProductModel>
    {
        /// <summary>
        /// Gets and sets the unique identifier of the product being deleted.
        /// </summary>
        public int Id { get; set; }
    }
}