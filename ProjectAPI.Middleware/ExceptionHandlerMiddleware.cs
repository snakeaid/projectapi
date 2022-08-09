using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ProjectAPI.Middleware
{
    /// <summary>
    /// This class provides custom exception handling.
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        /// <summary>
        /// An instance of <see cref="ILogger"/>, used for logging.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// An instance of <see cref="RequestDelegate"/> which represents the request.
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructs a new instance of <see cref="ExceptionHandlerMiddleware"/> using the specified logger and request delegate. 
        /// </summary>
        /// <param name="next">An instance of <see cref="RequestDelegate"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/> for <see cref="ExceptionHandlerMiddleware"/>.</param>
        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Tries to invoke the delegate and catches the exception if there is one.
        /// </summary>
        /// <param name="context">An instance of <see cref="HttpContext"/> class.</param>
        /// <returns><see cref="Task"/></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles the exception caught in <see cref="InvokeAsync(HttpContext)"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns><see cref="Task"/></returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            string exceptionType = exception.GetType().ToString().Split('.').LastOrDefault();
            string exceptionMessage = exception.Message;
            if (exception is RequestFaultException faultException)
            {
                exceptionType = faultException.Fault.Exceptions.FirstOrDefault().ExceptionType.Split('.')
                    .LastOrDefault();
                exceptionMessage = faultException.Fault.Exceptions.FirstOrDefault().Message;
            }

            _logger.LogWarning(Regex.Unescape(exceptionMessage));

            var message = JsonSerializer.Serialize(new { errorMessage = exceptionMessage });

            switch (exceptionType)
            {
                case "KeyNotFoundException":
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case "IndexOutOfRangeException":
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case "InvalidOperationException":
                case "ArgumentException":
                    message = Regex.Unescape(exceptionMessage);
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            await response.WriteAsync(message);
        }
    }
}