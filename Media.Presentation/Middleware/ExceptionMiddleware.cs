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
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env, 
            ILogger<ExceptionMiddleware> logger)
        {
            this._next = next;
            this._env = env;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this._next(context);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                await this.HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// This method is called when an exception is thrown. 
        /// Depending on the type of exception, change the API response.
        /// </summary>
        internal Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Exceptions can add custom headers by overriding GetHeaders().
            if (exception is CustomException customException)
            {
                foreach (var header in customException.GetHeaders())
                {
                    context.Response.Headers[header.Key] = header.Value;
                }
            }

            context.Response.StatusCode = exception switch
            {
                BadRequestException _ => StatusCodes.Status400BadRequest,
                UnauthorizedException _ => StatusCodes.Status401Unauthorized,
                NotFoundException _ => StatusCodes.Status404NotFound,
                AlreadyUsedException _ => StatusCodes.Status409Conflict,
                TooManyRequestsException _ => StatusCodes.Status429TooManyRequests,
                DatabaseOperationException _ => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError,
            };

            string message = context.Response.StatusCode == 500 && this._env.IsProduction()
                ? "Something has gone wrong."
                : exception.Message;

            var response = new
            {
                statusCode = context.Response.StatusCode,
                message
            };
            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
