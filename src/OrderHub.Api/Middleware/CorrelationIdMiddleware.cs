using Microsoft.Extensions.Primitives;

namespace OrderHub.Api.Middleware;

/// <summary>
/// Middleware que adiciona um ID de correlação aos cabeçalhos de requisição e resposta, facilitando o rastreamento de requisições entre diferentes serviços.
/// </summary>
internal sealed class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context.Request.Headers[HeaderName]);
        context.TraceIdentifier = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            await next(context);
        }
    }

    private static string ResolveCorrelationId(StringValues values) =>
        values.Count == 1 && !string.IsNullOrWhiteSpace(values[0]) ? values[0]! : Guid.NewGuid().ToString("N");
}