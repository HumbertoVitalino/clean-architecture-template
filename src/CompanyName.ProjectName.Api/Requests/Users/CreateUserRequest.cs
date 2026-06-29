using CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Boundaries;

namespace CompanyName.ProjectName.Api.Requests.Users;

public sealed record CreateUserRequest(string Name, string Email)
{
    public CreateUserInput MapToInput(Guid correlationId) => new(Name, Email, correlationId);
}
