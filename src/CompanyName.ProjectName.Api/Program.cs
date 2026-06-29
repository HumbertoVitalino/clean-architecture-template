using CompanyName.ProjectName.Api.Endpoints;
using CompanyName.ProjectName.Api.IoC;
using CompanyName.ProjectName.Api.Middleware;
using CompanyName.ProjectName.Application.IoC;
using CompanyName.ProjectName.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApi(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.Services.MigrateDatabase();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();

public partial class Program;
