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
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Mapping;
using ProjectAPI.ModelValidation;
using ProjectAPI.Primitives;
using ProjectAPI.ProductService;
using Xunit;

namespace ProjectAPI.Tests
{
    public class CreateProductModelConsumerTests : IAsyncDisposable
    {
        private IRequestClient<CreateProductModel> client;
        private string databaseName;
        private ITestHarness harness;

        private IMapper mockMapper;
        private ServiceProvider provider;

        public CreateProductModelConsumerTests()
        {
            databaseName = Guid.NewGuid().ToString();
            var context = mockContext;
            context.Products.Add(new Product
                { Id = 2, Name = "Test Product 2", Description = "Test description 2", CategoryId = 1 });
            context.Products.Add(new Product
                { Id = 3, Name = "Test Product 3", Description = "Test description 3", CategoryId = 1 });
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
                .AddSingleton<ILogger, NullLogger<CreateProductModelConsumer>>()
                .AddScoped<IValidator<CreateProductModel>, CreateProductModelValidator>()
                .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<CreateProductModelConsumer>(); })
                .BuildServiceProvider(true);

            harness = provider.GetRequiredService<ITestHarness>();

            await harness.Start();

            client = harness.GetRequestClient<CreateProductModel>();
        }

        [Fact]
        public async Task CreateProductModelConsumer_AddsProduct_IfValidModelPassed()
        {
            //Arrange
            await ArrangeMT();
            var model = new CreateProductModel
            {
                Name = "string", Description = "string", CategoryId = 1,
                SpecificationData = new Dictionary<string, string>
                    { { "Spec 1", "Value 1" }, { "Spec 2", "Value 2" } }
            };

            //Act
            var countBefore = await mockContext.Products.CountAsync();
            await client.GetResponse<ProductModel>(model);
            var countAfter = await mockContext.Products.CountAsync();

            //Assert
            Assert.Equal(countBefore + 1, countAfter);
            Assert.Equal(model.Name, mockContext.Products.LastOrDefault().Name);
            Assert.Equal(model.Description, mockContext.Products.LastOrDefault().Description);
        }

        [Fact]
        public async Task CreateProductModelConsumer_ReturnsProduct_IfValidModelPassed()
        {
            //Arrange
            await ArrangeMT();
            var model = new CreateProductModel
            {
                Name = "string", Description = "string", CategoryId = 1,
                SpecificationData = new Dictionary<string, string>
                    { { "Spec 1", "Value 1" }, { "Spec 2", "Value 2" } }
            };

            //Act
            var result = (await client.GetResponse<ProductModel>(model)).Message;

            var expectedJson =
                JsonSerializer.Serialize(mockMapper.Map<ProductModel>(mockContext.Products.LastOrDefault()));
            var actualJson = JsonSerializer.Serialize(result);

            //Assert
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task CreateProductModelConsumer_CoordinatesProductSpecifications_IfAllOk()
        {
            //Arrange
            await ArrangeMT();
            var context = mockContext;
            context.Categories.Add(new Category
            {
                Id = 2, Name = "Category 2", Description = "Description 2",
                Specifications = new List<string> { "Spec 2", "Spec 3" }
            });
            context.SaveChanges();
            var model = new CreateProductModel
            {
                Name = "string", Description = "string", CategoryId = 2,
                SpecificationData = new Dictionary<string, string>
                    { { "Spec 1", "Value 1" }, { "Spec 2", "Value 2" } }
            };

            //Act
            var result = (await client.GetResponse<ProductModel>(model)).Message;

            //Assert
            Assert.Equal(mockContext.Categories.FirstOrDefault(x => x.Id == result.CategoryId).Specifications,
                result.SpecificationData.Keys);
        }

        [Fact]
        public async Task PostProductHandler_AssignsDefaultCategory_IfValidModelPassedButCategoryIdIsInvalid()
        {
            //Arrange
            await ArrangeMT();
            var model = new CreateProductModel
            {
                Name = "string", Description = "string", CategoryId = 5,
                SpecificationData = new Dictionary<string, string>
                    { { "Spec 1", "Value 1" }, { "Spec 2", "Value 2" } }
            };

            //Act
            var result = (await client.GetResponse<ProductModel>(model)).Message;

            //Assert
            Assert.Equal(1, result.CategoryId);
        }


        [Theory]
        [InlineData("", "", 2)]
        [InlineData("1", "123456", -1)]
        [InlineData("123", "1", 5)]
        public async Task CreateProductModelConsumer_ThrowsArgumentException_IfInvalidModelPassed(string name,
            string description, int categoryId)
        {
            //Arrange
            await ArrangeMT();
            var model = new CreateProductModel { Name = name, Description = description, CategoryId = categoryId };

            //Act

            //Assert
            var ex = await Assert.ThrowsAsync<RequestFaultException>(async () =>
                await client.GetResponse<ProductModel>(model));
            var exType = ex.Fault.Exceptions.FirstOrDefault().ExceptionType.Split('.')
                .LastOrDefault();
            var exMessage = ex.Fault.Exceptions.FirstOrDefault().Message;
            Assert.Equal("ArgumentException", exType);
            Assert.Contains("must", exMessage);
        }
    }
}