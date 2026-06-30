using CompanyName.ProjectName.Domain.Users;

namespace CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Boundaries;

public sealed record CreateUserInput(string Name, string Email, Guid CorrelationId = default, UserRole Role = UserRole.User);
