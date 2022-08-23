using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectAPI.BusinessLogic.Handlers;
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Mapping;
using ProjectAPI.Primitives;
using Xunit;

namespace ProjectAPI.Tests
{
    public class GetAllProductsHandlerTests : IDisposable
    {
        private readonly CatalogContext mockContext;
        private readonly ILogger<GetAllProductsHandler> mockLogger;
        private readonly Mapper mockMapper;
        private GetAllProductsHandler handler;
        private GetAllProductsRequest request;

        public GetAllProductsHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CatalogContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            mockContext = new CatalogContext(options);

            mockLogger = new Mock<ILogger<GetAllProductsHandler>>().Object;

            var allMappersProfile = new AllMappersProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(allMappersProfile));
            mockMapper = new Mapper(configuration);

            handler = new GetAllProductsHandler(mockContext, mockMapper, mockLogger);
        }

        public void Dispose()
        {
            mockContext.Database.EnsureDeleted();
            mockContext.Dispose();
        }

        [Fact]
        public async Task GetAllProductsHandler_ReturnsAllProducts_IfAllOk()
        {
            //Arrange
            mockContext.Products.Add(new Product
                { Id = 2, Name = "Test Products", Description = "Test description", CategoryId = 1 });
            request = new GetAllProductsRequest { };

            //Act
            var result = await handler.Handle(request, default);

            var expectedJson = JsonSerializer.Serialize(mockMapper.Map<List<ProductModel>>(mockContext.Products));
            var actualJson = JsonSerializer.Serialize(result);

            //Assert
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task GetAllProductsHandler_ReturnsNoProducts_IfContextIsEmpty()
        {
            //Arrange
            request = new GetAllProductsRequest { };

            //Act
            var result = await handler.Handle(request, default);

            //Assert
            Assert.Empty(result);
        }
    }
}