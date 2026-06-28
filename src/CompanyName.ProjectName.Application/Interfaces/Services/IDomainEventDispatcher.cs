using CompanyName.ProjectName.Domain.Abstractions;

namespace CompanyName.ProjectName.Application.Interfaces.Services;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
}
