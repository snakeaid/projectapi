using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    /// <summary>
    /// This class represents a MediatR request to update a product and implements
    /// <see cref="IRequest{TResponse}"/> for <see cref="Primitives.ProductModel"/>.
    /// </summary>
    public class PutProductRequest : IRequest<ProductModel>
    {
        /// <summary>
        /// Gets and sets the unique identifier of the product to be updated.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets and sets the <see cref="Primitives.ProductModel"/> model instance for the product which is to be updated.
        /// </summary>
        public ProductModel ProductModel { get; set; }
    }
}