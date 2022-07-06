using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    /// <summary>
    /// This class represents a MediatR request to update a category and implements
    /// <see cref="IRequest{TResponse}"/> for <see cref="Primitives.CategoryModel"/>.
    /// </summary>
    public class PutCategoryRequest : IRequest<CategoryModel>
    {
        /// <summary>
        /// Gets and sets the unique identifier of the category to be updated.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets and sets the <see cref="Primitives.CategoryModel"/> model instance for the category which is to be updated.
        /// </summary>
        public CategoryModel CategoryModel { get; set; }
    }
}