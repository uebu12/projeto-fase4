using Fiap.EcoColeta.Api.Exceptions;
using System.Net;
using System.Text.Json;

namespace Fiap.EcoColeta.Api.Middleware
{
    /// <summary>
    /// Middleware global de tratamento de exceções. Captura erros não
    /// tratados ao longo do pipeline e devolve uma resposta JSON
    /// padronizada com o status code apropriado, evitando que stack
    /// traces sejam expostos ao cliente.
    /// </summary>
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                NegocioException => HttpStatusCode.BadRequest,
                RecursoNaoEncontradoException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(exception, "Erro não tratado na aplicação EcoColeta API");
            }

            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new
            {
                status = (int)statusCode,
                erro = statusCode == HttpStatusCode.InternalServerError
                    ? "Ocorreu um erro interno ao processar a solicitação."
                    : exception.Message,
                timestamp = DateTime.UtcNow
            });

            return context.Response.WriteAsync(result);
        }
    }
}
