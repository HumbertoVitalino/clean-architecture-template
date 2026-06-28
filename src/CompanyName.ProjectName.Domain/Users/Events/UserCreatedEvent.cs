using CompanyName.ProjectName.Domain.Abstractions;

namespace CompanyName.ProjectName.Domain.Users.Events;

public sealed record UserCreatedEvent(Guid UserId) : IDomainEvent;
