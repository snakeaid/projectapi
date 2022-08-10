using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Mapping;
using ProjectAPI.Primitives;
using ProjectAPI.ProductService;
using Xunit;

namespace ProjectAPI.Tests
{
    public class DeleteProductModelConsumerTests : IAsyncDisposable
    {
        private IRequestClient<DeleteProductModel> client;
        private string databaseName;
        private ITestHarness harness;
        private IMapper mockMapper;
        private ServiceProvider provider;

        public DeleteProductModelConsumerTests()
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
                .AddSingleton<ILogger, NullLogger<DeleteProductModelConsumer>>()
                .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<DeleteProductModelConsumer>(); })
                .BuildServiceProvider(true);

            harness = provider.GetRequiredService<ITestHarness>();

            await harness.Start();

            client = harness.GetRequestClient<DeleteProductModel>();
        }

        [Fact]
        public async Task DeleteProductModelConsumer_DeletesProduct_IfPassedIdExists()
        {
            //Arrange
            await ArrangeMT();
            var model = new DeleteProductModel { Id = 2 };

            //Act
            var result = (await client.GetResponse<ProductModel>(model)).Message;

            //Assert
            Assert.NotNull(mockContext.Products.IgnoreQueryFilters()
                .FirstOrDefault(x => x.Id == model.Id).DateDeleted);
        }

        [Fact]
        public async Task DeleteProductModelConsumer_ReturnsProduct_IfIdExists()
        {
            //Arrange
            await ArrangeMT();
            var model = new DeleteProductModel { Id = 3 };

            //Act
            var result = (await client.GetResponse<ProductModel>(model)).Message;

            var expectedJson = JsonSerializer.Serialize(mockMapper.Map<ProductModel>(mockContext.Products
                .IgnoreQueryFilters().FirstOrDefault(c => c.Id == model.Id)));
            var actualJson = JsonSerializer.Serialize(result);

            //Assert
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task DeleteProductModelConsumer_ThrowsKeyNotFoundException_IfPassedIdDoesntExist()
        {
            //Arrange
            await ArrangeMT();
            var model = new DeleteProductModel { Id = 5 };

            //Act

            //Assert
            var ex = await Assert.ThrowsAsync<RequestFaultException>(async () =>
                await client.GetResponse<ProductModel>(model));
            var exType = ex.Fault.Exceptions.FirstOrDefault().ExceptionType.Split('.')
                .LastOrDefault();
            var exMessage = ex.Fault.Exceptions.FirstOrDefault().Message;
            Assert.Equal($"Product {model.Id} NOT FOUND", exMessage);
            Assert.Equal("KeyNotFoundException", exType);
        }
    }
}