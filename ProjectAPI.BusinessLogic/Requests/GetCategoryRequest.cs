using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
	public class GetCategoryRequest : IRequest<CategoryModel>
    {
        public int Id { get; set; }
    }
}