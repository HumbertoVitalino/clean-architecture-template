using CompanyName.ProjectName.Api.Validators.Users;
using CompanyName.ProjectName.Application.DTOs.Users;
using CompanyName.ProjectName.Application.Interfaces.UseCases;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Boundaries;
using CompanyName.ProjectName.Application.UseCases.Users.GetUserById.Boundaries;

namespace CompanyName.ProjectName.Api.Endpoints.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsers(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization();

        group.MapPost("", async (
            CreateUserInput input,
            ICreateUserUseCase useCase,
            CreateUserInputValidator validator,
            CancellationToken ct) =>
        {
            var validation = validator.Validate(input);
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            var output = await useCase.ExecuteAsync(input, ct);

            if (!output.IsValid)
                return Results.BadRequest(output.ErrorMessages);

            var response = output.Result as UserResponse;
            return Results.Created($"/api/users/{response!.Id}", response);
        })
        .AllowAnonymous()
        .WithName("CreateUser")
        .Produces<UserResponse>(StatusCodes.Status201Created)
        .Produces<IEnumerable<string>>(StatusCodes.Status400BadRequest);

        group.MapGet("{id:guid}", async (
            Guid id,
            IGetUserByIdUseCase useCase,
            CancellationToken ct) =>
        {
            var output = await useCase.ExecuteAsync(new GetUserByIdInput(id), ct);

            if (!output.IsValid)
                return Results.NotFound(output.ErrorMessages);

            return Results.Ok(output.Result as UserResponse);
        })
        .WithName("GetUserById")
        .Produces<UserResponse>()
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
