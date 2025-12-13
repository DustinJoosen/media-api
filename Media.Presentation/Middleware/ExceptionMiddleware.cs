using Media.Core.Exceptions;

namespace Media.Presentation.Middleware
{
    /// <summary>
    /// Middleware that catches all thrown exceptions, and adjusts the API response.
    /// Throwing an UnauthorizedException automatically gives a 401 for example.
    /// This way you don't need to handle specific return values, and can just throw
    /// exceptions when values don't match.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this._next(context);
            }
            catch (Exception ex)
            {
                await this.HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// This method is called when an exception is thrown. 
        /// Depending on the type of exception, change the API response.
        /// </summary>
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = exception switch
            {
                BadRequestException _ => StatusCodes.Status400BadRequest,
                UnauthorizedException _ => StatusCodes.Status401Unauthorized,
                NotFoundException _ => StatusCodes.Status404NotFound,
                AlreadyUsedException _ => StatusCodes.Status409Conflict,
                DatabaseOperationException _ => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError,
            };
            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = exception.Message
            };
            return context.Response.WriteAsJsonAsync(response);
        }
    }

}
