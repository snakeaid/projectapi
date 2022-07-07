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
using Microsoft.AspNetCore.Mvc;
using ProjectAPI.BusinessLogic.Requests;
using ProjectAPI.BusinessLogic.Handlers;
using ProjectAPI.Mapping;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Primitives;
using ProjectAPI.ModelValidation;

namespace ProjectAPI.Tests
{
	public class PutCategoryHandlerTests : IDisposable
	{
		private readonly CatalogContext mockContext;
		private readonly ILogger<PutCategoryHandler> mockLogger;
		private readonly Mapper mockMapper;
		private readonly IValidator<CategoryModel> mockValidator;

		private PutCategoryRequest request;
		private PutCategoryHandler handler;

		public PutCategoryHandlerTests()
		{
			var options = new DbContextOptionsBuilder<CatalogContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;
			mockContext = new CatalogContext(options);
			mockContext.Categories.Add(new Category { Id = 2, Name = "Test category 2", Description = "Test description 2", Specifications=new List<string> { "Spec 1", "Spec 2", "Spec 3"}, Products=new List<Product> { new Product { Id=1, Name="AAA"} } });
			mockContext.Categories.Add(new Category { Id = 3, Name = "Test category 3", Description = "Test description 3" });
			mockContext.SaveChanges();

			mockLogger = new Mock<ILogger<PutCategoryHandler>>().Object;

			var allMappersProfile = new AllMappersProfile();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(allMappersProfile));
			mockMapper = new Mapper(configuration);

			mockValidator = new CategoryModelValidator();

			handler = new PutCategoryHandler(mockContext, mockMapper, mockLogger, mockValidator);
		}

		public void Dispose()
		{
			mockContext.Database.EnsureDeleted();
			mockContext.Dispose();
		}

        [Fact]
        public async Task PutCategoryHandler_UpdatesCategory_IfValidModelAndIdPassed()
        {
            //Arange
            request = new PutCategoryRequest { Id=2, CategoryModel = new CategoryModel { Name = "string", Description = "string", Specifications = new List<string> { "Spec 1", "Spec 3", "Spec 5" } } };

            //Act
            await handler.Handle(request, default);

			//Assert
			Assert.NotNull(mockContext.Categories.FirstOrDefault(x => x.Id == request.Id).DateUpdated);
            Assert.Equal(request.CategoryModel.Name, mockContext.Categories.FirstOrDefault(x => x.Id==request.Id).Name);
            Assert.Equal(request.CategoryModel.Description, mockContext.Categories.FirstOrDefault(x => x.Id == request.Id).Description);
            Assert.Equal(request.CategoryModel.Specifications, mockContext.Categories.FirstOrDefault(x => x.Id == request.Id).Specifications);
        }

		[Fact]
		public async Task PutCategoryHandler_UpdatesAllProductSpecifications_IfValidModelAndIdPassed()
		{
			//Arange
			request = new PutCategoryRequest { Id = 2, CategoryModel = new CategoryModel { Name = "string", Description = "string", Specifications = new List<string> { "Spec 1", "Spec 3", "Spec 5" } } };

			//Act
			await handler.Handle(request, default);

			//Assert
			foreach (Product product in mockContext.Categories.Include(c => c.Products).FirstOrDefault(x => x.Id == request.Id).Products)
				Assert.Equal(request.CategoryModel.Specifications, product.SpecificationData.Keys);
		}

		[Fact]
        public async Task PutCategoryHandler_ReturnsCategory_IfValidModelAndIdPassed()
        {
            //Arange
            request = new PutCategoryRequest { Id = 2, CategoryModel = new CategoryModel { Name = "string", Description = "string" } };

            //Act
            var result = await handler.Handle(request, default);

            var expectedJson = JsonSerializer.Serialize(mockMapper.Map<CategoryModel>(mockContext.Categories.FirstOrDefault(x => x.Id == request.Id)));
            var actualJson = JsonSerializer.Serialize(result);

            //Assert
            Assert.Equal(expectedJson, actualJson);
        }

		[Fact]
		public async Task GetCategoryHandler_ThrowsKeyNotFoundException_IfPassedIdDoesntExist()
		{
			//Arange
			request = new PutCategoryRequest { Id = 5, CategoryModel = new CategoryModel { Name = "string", Description = "string" } };

			//Act

			//Assert
			var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await handler.Handle(request, default));
			Assert.Equal($"Category {request.Id} NOT FOUND", ex.Message);
		}

        [Theory]
        [InlineData("", "")]
        [InlineData("1", "123456")]
		[InlineData("123", "1")]
		public async Task PutCategoryHandler_ThrowsAgrumentException_IfInvalidModelPassedButPassedIdExists(string name, string description)
        {
            //Arange
            request = new PutCategoryRequest { Id=2, CategoryModel = new CategoryModel { Name = name, Description = description } };

			//Act

			//Assert
			var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(request, default));
			Assert.Contains("must", ex.Message);
		}
    }
}

