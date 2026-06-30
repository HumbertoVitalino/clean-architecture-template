using CompanyName.ProjectName.Api.Requests.Auth;
using CompanyName.ProjectName.Application.UseCases.Users.Login.Boundaries;

namespace CompanyName.ProjectName.Api.Mappers.Auth;

internal static class LoginRequestMapper
{
    internal static LoginInput MapToInput(this LoginRequest request, Guid? correlationId = null) =>
        new(request.Email, correlationId ?? Guid.NewGuid());
}
