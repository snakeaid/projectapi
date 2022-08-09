using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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

namespace ProjectAPI.UploadService
{
    /// <summary>
    /// This class represents a MassTransit consumer class which consumes messages to upload a batch of products
    /// and implements <see cref="IConsumer{TMessage}"/> for <see cref="UploadRquest"/>.
    /// </summary>
    public class BatchProductUploadConsumer : IConsumer<UploadRequest>
    {
        private readonly CatalogContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateProductModel> _validator;

        /// <summary>
        /// Constructs an instance of <see cref="BatchProductUploadConsumer"/> using the specified context, mapper,
        /// logger and validator.
        /// </summary>
        /// <param name="context">An instance of <see cref="CatalogContext"/>.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TProductName}"/>
        /// for <see cref="BatchProductUploadConsumer"/>.</param>
        /// <param name="validator">An instance of <see cref="IValidator{T}"/> for <see cref="ProductModel"/>.</param>
        public BatchProductUploadConsumer(CatalogContext context, IMapper mapper,
            ILogger<BatchProductUploadConsumer> logger, IValidator<CreateProductModel> validator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        /// <summary>
        /// Consumes the message to upload a batch of products.
        /// </summary>
        /// <param name="context">An instance of <see cref="ConsumeContext{T}"/> for <see cref="UploadRequest"/>.</param>
        public async Task Consume(ConsumeContext<UploadRequest> context)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(r => r.Id == context.Message.Id);
            _logger.Log(LogLevel.Information, $"Beginning batch upload {request.Id}.");
            var products = request.ParseIntoList<CreateProductModel>();
            _logger.Log(LogLevel.Information, "Parsed the file successfully.");
            request.Status = "Parsed the file successfully.";
            await _context.SaveChangesAsync();

            int successCount = 0;
            for (var i = 0; i < products.Count; i++)
            {
                var productModel = products[i];
                var result = await _validator.ValidateAsync(productModel);
                if (!result.IsValid)
                {
                    string errors = Regex.Unescape(JsonSerializer.Serialize(result.ToDictionary()));
                    _logger.Log(LogLevel.Information,
                        $"Category {i + 1} is invalid, moving to the next one. Errors: \n" + errors);
                    continue;
                }

                if (await _context.Categories.FirstOrDefaultAsync(c => c.Id == productModel.CategoryId) == null)
                {
                    _logger.LogWarning(
                        $"Category {productModel.CategoryId} NOT FOUND. Assigning category 1 to the product");
                    productModel.CategoryId = 1;
                }

                var finalSpecifications = (await _context.Categories
                        .FirstOrDefaultAsync(c => c.Id == productModel.CategoryId)).Specifications
                    .ToDictionary(s => s, s => "");
                foreach (KeyValuePair<string, string> keyValuePair in productModel.SpecificationData)
                    if (finalSpecifications.ContainsKey(keyValuePair.Key))
                        finalSpecifications[keyValuePair.Key] = keyValuePair.Value;
                productModel.SpecificationData = finalSpecifications;

                var product = _mapper.Map<Product>(productModel);
                _context.Products.Add(product);

                successCount++;
                _logger.Log(LogLevel.Information, $"Added product {i + 1} successfully.");
                request.Status = $"Adding the products...\n{successCount}/{products.Count} added.";
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();

            var sb = new StringBuilder(
                $"Upload completed: {successCount}/{products.Count} products were added successfully.");
            if (successCount < products.Count)
                sb.Append($" The rest {products.Count - successCount} products were invalid.");
            request.Status = sb.ToString();
            await _context.SaveChangesAsync();

            sb.Insert(7, $"{request.Id} ");
            _logger.Log(LogLevel.Information, sb.ToString());
        }
    }
}