using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Handlers
{
    /// <summary>
    /// This class represents a MediatR request handler to update a product and implements
    /// <see cref="IRequestHandler{TRequest,TResponse}"/> for
    /// <see cref="PutProductRequest"/>, <see cref="ProductModel"/>.
    /// </summary>
    public class PutProductHandler : IRequestHandler<PutProductRequest, ProductModel>
    {
        private readonly IRequestClient<UpdateProductModel> _client;

        /// <summary>
        /// Constructs an instance of <see cref="PutProductHandler"/> using the request client.
        /// </summary>
        /// <param name="client">An instance of <see cref="IRequestClient{TRequest}"/>
        /// for <see cref="UpdateProductModel"/>.</param>
        public PutProductHandler(IRequestClient<UpdateProductModel> client)
        {
            _client = client;
        }

        /// <summary>
        /// Handles the specified request to update a product.
        /// </summary>
        /// <param name="request">An instance of <see cref="PutProductRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="ProductModel"/></returns>
        /// <exception cref="ArgumentException">Thrown if the provided model of the product is invalid.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if there is no product found by the specified identifier.</exception>
        public async Task<ProductModel> Handle(PutProductRequest request, CancellationToken cancellationToken)
        {
            var productModel = request.ProductModel;
            var response = await _client.GetResponse<ProductModel>(productModel);
            return response.Message;
        }
    }
}