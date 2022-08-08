using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectAPI.BusinessLogic.Extensions;
using ProjectAPI.DataAccess;
using ProjectAPI.DataAccess.Primitives;
using ProjectAPI.Primitives;

namespace ProjectAPI.BatchUploadService
{
    public class BatchCategoryUploadConsumer : IConsumer<UploadRequest>
    {
        private readonly CatalogContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateCategoryModel> _validator;

        /// <summary>
        /// Constructs an instance of <see cref="BatchCategoryUploadConsumer"/> using the specified context, mapper,
        /// logger and validator.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
        /// for <see cref="BatchCategoryUploadConsumer"/>.</param>
        /// <param name="validator">An instance of <see cref="IValidator{T}"/> for <see cref="CategoryModel"/>.</param>
        public BatchCategoryUploadConsumer(CatalogContext context, IMapper mapper,
            ILogger<BatchCategoryUploadConsumer> logger, IValidator<CreateCategoryModel> validator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        public async Task Consume(ConsumeContext<UploadRequest> context)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(r => r.Id == context.Message.Id);
            _logger.Log(LogLevel.Information, $"Beginning batch upload {request.Id}.");
            var categories = request.ParseIntoList<CreateCategoryModel>();
            _logger.Log(LogLevel.Information, "Parsed the file successfully.");
            request.Status = "Parsed the file successfully.";
            await _context.SaveChangesAsync();

            int successCount = 0;
            for (var i = 0; i < categories.Count; i++)
            {
                var categoryModel = categories[i];
                var result = await _validator.ValidateAsync(categoryModel);
                if (!result.IsValid)
                {
                    // string errors = JsonSerializer.Serialize(result.ToDictionary());
                    // throw new ArgumentException(errors); //TODO errors
                    _logger.Log(LogLevel.Information, $"Category {i + 1} is invalid, moving to the next one.");
                    continue;
                }

                var category = _mapper.Map<Category>(categoryModel);
                await _context.Categories.AddAsync(category);
                successCount++;
                _logger.Log(LogLevel.Information, $"Added category {i + 1} successfully.");
                request.Status = $"Adding the categories...\n{successCount}/{categories.Count} added.";
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();

            var sb = new StringBuilder(
                $"Upload completed: {successCount}/{categories.Count} categories were added successfully.");
            if (successCount < categories.Count)
                sb.Append(" The rest {products.Count - successCount} categories were invalid.");
            request.Status = sb.ToString();
            await _context.SaveChangesAsync();

            sb.Insert(7, $"{request.Id} ");
            _logger.Log(LogLevel.Information, sb.ToString());
        }
    }
}