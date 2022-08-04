using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectAPI.BusinessLogic.Handlers;
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Mapping;
using ProjectAPI.ModelValidation;
using ProjectAPI.Primitives;
using Xunit;

namespace ProjectAPI.Tests
{
	public class PostCategoryHandlerTests : IDisposable
	{
		private readonly CatalogContext mockContext;
		private readonly ILogger<PostCategoryHandler> mockLogger;
		private readonly Mapper mockMapper;
		private readonly IValidator<CreateCategoryModel> mockValidator;

		private PostCategoryRequest request;
		private PostCategoryHandler handler;

		public PostCategoryHandlerTests()
		{
			var options = new DbContextOptionsBuilder<CatalogContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;
			mockContext = new CatalogContext(options);
			mockContext.Categories.Add(new Category { Id = 2, Name = "Test category 2", Description = "Test description 2" });
			mockContext.Categories.Add(new Category { Id = 3, Name = "Test category 3", Description = "Test description 3" });
			mockContext.SaveChanges();

			mockLogger = new Mock<ILogger<PostCategoryHandler>>().Object;

			var allMappersProfile = new AllMappersProfile();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(allMappersProfile));
			mockMapper = new Mapper(configuration);

			mockValidator = new CreateCategoryModelValidator();

			handler = new PostCategoryHandler(mockContext, mockMapper, mockLogger, mockValidator);
		}

		public void Dispose()
		{
			mockContext.Database.EnsureDeleted();
			mockContext.Dispose();
		}

		[Fact]
		public async Task PostCategoryHandler_AddsCategory_IfValidModelPassed()
		{
			//Arange
			request = new PostCategoryRequest { CategoryModel = new CreateCategoryModel { Name = "string", Description = "string", Specifications = new List<string> { "Spec 1", "Spec 3", "Spec 5" } } };

			//Act
			int countBefore = mockContext.Categories.Count();
			await handler.Handle(request, default);
			int countAfter = mockContext.Categories.Count();

			//Assert
			Assert.Equal(countBefore + 1, countAfter);
			Assert.Equal(request.CategoryModel.Name, mockContext.Categories.LastOrDefault().Name);
			Assert.Equal(request.CategoryModel.Description, mockContext.Categories.LastOrDefault().Description);
			Assert.Equal(request.CategoryModel.Specifications, mockContext.Categories.LastOrDefault().Specifications);
		}

		[Fact]
		public async Task PostCategoryHandler_ReturnsCategory_IfValidModelPassed()
		{
			//Arange
			request = new PostCategoryRequest { CategoryModel = new CreateCategoryModel { Name = "string", Description = "string", Specifications = new List<string> { "Spec 1", "Spec 3", "Spec 5" } } };

			//Act
			var result = await handler.Handle(request, default);

			var expectedJson = JsonSerializer.Serialize(mockMapper.Map<CategoryModel>(mockContext.Categories.LastOrDefault()));
			var actualJson = JsonSerializer.Serialize(result);

			//Assert
			Assert.Equal(expectedJson, actualJson);
		}

		[Theory]
		[InlineData("", "")]
		[InlineData("1", "123456")]
		[InlineData("123", "1")]
		public async Task PostCategoryHandler_ThrowsAgrumentException_IfInvalidModelPassed(string name, string description)
		{
			//Arange
			request = new PostCategoryRequest { CategoryModel = new CreateCategoryModel { Name = name, Description = description } };

			//Act

			//Assert
			var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(request, default));
			Assert.Contains("must", ex.Message);
		}
	}
}

