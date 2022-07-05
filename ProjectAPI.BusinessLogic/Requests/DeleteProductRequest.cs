﻿using MediatR;
using ProjectAPI.Primitives;

namespace ProjectAPI.BusinessLogic.Requests
{
    public class DeleteProductRequest : IRequest<ProductModel>
    {
        public int Id { get; set; }
    }
}