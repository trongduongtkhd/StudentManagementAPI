using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StudentManagementAPI.Exceptions;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudentManagementAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Ghi đầy đủ lỗi vào Output / Console
                _logger.LogError(
                    ex,
                    "Unhandled exception: {Message}",
                    ex.Message
                );

                await HandleExceptionAsync(context, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode = 500;
            string message = "Có lỗi xảy ra";
            switch (exception)
            {
                case NotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;

                case BadRequestException:
                    statusCode = (int)HttpStatusCode.BadRequest;

                    message = exception.Message;
                    break;

                case ForbiddenException:

                    statusCode = (int)HttpStatusCode.Forbidden;
                    message = exception.Message;
                    break;
                case UnauthorizedException:

                    statusCode = (int)HttpStatusCode.Unauthorized;

                    message = exception.Message;

                    break;
                default:
                    statusCode = 500;
                    break;
            }

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = statusCode;

            var response = new
            {
                success = false,

                statusCode,

                message,
                 
                timestamp = DateTime.UtcNow
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

    }
}