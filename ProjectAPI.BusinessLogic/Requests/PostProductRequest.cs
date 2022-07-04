using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    public class PostProductRequest : IRequest<ProductDTO>
    {
        public ProductDTO ProductDTO { get; set; }
    }
}