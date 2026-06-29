namespace CompanyName.ProjectName.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeader = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!Guid.TryParse(context.Request.Headers[CorrelationIdHeader].FirstOrDefault(), out var correlationId))
        {
            correlationId = Guid.NewGuid();
            context.Request.Headers[CorrelationIdHeader] = correlationId.ToString();
        }

        context.Response.Headers[CorrelationIdHeader] = correlationId.ToString();

        await next(context);
    }
}
