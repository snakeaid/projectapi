using MediatR;
using ProjectAPI.Primitives;
using System.Collections.Generic;

namespace ProjectAPI.BusinessLogic.Requests
{
    /// <summary>
    /// This class represents a MediatR request to get all categories and implements
    /// <see cref="IRequest{TResponse}"/> for <see cref="List{T}"/> of <see cref="CategoryModel"/>.
    /// </summary>
    public class GetAllCategoriesRequest : IRequest<List<CategoryModel>>
    {
        
    }
}