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
    public class UpdateCategoryModelConsumerTests : IAsyncDisposable
    {
        private IRequestClient<UpdateCategoryModel> client;
        private string databaseName;
        private ITestHarness harness;

        private IMapper mockMapper;
        private ServiceProvider provider;

        public UpdateCategoryModelConsumerTests()
        {
            databaseName = Guid.NewGuid().ToString();
            var context = mockContext;
            context.Categories.Add(new Category
            {
                Id = 2, Name = "Test category 2", Description = "Test description 2",
                Specifications = new List<string> { "Spec 1", "Spec 2", "Spec 3" },
                Products = new List<Product> { new Product { Id = 1, Name = "AAA" } }
            });
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
                .AddSingleton<ILogger, NullLogger<UpdateCategoryModelConsumer>>()
                .AddScoped<IValidator<UpdateCategoryModel>, UpdateCategoryModelValidator>()
                .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<UpdateCategoryModelConsumer>(); })
                .BuildServiceProvider(true);

            harness = provider.GetRequiredService<ITestHarness>();

            await harness.Start();

            client = harness.GetRequestClient<UpdateCategoryModel>();
        }

        [Fact]
        public async Task UpdateCategoryModelConsumer_UpdatesCategory_IfValidModelAndIdPassed()
        {
            //Arrange
            await ArrangeMT();
            var model = new UpdateCategoryModel
            {
                Id = 2, Name = "string", Description = "string",
                Specifications = new List<string> { "Spec 1", "Spec 3", "Spec 5" }
            };

            //Act
            var result = (await client.GetResponse<CategoryModel>(model)).Message;

            //Assert
            Assert.NotNull(mockContext.Categories.FirstOrDefault(x => x.Id == model.Id).DateUpdated);
            Assert.Equal(model.Name,
                mockContext.Categories.FirstOrDefault(x => x.Id == model.Id).Name);
            Assert.Equal(model.Description,
                mockContext.Categories.FirstOrDefault(x => x.Id == model.Id).Description);
            Assert.Equal(model.Specifications,
                mockContext.Categories.FirstOrDefault(x => x.Id == model.Id).Specifications);
        }

        [Fact]
        public async Task UpdateCategoryModelConsumer_UpdatesAllProductSpecifications_IfValidModelAndIdPassed()
        {
            //Arrange
            await ArrangeMT();
            var model = new UpdateCategoryModel
            {
                Id = 2, Name = "string", Description = "string",
                Specifications = new List<string> { "Spec 1", "Spec 3", "Spec 5" }
            };

            //Act
            var result = (await client.GetResponse<CategoryModel>(model)).Message;

            //Assert
            foreach (Product product in mockContext.Categories.Include(c => c.Products)
                         .FirstOrDefault(x => x.Id == model.Id).Products)
                Assert.Equal(model.Specifications, product.SpecificationData.Keys);
        }

        [Fact]
        public async Task UpdateCategoryModelConsumer_ReturnsCategory_IfValidModelAndIdPassed()
        {
            //Arrange
            await ArrangeMT();
            var model = new UpdateCategoryModel { Id = 2, Name = "string", Description = "string" };

            //Act
            var result = (await client.GetResponse<CategoryModel>(model)).Message;

            var expectedJson =
                JsonSerializer.Serialize(
                    mockMapper.Map<CategoryModel>(mockContext.Categories.FirstOrDefault(x => x.Id == model.Id)));
            var actualJson = JsonSerializer.Serialize(result);

            //Assert
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task UpdateCategoryModelConsumer_ThrowsKeyNotFoundException_IfPassedIdDoesntExist()
        {
            //Arrange
            await ArrangeMT();
            var model = new UpdateCategoryModel { Id = 5, Name = "string", Description = "string" };

            //Act

            //Assert
            var ex = await Assert.ThrowsAsync<RequestFaultException>(async () =>
                await client.GetResponse<CategoryModel>(model));
            var exType = ex.Fault.Exceptions.FirstOrDefault().ExceptionType.Split('.')
                .LastOrDefault();
            var exMessage = ex.Fault.Exceptions.FirstOrDefault().Message;
            Assert.Equal($"Category {model.Id} NOT FOUND", exMessage);
            Assert.Equal("KeyNotFoundException", exType);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("1", "123456")]
        [InlineData("123", "1")]
        public async Task PutCategoryHandler_ThrowsAgrumentException_IfInvalidModelPassedButPassedIdExists(string name,
            string description)
        {
            //Arrange
            await ArrangeMT();
            var model = new UpdateCategoryModel { Id = 2, Name = name, Description = description };

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