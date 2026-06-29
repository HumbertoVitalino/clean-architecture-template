using Asp.Versioning;
using Asp.Versioning.Builder;
using CompanyName.ProjectName.Api.Requests.Auth;
using CompanyName.ProjectName.Api.Validators.Auth;
using CompanyName.ProjectName.Application.Commons;
using CompanyName.ProjectName.Application.Interfaces.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace CompanyName.ProjectName.Api.Endpoints.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuth(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/auth")
            .WithApiVersionSet(versionSet)
            .WithTags("Auth")
            .AllowAnonymous();

        group.MapPost("login",
            async (
                LoginRequest request,
                ILoginUseCase useCase,
                LoginRequestValidator validator,
                [FromHeader(Name = "X-Correlation-Id")] Guid correlationId,
                CancellationToken cancellationToken
            ) =>
            {
                var validation = validator.Validate(request);
                if (!validation.IsValid)
                    return Results.Unauthorized();

                var output = await useCase.ExecuteAsync(request.MapToInput(correlationId), cancellationToken);

                if (!output.IsValid)
                    return Results.Unauthorized();

                return Results.Ok(output);
            }
        )
        .WithName("Login")
        .Produces<Output>()
        .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}
