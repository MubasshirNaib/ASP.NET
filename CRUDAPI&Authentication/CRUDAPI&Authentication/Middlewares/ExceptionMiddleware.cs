//using WebApplication2.Exceptions;
using WebApplication2.Models;
using System.Net;
using WebApplication2.Exceptions;

namespace WebApplication2.Middlewares
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {

                _logger.LogError($"Something went wrong");
                await HandleException(context, ex);
            }

        }
        private static Task HandleException(HttpContext context,Exception ex)
        {
           // int statusCode = (int)HttpStatusCode.InternalServerError;
           int statusCode = StatusCodes.Status500InternalServerError;
            switch (ex)
            {
                case NotFoundException _:
                    statusCode = StatusCodes.Status404NotFound;
                    break;
                case BadRequestException _:
                    statusCode = StatusCodes.Status400BadRequest;
                    break;
                case DivideByZeroException _:
                    statusCode = StatusCodes.Status400BadRequest;
                    break;
                    // you can define some more exceptions, according to need
            }
            var errorResponse = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = ex.Message
            };
            context.Response.ContentType= "application/json";
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync(errorResponse.ToString());
        }
        //private static Task HandleException(HttpContext context, Exception ex)
        //{
        //    int statusCode = (int)HttpStatusCode.InternalServerError;
        //    int statusCode = StatusCodes.Status500InternalServerError;
        //    switch (ex)
        //    {
        //        case NotFoundException _:
        //            statusCode = StatusCodes.Status404NotFound;
        //            break;
        //        case BadRequestException _:
        //            statusCode = StatusCodes.Status400BadRequest;
        //            break;
        //        case DivideByZeroException _:
        //            statusCode = StatusCodes.Status400BadRequest;
        //            break;
        //            // you can define some more exceptions, according to need
        //    }
        //    var errorResponse = new ErrorResponse
        //    {
        //        StatusCode = statusCode,
        //        Message = ex.Message
        //    };
        //    context.Response.ContentType = "application/json";
        //    context.Response.StatusCode = statusCode;
        //    return context.Response.WriteAsync(errorResponse.ToString());
        //}
    }

    // Extension method for this middleware

    public static class ExceptionMiddlewareExtension
    {
        public static void CongigureExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }

}
