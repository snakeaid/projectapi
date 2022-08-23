using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectAPI.CategoryService;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Mapping;
using ProjectAPI.ModelValidation;
using ProjectAPI.Primitives;
using Xunit;

namespace ProjectAPI.Tests
{
    public class CreateCategoryModelConsumerTests : IAsyncDisposable
    {
        private IRequestClient<CreateCategoryModel> client;
        private string databaseName;
        private ITestHarness harness;

        private IMapper mockMapper;
        private ServiceProvider provider;

        public CreateCategoryModelConsumerTests()
        {
            databaseName = Guid.NewGuid().ToString();
            var context = mockContext;
            context.Categories.Add(
                new Category { Id = 2, Name = "Test category 2", Description = "Test description 2" });
            context.Categories.Add(
                new Category { Id = 3, Name = "Test category 3", Description = "Test description 3" });
            context.SaveChanges();
            context.Dispose();

            var allMappersProfile = new AllMappersProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(allMappersProfile));
            mockMapper = new Mapper(configuration);
        }

        private CatalogContext mockContext
        {
            get
            {
                var options = new DbContextOptionsBuilder<CatalogContext>().UseInMemoryDatabase(databaseName).Options;
                return new CatalogContext(options);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await harness.Stop();
            await provider.DisposeAsync();
            await mockContext.Database.EnsureDeletedAsync();
            await mockContext.DisposeAsync();
            databaseName = String.Empty;
        }

        private async Task ArrangeMT()
        {
            provider = new ServiceCollection()
                .AddDbContext<CatalogContext>(opts => opts.UseInMemoryDatabase(databaseName))
                .AddAutoMapper(typeof(AllMappersProfile))
                .AddSingleton<ILogger, NullLogger<CreateCategoryModelConsumer>>()
                .AddScoped<IValidator<CreateCategoryModel>, CreateCategoryModelValidator>()
                .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<CreateCategoryModelConsumer>(); })
                .BuildServiceProvider(true);

            harness = provider.GetRequiredService<ITestHarness>();

            await harness.Start();

            client = harness.GetRequestClient<CreateCategoryModel>();
        }

        [Fact]
        public async Task CreateCategoryModelConsumer_AddsCategory_IfValidModelPassed()
        {
            //Arrange
            await ArrangeMT();
            var model = new CreateCategoryModel
            {
                Name = "string", Description = "string",
                Specifications = new List<string> { "Spec 1", "Spec 3", "Spec 5" }
            };

            //Act
            var countBefore = await mockContext.Categories.CountAsync();
            await client.GetResponse<CategoryModel>(model);
            var countAfter = await mockContext.Categories.CountAsync();

            //Assert
            Assert.Equal(countBefore + 1, countAfter);
            Assert.Equal(model.Name, mockContext.Categories.LastOrDefault().Name);
            Assert.Equal(model.Description, mockContext.Categories.LastOrDefault().Description);
            Assert.Equal(model.Specifications, mockContext.Categories.LastOrDefault().Specifications);
        }

        [Fact]
        public async Task CreateCategoryModelConsumer_ReturnsCategory_IfValidModelPassed()
        {
            //Arrange
            await ArrangeMT();
            var model = new CreateCategoryModel
            {
                Name = "string", Description = "string",
                Specifications = new List<string> { "Spec 1", "Spec 3", "Spec 5" }
            };

            //Act
            var result = (await client.GetResponse<CategoryModel>(model)).Message;

            var expectedJson =
                JsonSerializer.Serialize(mockMapper.Map<CategoryModel>(mockContext.Categories.LastOrDefault()));
            var actualJson = JsonSerializer.Serialize(result);

            //Assert
            Assert.Equal(expectedJson, actualJson);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("1", "123456")]
        [InlineData("123", "1")]
        public async Task CreateCategoryModelConsumer_ThrowsArgumentException_IfInvalidModelPassed(string name,
            string description)
        {
            //Arrange
            await ArrangeMT();
            var model = new CreateCategoryModel { Name = name, Description = description };

            //Act

            //Assert
            var ex = await Assert.ThrowsAsync<RequestFaultException>(async () =>
                await client.GetResponse<CategoryModel>(model));
            var exType = ex.Fault.Exceptions.FirstOrDefault().ExceptionType.Split('.')
                .LastOrDefault();
            var exMessage = ex.Fault.Exceptions.FirstOrDefault().Message;
            Assert.Equal("ArgumentException", exType);
            Assert.Contains("must", exMessage);
        }
    }
}