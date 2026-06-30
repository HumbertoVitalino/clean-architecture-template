using CompanyName.ProjectName.Api.Requests.Users;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Boundaries;

namespace CompanyName.ProjectName.Api.Mappers.Users;

internal static class CreateUserRequestMapper
{
    internal static CreateUserInput MapToInput(this CreateUserRequest request, Guid? correlationId = null) =>
        new(request.Name, request.Email, correlationId ?? Guid.NewGuid());
}
