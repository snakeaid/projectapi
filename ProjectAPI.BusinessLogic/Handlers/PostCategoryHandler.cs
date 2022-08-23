using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler to post a category and implements
    /// <see cref="IRequestHandler{TRequest,TResponse}"/> for
    /// <see cref="PostCategoryHandler"/>, <see cref="CategoryModel"/>.
    /// </summary>
    public class PostCategoryHandler : IRequestHandler<PostCategoryRequest, CategoryModel>
    {
        private readonly IRequestClient<CreateCategoryModel> _client;

        /// <summary>
        /// Constructs an instance of <see cref="PostCategoryHandler"/> using the request client.
        /// </summary>
        /// <param name="client">An instance of <see cref="IRequestClient{TRequest}"/>
        /// for <see cref="CreateCategoryModel"/>.</param>
        public PostCategoryHandler(IRequestClient<CreateCategoryModel> client)
        {
            _client = client;
        }

        /// <summary>
        /// Handles the specified request to post a category.
        /// </summary>
        /// <param name="request">An instance of <see cref="PostCategoryRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="CategoryModel"/></returns>
        public async Task<CategoryModel> Handle(PostCategoryRequest request, CancellationToken cancellationToken)
        {
            var categoryModel = request.CategoryModel;
            var response = await _client.GetResponse<CategoryModel>(categoryModel);
            return response.Message;
        }
    }
}