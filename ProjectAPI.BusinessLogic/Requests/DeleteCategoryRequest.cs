using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    public class DeleteCategoryRequest : IRequest<CategoryModel>
    {
        public int Id { get; set; }
    }
}