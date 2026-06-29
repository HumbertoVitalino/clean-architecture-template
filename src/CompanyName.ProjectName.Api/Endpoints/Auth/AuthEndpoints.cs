using CompanyName.ProjectName.Api.Validators.Auth;
using CompanyName.ProjectName.Application.DTOs.Users;
using CompanyName.ProjectName.Application.Interfaces.UseCases;
using CompanyName.ProjectName.Application.UseCases.Users.Login.Boundaries;

namespace CompanyName.ProjectName.Api.Endpoints.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuth(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            LoginInput input,
            ILoginUseCase useCase,
            LoginInputValidator validator,
            CancellationToken ct) =>
        {
            var validation = validator.Validate(input);
            if (!validation.IsValid)
                return Results.Unauthorized();

            var output = await useCase.ExecuteAsync(input, ct);

            if (!output.IsValid)
                return Results.Unauthorized();

            return Results.Ok(output.Result as LoginResponse);
        })
        .WithTags("Auth")
        .AllowAnonymous()
        .WithName("Login")
        .Produces<LoginResponse>()
        .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}
