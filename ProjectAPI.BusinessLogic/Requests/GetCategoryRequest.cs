using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    /// <summary>
    /// This class represents a MediatR request to get a category and implements
    /// <see cref="IRequest{TResponse}"/> for <see cref="CategoryModel"/>.
    /// </summary>
	public class GetCategoryRequest : IRequest<CategoryModel>
    {
        /// <summary>
        /// Gets and sets the unique identifier of the desired category.
        /// </summary>
        public int Id { get; set; }
    }
}