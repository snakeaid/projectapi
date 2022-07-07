using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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

namespace ProjectAPI.Tests
{
	public class GetAllCategoriesHandlerTests
	{
		private readonly CatalogContext mockContext;
		private readonly ILogger<GetAllCategoriesHandler> mockLogger;
		private readonly Mapper mockMapper;

		public GetAllCategoriesHandlerTests()
        {
			var options = new DbContextOptionsBuilder<CatalogContext>()
				.UseInMemoryDatabase("TestDB")
				.Options;

			mockContext = new CatalogContext(options);

			mockLogger = new Mock<ILogger<GetAllCategoriesHandler>>().Object;

			var allMappersProfile = new AllMappersProfile();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(allMappersProfile));
			mockMapper = new Mapper(configuration);
		}

		[Fact]
		public async Task ReturnsListOfCategories()
        {
			//Arrange
			var request = new GetAllCategoriesRequest { };
			var handler = new GetAllCategoriesHandler(mockContext, mockMapper, mockLogger);

			//Act
			var result = await handler.Handle(request, default);

			//Assert
			Assert.NotEmpty(result);
		}
	}
}

