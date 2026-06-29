namespace CompanyName.ProjectName.Application.UseCases.Users.Login.Boundaries;

public sealed record LoginInput(string Email, Guid CorrelationId = default);
