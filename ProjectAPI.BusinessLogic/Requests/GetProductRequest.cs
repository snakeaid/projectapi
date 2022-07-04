using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
	public class GetProductRequest : IRequest<ProductModel>
    {
        public int Id { get; set; }
    }
}