using CompanyName.ProjectName.Api.Endpoints;
using CompanyName.ProjectName.Api.IoC;
using CompanyName.ProjectName.Api.Middlewares;
using CompanyName.ProjectName.Application.IoC;
using CompanyName.ProjectName.Infrastructure.IoC;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApi(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<CorrelationIdLoggingMiddleware>();
app.UseSerilogRequestLogging();

app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "CompanyName.ProjectName API v1");
    options.RoutePrefix = "swagger";
});

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapEndpoints();

app.Run();

public partial class Program;
