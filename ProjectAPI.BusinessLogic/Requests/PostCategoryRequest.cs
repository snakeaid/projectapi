using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    /// <summary>
    /// This class represents a MediatR request to post a category and implements
    /// <see cref="IRequest{TResponse}"/> for <see cref="Primitives.CategoryModel"/>.
    /// </summary>
    public class PostCategoryRequest : IRequest<CategoryModel>
    {
        /// <summary>
        /// Gets and sets the <see cref="Primitives.CategoryModel"/> model instance for the category which is to be posted.
        /// </summary>
        public CategoryModel CategoryModel { get; set; }
    }
}