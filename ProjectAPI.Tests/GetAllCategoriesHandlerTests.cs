using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.BusinessLogic.Handlers;
using ProjectAPI.Mapping;
using ProjectAPI.DataAccess;
using ProjectAPI.Primitives;
using ProjectAPI.DataAccess.Primitives;

namespace ProjectAPI.Tests
{
	public class GetAllCategoriesHandlerTests : IDisposable
	{
		private readonly CatalogContext mockContext;
		private readonly ILogger<GetAllCategoriesHandler> mockLogger;
		private readonly Mapper mockMapper;
		private GetAllCategoriesRequest request;
		private GetAllCategoriesHandler handler;

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
			//Arange
			request = new GetAllCategoriesRequest { };

			//Act
			var result = await handler.Handle(request, default);

			//Assert
			Assert.Single(result);
		}

		[Fact]
		public async Task GetAllCategoriesHandler_ReturnsAllCategories_IfAllOk()
		{
			//Arange
			mockContext.Categories.Add(new Category { Id = 2, Name = "Test category", Description = "Test description" });
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

