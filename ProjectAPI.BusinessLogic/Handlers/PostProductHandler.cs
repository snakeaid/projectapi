using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler to post a product and implements
    /// <see cref="IRequestHandler{TRequest,TResponse}"/> for
    /// <see cref="PostProductHandler"/>, <see cref="ProductModel"/>.
    /// </summary>
    public class PostProductHandler : IRequestHandler<PostProductRequest, ProductModel>
    {
        private readonly IRequestClient<CreateProductModel> _client;

        /// <summary>
        /// Constructs an instance of <see cref="PostProductHandler"/> using the request client.
        /// </summary>
        /// <param name="client">An instance of <see cref="IRequestClient{TRequest}"/>
        /// for <see cref="CreateProductModel"/>.</param>
        public PostProductHandler(IRequestClient<CreateProductModel> client)
        {
            _client = client;
        }

        /// <summary>
        /// Handles the specified request to post a product.
        /// </summary>
        /// <param name="request">An instance of <see cref="PostProductRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ProductModel"/></returns>
        public async Task<ProductModel> Handle(PostProductRequest request, CancellationToken cancellationToken)
        {
            var productModel = request.ProductModel;
            var response = await _client.GetResponse<ProductModel>(productModel);
            return response.Message;
        }
    }
}