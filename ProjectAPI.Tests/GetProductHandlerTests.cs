using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GetProductHandlerTests : IDisposable
    {
        private readonly CatalogContext mockContext;
        private readonly ILogger<GetProductHandler> mockLogger;
        private readonly Mapper mockMapper;
        private GetProductHandler handler;
        private GetProductRequest request;

        public GetProductHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CatalogContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            mockContext = new CatalogContext(options);
            mockContext.Products.Add(new Product
                { Id = 2, Name = "Test Product 2", Description = "Test description 2", CategoryId = 1 });
            mockContext.Products.Add(new Product
                { Id = 3, Name = "Test Product 3", Description = "Test description 3", CategoryId = 1 });
            mockContext.SaveChanges();

            mockLogger = new Mock<ILogger<GetProductHandler>>().Object;

            var allMappersProfile = new AllMappersProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(allMappersProfile));
            mockMapper = new Mapper(configuration);

            handler = new GetProductHandler(mockContext, mockMapper, mockLogger);
        }

        public void Dispose()
        {
            mockContext.Database.EnsureDeleted();
            mockContext.Dispose();
        }

        [Fact]
        public async Task GetProductHandler_ReturnsProduct_IfPassedIdExists()
        {
            //Arrange
            request = new GetProductRequest { Id = 3 };

            //Act
            var result = await handler.Handle(request, default);

            var expectedJson =
                JsonSerializer.Serialize(
                    mockMapper.Map<ProductModel>(mockContext.Products.FirstOrDefault(x => x.Id == request.Id)));
            var actualJson = JsonSerializer.Serialize(result);

            //Assert
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task GetProductHandler_ThrowsKeyNotFoundException_IfPassedIdDoesntExist()
        {
            //Arrange
            request = new GetProductRequest { Id = 5 };

            //Act

            //Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await handler.Handle(request, default));
            Assert.Equal($"Product {request.Id} NOT FOUND", ex.Message);
        }
    }
}