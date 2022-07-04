using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    public class PostCategoryRequest : IRequest<CategoryModel>
    {
        public CategoryModel CategoryModel { get; set; }
    }
}