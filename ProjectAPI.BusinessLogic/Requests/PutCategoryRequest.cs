using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    public class PutCategoryRequest : IRequest<CategoryModel>
    {
        public int Id { get; set; }
        public CategoryModel CategoryModel { get; set; }
    }
}