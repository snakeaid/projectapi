using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    /// <summary>
    /// This class represents a MediatR request for category deletion and implements
    /// <see cref="IRequest{TResponse}"/> for <see cref="CategoryModel"/>.
    /// </summary>
    public class DeleteCategoryRequest : IRequest<CategoryModel>
    {
        /// <summary>
        /// Gets and sets the unique identifier of the category being deleted.
        /// </summary>
        public int Id { get; set; }
    }
}