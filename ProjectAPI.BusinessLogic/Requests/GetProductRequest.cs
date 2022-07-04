using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
	public class GetProductRequest : IRequest<ProductDTO>
    {
        public int Id { get; set; }
    }
}