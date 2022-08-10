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
    public class GetAllCategoriesHandlerTests : IDisposable
    {
        private readonly CatalogContext mockContext;
        private readonly ILogger<GetAllCategoriesHandler> mockLogger;
        private readonly Mapper mockMapper;
        private GetAllCategoriesHandler handler;
        private GetAllCategoriesRequest request;

        public GetAllCategoriesHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CatalogContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            mockContext = new CatalogContext(options);

            mockLogger = new Mock<ILogger<GetAllCategoriesHandler>>().Object;

            var allMappersProfile = new AllMappersProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(allMappersProfile));
            mockMapper = new Mapper(configuration);

            handler = new GetAllCategoriesHandler(mockContext, mockMapper, mockLogger);
        }

        public void Dispose()
        {
            mockContext.Database.EnsureDeleted();
            mockContext.Dispose();
        }

        [Fact]
        public async Task GetAllCategoriesHandler_ReturnsOneCategory_IfContextIsEmpty()
        {
            //Arrange
            request = new GetAllCategoriesRequest { };

            //Act
            var result = await handler.Handle(request, default);

            //Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllCategoriesHandler_ReturnsAllCategories_IfAllOk()
        {
            //Arrange
            mockContext.Categories.Add(
                new Category { Id = 2, Name = "Test category", Description = "Test description" });
            request = new GetAllCategoriesRequest { };

            //Act
            var result = await handler.Handle(request, default);

            var expectedJson = JsonSerializer.Serialize(mockMapper.Map<List<CategoryModel>>(mockContext.Categories));
            var actualJson = JsonSerializer.Serialize(result);

            //Assert
            Assert.Equal(expectedJson, actualJson);
        }
    }
}