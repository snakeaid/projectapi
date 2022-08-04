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
	public class DeleteProductHandlerTests : IDisposable
	{
		private readonly CatalogContext mockContext;
		private readonly ILogger<DeleteProductHandler> mockLogger;
		private readonly Mapper mockMapper;
		private DeleteProductRequest request;
		private DeleteProductHandler handler;

		public DeleteProductHandlerTests()
		{
			var options = new DbContextOptionsBuilder<CatalogContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;
			mockContext = new CatalogContext(options);
			mockContext.Products.Add(new Product { Id = 2, Name = "Test Product 2", Description = "Test description 2", CategoryId = 1 });
			mockContext.Products.Add(new Product { Id = 3, Name = "Test Product 3", Description = "Test description 3", CategoryId = 1 });
			mockContext.SaveChanges();

			mockLogger = new Mock<ILogger<DeleteProductHandler>>().Object;

			var allMappersProfile = new AllMappersProfile();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(allMappersProfile));
			mockMapper = new Mapper(configuration);

			handler = new DeleteProductHandler(mockContext, mockMapper, mockLogger);
		}

		public void Dispose()
		{
			mockContext.Database.EnsureDeleted();
			mockContext.Dispose();
		}

		[Fact]
		public async Task DeleteProductHandler_DeletesProduct_IfPassedIdExists()
		{
			//Arange
			request = new DeleteProductRequest { Id = 3 };

			//Act
			var result = await handler.Handle(request, default);

			//Assert
			Assert.NotNull(mockContext.Products.IgnoreQueryFilters().FirstOrDefault(x => x.Id == request.Id).DateDeleted);
		}

		[Fact]
		public async Task DeleteProductHandler_ReturnsProduct_IfIdExists()
		{
			//Arange
			request = new DeleteProductRequest { Id = 3 };

			//Act
			var result = await handler.Handle(request, default);

			var expectedJson = JsonSerializer.Serialize(mockMapper.Map<ProductModel>(mockContext.Products.IgnoreQueryFilters().FirstOrDefault(c => c.Id == request.Id)));
			var actualJson = JsonSerializer.Serialize(result);

			//Assert
			Assert.Equal(expectedJson, actualJson);
		}

		[Fact]
		public async Task DeleteProductHandler_ThrowsKeyNotFoundException_IfPassedIdDoesntExist()
		{
			//Arange
			request = new DeleteProductRequest { Id = 5 };

			//Act

			//Assert
			var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await handler.Handle(request, default));
			Assert.Equal($"Product {request.Id} NOT FOUND", ex.Message);
		}
	}
}

