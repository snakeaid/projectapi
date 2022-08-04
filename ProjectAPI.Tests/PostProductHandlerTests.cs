using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;
using Moq;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.BusinessLogic.Handlers;
using ProjectAPI.Mapping;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Primitives;
using ProjectAPI.ModelValidation;

namespace ProjectAPI.Tests
{
	public class PostProductHandlerTests : IDisposable
	{
		private readonly CatalogContext mockContext;
		private readonly ILogger<PostProductHandler> mockLogger;
		private readonly Mapper mockMapper;
		private readonly IValidator<CreateProductModel> mockValidator;

		private PostProductRequest request;
		private PostProductHandler handler;

		public PostProductHandlerTests()
		{
			var options = new DbContextOptionsBuilder<CatalogContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;
			mockContext = new CatalogContext(options);
			mockContext.Products.Add(new Product { Id = 2, Name = "Test Product 2", Description = "Test description 2", CategoryId = 1 });
			mockContext.Products.Add(new Product { Id = 3, Name = "Test Product 3", Description = "Test description 3", CategoryId = 1 });
			mockContext.SaveChanges();

			mockLogger = new Mock<ILogger<PostProductHandler>>().Object;

			var allMappersProfile = new AllMappersProfile();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(allMappersProfile));
			mockMapper = new Mapper(configuration);

			mockValidator = new CreateProductModelValidator();

			handler = new PostProductHandler(mockContext, mockMapper, mockLogger, mockValidator);
		}

		public void Dispose()
		{
			mockContext.Database.EnsureDeleted();
			mockContext.Dispose();
		}

		[Fact]
		public async Task PostProductHandler_AddsProduct_IfValidModelPassed()
		{
			//Arange
			request = new PostProductRequest { ProductModel = new CreateProductModel { Name = "string", Description = "string", CategoryId = 1, SpecificationData = new Dictionary<string, string> { { "Spec 1", "Value 1" }, { "Spec 2", "Value 2" } } } };

			//Act
			int countBefore = mockContext.Products.Count();
			await handler.Handle(request, default);
			int countAfter = mockContext.Products.Count();

			//Assert
			Assert.Equal(countBefore + 1, countAfter);
			Assert.Equal(request.ProductModel.Name, mockContext.Products.LastOrDefault().Name);
			Assert.Equal(request.ProductModel.Description, mockContext.Products.LastOrDefault().Description);
		}

		[Fact]
		public async Task PostProductHandler_ReturnsProduct_IfValidModelPassed()
		{
			//Arange
			request = new PostProductRequest { ProductModel = new CreateProductModel { Name = "string", Description = "string", CategoryId = 1, SpecificationData = new Dictionary<string, string> { { "Spec 1", "Value 1" }, { "Spec 2", "Value 2" } } } };

			//Act
			var result = await handler.Handle(request, default);

			var expectedJson = JsonSerializer.Serialize(mockMapper.Map<ProductModel>(mockContext.Products.LastOrDefault()));
			var actualJson = JsonSerializer.Serialize(result);

			//Assert
			Assert.Equal(expectedJson, actualJson);
		}

		[Fact]
		public async Task PostProductHandler_AssignsDefaultCategory_IfValidModelPassedButCategoryIdIsInvalid()
		{
			//Arange
			request = new PostProductRequest { ProductModel = new CreateProductModel { Name = "string", Description = "string", CategoryId = 5, SpecificationData = new Dictionary<string, string> { { "Spec 1", "Value 1" }, { "Spec 2", "Value 2" } } } };

			//Act
			var result = await handler.Handle(request, default);

			//Assert
			Assert.Equal(1, result.CategoryId);
		}

		[Fact]
		public async Task PostProductHandler_CoordinatesProductSpecifications_IfAllOk()
		{
			//Arange
			mockContext.Categories.Add(new Category { Id = 2, Name = "Category 2", Description = "Description 2", Specifications = new List<string> { "Spec 2", "Spec 3" } });
			mockContext.SaveChanges();
			request = new PostProductRequest { ProductModel = new CreateProductModel { Name = "string", Description = "string", CategoryId = 2, SpecificationData = new Dictionary<string, string> { { "Spec 1", "Value 1" }, { "Spec 2", "Value 2" } } } };

			//Act
			var result = await handler.Handle(request, default);

			//Assert
			Assert.Equal(mockContext.Categories.FirstOrDefault(x=>x.Id==result.CategoryId).Specifications,result.SpecificationData.Keys);
		}

		[Theory]
		[InlineData("", "", 2)]
		[InlineData("1", "123456", -1)]
		[InlineData("123", "1", 5)]
		public async Task PostProductHandler_ThrowsAgrumentException_IfInvalidModelPassed(string name, string description, int categoryId)
		{
			//Arange
			request = new PostProductRequest { ProductModel = new CreateProductModel { Name = name, Description = description, CategoryId = categoryId } };

			//Act

			//Assert
			var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(request, default));
			Assert.Contains("must", ex.Message);
		}
	}
}

