using CompanyName.ProjectName.Application.UseCases.Users.Login.Boundaries;

namespace CompanyName.ProjectName.Api.Requests.Auth;

public sealed record LoginRequest(string Email)
{
    public LoginInput MapToInput(Guid correlationId) => new(Email, correlationId);
}
