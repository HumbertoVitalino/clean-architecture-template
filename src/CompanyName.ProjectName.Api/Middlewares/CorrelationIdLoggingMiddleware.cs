using Serilog.Context;

namespace CompanyName.ProjectName.Api.Middlewares;

public sealed class CorrelationIdLoggingMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var header)
            && Guid.TryParse(header, out var parsed)
                ? parsed
                : Guid.NewGuid();

        context.Response.Headers[HeaderName] = correlationId.ToString();

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
