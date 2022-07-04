using MediatR;
using System.Collections.Generic;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
	public class GetAllProductsRequest : IRequest<List<ProductDTO>>
    {
        public int Id { get; set; }
    }
}