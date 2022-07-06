using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    /// <summary>
    /// This class represents a MediatR request to get a product and implements
    /// <see cref="IRequest{TResponse}"/> for <see cref="ProductModel"/>.
    /// </summary>
	public class GetProductRequest : IRequest<ProductModel>
    {
        /// <summary>
        /// Gets and sets the unique identifier of the desired product.
        /// </summary>
        public int Id { get; set; }
    }
}