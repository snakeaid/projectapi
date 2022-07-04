using MediatR;
using ProjectAPI.DataAccess.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    public class DeleteProductRequest : IRequest<Product>
    {
        public int Id { get; set; }
    }
}