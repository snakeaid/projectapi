using MediatR;
using System.Collections.Generic;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    /// <summary>
    /// This class represents a MediatR request to get all categories and implements
    /// <see cref="IRequest{TResponse}"/> for <see cref="List{T}"/> of <see cref="ProductModel"/>.
    /// </summary>
	public class GetAllProductsRequest : IRequest<List<ProductModel>>
    {
        
    }
}