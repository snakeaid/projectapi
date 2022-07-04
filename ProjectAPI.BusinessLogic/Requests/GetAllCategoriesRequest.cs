using MediatR;
using ProjectAPI.Primitives;
using System.Collections.Generic;

namespace ProjectAPI.BusinessLogic.Requests
{
    public class GetAllCategoriesRequest : IRequest<List<CategoryModel>>
    {
        
    }
}