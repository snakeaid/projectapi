using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    public class PutProductRequest : IRequest<ProductDTO>
    {
        public int Id { get; set; }
        public ProductDTO ProductDTO { get; set; }
    }
}