using Asp.Versioning;
using Asp.Versioning.Builder;
using CompanyName.ProjectName.Api.Mappers.Users;
using CompanyName.ProjectName.Api.Requests.Users;
using CompanyName.ProjectName.Api.Validators.Users;
using CompanyName.ProjectName.Application.Commons;
using CompanyName.ProjectName.Application.DTOs.Users;
using CompanyName.ProjectName.Application.Interfaces.UseCases;
using CompanyName.ProjectName.Application.UseCases.Users.GetUserById.Boundaries;
using Microsoft.AspNetCore.Mvc;

namespace CompanyName.ProjectName.Api.Endpoints.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsers(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/users")
            .WithApiVersionSet(versionSet)
            .WithTags("Users")
            .RequireAuthorization("UserOnly");

        group.MapPost("", async (
            CreateUserRequest request,
            ICreateUserUseCase useCase,
            CreateUserRequestValidator validator,
            [FromHeader(Name = "X-Correlation-Id")] Guid? correlationId,
            CancellationToken cancellationToken) =>
        {
            var validation = validator.Validate(request);
            if (!validation.IsValid)
            {
                Output validationOutput = new();
                validationOutput.AddErrorMessages(validation.Errors.Select(e => e.ErrorMessage));
                return Results.BadRequest(validationOutput);
            }

            var output = await useCase.ExecuteAsync(request.MapToInput(correlationId), cancellationToken);

            if (!output.IsValid)
                return Results.BadRequest(output);

            return Results.Created($"/api/v1/users/{output.GetResult<UserResponse>()!.Id}", output);
        })
        .AllowAnonymous()
        .WithName("CreateUser")
        .Produces<Output>(StatusCodes.Status201Created)
        .Produces<Output>(StatusCodes.Status400BadRequest);

        group.MapGet("{id:guid}", async (
            Guid id,
            IGetUserByIdUseCase useCase,
            [FromHeader(Name = "X-Correlation-Id")] Guid? correlationId,
            CancellationToken cancellationToken) =>
        {
            var output = await useCase.ExecuteAsync(new GetUserByIdInput(id, correlationId ?? Guid.NewGuid()), cancellationToken);

            if (!output.IsValid)
                return Results.NotFound(output);

            return Results.Ok(output);
        })
        .WithName("GetUserById")
        .Produces<Output>()
        .Produces<Output>(StatusCodes.Status404NotFound);

        return app;
    }
}
