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
    public class GetCategoryHandlerTests : IDisposable
    {
        private readonly CatalogContext mockContext;
        private readonly ILogger<GetCategoryHandler> mockLogger;
        private readonly Mapper mockMapper;
        private GetCategoryHandler handler;
        private GetCategoryRequest request;

        public GetCategoryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<CatalogContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            mockContext = new CatalogContext(options);
            mockContext.Categories.Add(new Category
                { Id = 2, Name = "Test category 2", Description = "Test description 2" });
            mockContext.Categories.Add(new Category
                { Id = 3, Name = "Test category 3", Description = "Test description 3" });
            mockContext.SaveChanges();

            mockLogger = new Mock<ILogger<GetCategoryHandler>>().Object;

            var allMappersProfile = new AllMappersProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(allMappersProfile));
            mockMapper = new Mapper(configuration);

            handler = new GetCategoryHandler(mockContext, mockMapper, mockLogger);
        }

        public void Dispose()
        {
            mockContext.Database.EnsureDeleted();
            mockContext.Dispose();
        }

        [Fact]
        public async Task GetCategoryHandler_ReturnsCategory_IfPassedIdExists()
        {
            //Arrange
            request = new GetCategoryRequest { Id = 3 };

            //Act
            var result = await handler.Handle(request, default);

            var expectedJson =
                JsonSerializer.Serialize(
                    mockMapper.Map<CategoryModel>(mockContext.Categories.FirstOrDefault(x => x.Id == request.Id)));
            var actualJson = JsonSerializer.Serialize(result);

            //Assert
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task GetCategoryHandler_ThrowsKeyNotFoundException_IfPassedIdDoesntExist()
        {
            //Arrange
            request = new GetCategoryRequest { Id = 5 };

            //Act

            //Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await handler.Handle(request, default));
            Assert.Equal($"Category {request.Id} NOT FOUND", ex.Message);
        }
    }
}