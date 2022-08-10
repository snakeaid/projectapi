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
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.BusinessLogic.Handlers;
using ProjectAPI.Mapping;
using ProjectAPI.DataAccess;
using ProjectAPI.Primitives;
using ProjectAPI.DataAccess.Primitives;

namespace ProjectAPI.Tests
{
	public class GetAllProductsHandlerTests : IDisposable
	{
		private readonly CatalogContext mockContext;
		private readonly ILogger<GetAllProductsHandler> mockLogger;
		private readonly Mapper mockMapper;
		private GetAllProductsRequest request;
		private GetAllProductsHandler handler;

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
			//Arange
			mockContext.Products.Add(new Product { Id = 2, Name = "Test Products", Description = "Test description", CategoryId = 1 });
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
			//Arange
			request = new GetAllProductsRequest { };

			//Act
			var result = await handler.Handle(request, default);

			//Assert
			Assert.Empty(result);
		}
	}
}

