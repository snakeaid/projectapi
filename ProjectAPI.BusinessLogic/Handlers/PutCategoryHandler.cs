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
    /// This class represents a MediatR request handler to update a category and implements
    /// <see cref="IRequestHandler{TRequest,TResponse}"/> for
    /// <see cref="PutCategoryRequest"/>, <see cref="CategoryModel"/>.
    /// </summary>
    public class PutCategoryHandler : IRequestHandler<PutCategoryRequest, CategoryModel>
    {
        private readonly IRequestClient<UpdateCategoryModel> _client;

        /// <summary>
        /// Constructs an instance of <see cref="PutCategoryHandler"/> using the request client.
        /// </summary>
        /// <param name="client">An instance of <see cref="IRequestClient{TRequest}"/>
        /// for <see cref="UpdateCategoryModel"/>.</param>
        public PutCategoryHandler(IRequestClient<UpdateCategoryModel> client)
        {
            _client = client;
        }

        /// <summary>
        /// Handles the specified request to update a category.
        /// </summary>
        /// <param name="request">An instance of <see cref="PutCategoryRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Task{TResult}"/> for <see cref="CategoryModel"/></returns>
        /// <exception cref="ArgumentException">Thrown if the provided model of the category is invalid.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if there is no category found by the specified identifier.</exception>
        public async Task<CategoryModel> Handle(PutCategoryRequest request, CancellationToken cancellationToken)
        {
            var categoryModel = request.CategoryModel;
            var response = await _client.GetResponse<CategoryModel>(categoryModel);
            return response.Message;
        }
    }
}