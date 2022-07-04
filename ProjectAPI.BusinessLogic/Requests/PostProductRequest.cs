using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    public class PostProductRequest : IRequest<ProductModel>
    {
        public ProductModel ProductModel { get; set; }
    }
}