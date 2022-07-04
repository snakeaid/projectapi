using MediatR;
using System.Collections.Generic;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
	public class GetAllProductsRequest : IRequest<List<ProductModel>>
    {
        public int Id { get; set; }
    }
}