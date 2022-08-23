using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler for category deletion and implements
    /// <see cref="IRequestHandler{TRequest,TResponse}"/> for
    /// <see cref="DeleteCategoryRequest"/>, <see cref="CategoryModel"/>.
    /// </summary>
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryRequest, CategoryModel>
    {
        private readonly IRequestClient<DeleteCategoryModel> _client;

        /// <summary>
        /// Constructs an instance of <see cref="DeleteCategoryHandler"/> using the request client.
        /// </summary>
        /// <param name="client">An instance of <see cref="IRequestClient{TRequest}"/>
        /// for <see cref="DeleteCategoryModel"/>.</param>
        public DeleteCategoryHandler(IRequestClient<DeleteCategoryModel> client)
        {
            _client = client;
        }

        /// <summary>
        /// Handles the specified request for category deletion.
        /// </summary>
        /// <param name="request">An instance of <see cref="DeleteCategoryRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="CategoryModel"/></returns>
        public async Task<CategoryModel> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
        {
            var productModel = new DeleteCategoryModel { Id = request.Id };
            var response = await _client.GetResponse<CategoryModel>(productModel);
            return response.Message;
        }
    }
}