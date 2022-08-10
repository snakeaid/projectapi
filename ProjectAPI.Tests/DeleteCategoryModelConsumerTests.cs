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
using ProjectAPI.CategoryService;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Mapping;
using ProjectAPI.Primitives;
using Xunit;

namespace ProjectAPI.Tests
{
    public class DeleteCategoryModelConsumerTests : IAsyncDisposable
    {
        private IRequestClient<DeleteCategoryModel> client;
        private string databaseName;
        private ITestHarness harness;
        private IMapper mockMapper;
        private ServiceProvider provider;

        public DeleteCategoryModelConsumerTests()
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
                .AddSingleton<ILogger, NullLogger<DeleteCategoryModelConsumer>>()
                .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<DeleteCategoryModelConsumer>(); })
                .BuildServiceProvider(true);

            harness = provider.GetRequiredService<ITestHarness>();

            await harness.Start();

            client = harness.GetRequestClient<DeleteCategoryModel>();
        }

        [Fact]
        public async Task DeleteCategoryModelConsumer_DeletesCategory_IfPassedIdExists()
        {
            //Arrange
            await ArrangeMT();
            var model = new DeleteCategoryModel { Id = 2 };

            //Act
            var result = (await client.GetResponse<CategoryModel>(model)).Message;

            //Assert
            Assert.NotNull(mockContext.Categories.IgnoreQueryFilters()
                .FirstOrDefault(x => x.Id == model.Id).DateDeleted);
        }

        [Fact]
        public async Task DeleteCategoryModelConsumer_ReturnsCategory_IfIdExists()
        {
            //Arrange
            await ArrangeMT();
            var model = new DeleteCategoryModel { Id = 3 };

            //Act
            var result = (await client.GetResponse<CategoryModel>(model)).Message;

            var expectedJson = JsonSerializer.Serialize(mockMapper.Map<CategoryModel>(mockContext.Categories
                .IgnoreQueryFilters().FirstOrDefault(c => c.Id == model.Id)));
            var actualJson = JsonSerializer.Serialize(result);

            //Assert
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task DeleteCategoryModelConsumer_ThrowsKeyNotFoundException_IfPassedIdDoesntExist()
        {
            //Arrange
            await ArrangeMT();
            var model = new DeleteCategoryModel { Id = 5 };

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
    }
}