using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler for product deletion and implements
    /// <see cref="IRequestHandler{TRequest,TResponse}"/> for
    /// <see cref="DeleteProductRequest"/>, <see cref="ProductModel"/>.
    /// </summary>
    public class DeleteProductHandler : IRequestHandler<DeleteProductRequest, ProductModel>
    {
        private readonly IRequestClient<DeleteProductModel> _client;

        /// <summary>
        /// Constructs an instance of <see cref="DeleteProductHandler"/> using the request client.
        /// </summary>
        /// <param name="client">An instance of <see cref="IRequestClient{TRequest}"/>
        /// for <see cref="DeleteProductModel"/>.</param>
        public DeleteProductHandler(IRequestClient<DeleteProductModel> client)
        {
            _client = client;
        }

        /// <summary>
        /// Handles the specified request for product deletion.
        /// </summary>
        /// <param name="request">An instance of <see cref="DeleteProductRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ProductModel"/></returns>
        public async Task<ProductModel> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
        {
            var productModel = new DeleteProductModel { Id = request.Id };
            var response = await _client.GetResponse<ProductModel>(productModel);
            return response.Message;
        }
    }
}