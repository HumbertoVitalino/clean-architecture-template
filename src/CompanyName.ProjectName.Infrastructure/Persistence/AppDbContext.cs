using CompanyName.ProjectName.Application.Interfaces;
using CompanyName.ProjectName.Application.Interfaces.Services;
using CompanyName.ProjectName.Domain.Abstractions;
using CompanyName.ProjectName.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyName.ProjectName.Infrastructure.Persistence;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IDomainEventDispatcher? dispatcher = null
) : DbContext(options), IUnitOfWork
{
    private readonly List<IDomainEvent> _pendingEvents = [];

    public DbSet<UserModel> Users => Set<UserModel>();

    internal void EnqueueDomainEvents(IEnumerable<IDomainEvent> events) =>
        _pendingEvents.AddRange(events);

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await base.SaveChangesAsync(cancellationToken) > 0;

            if (dispatcher is not null && _pendingEvents.Count > 0)
            {
                var events = _pendingEvents.ToList();
                _pendingEvents.Clear();
                await dispatcher.DispatchAsync(events, cancellationToken);
            }

            return result;
        }
        catch
        {
            return false;
        }
    }
}
