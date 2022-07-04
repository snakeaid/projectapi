using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    public class PutProductRequest : IRequest<ProductModel>
    {
        public int Id { get; set; }
        public ProductModel ProductModel { get; set; }
    }
}