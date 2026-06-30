using CompanyName.ProjectName.Application.Interfaces;
using CompanyName.ProjectName.Application.Interfaces.Repositories;
using CompanyName.ProjectName.Domain.Users;
using CompanyName.ProjectName.Infrastructure.Repositories.Mappers;
using CompanyName.ProjectName.Infrastructure.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyName.ProjectName.Infrastructure.Repositories;

internal sealed class UserRepository(AppDbContext context) : IUserRepository
{
    public IUnitOfWork UnitOfWork => context;

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var model = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return model?.ToDomain();
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var model = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email.Value, cancellationToken);

        return model?.ToDomain();
    }

    public async Task<bool> ExistsWithEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        await context.Users.AnyAsync(u => u.Email == email.Value, cancellationToken);

    public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        context.EnqueueDomainEvents(entity.GetDomainEvents());
        entity.ClearDomainEvents();
        await context.Users.AddAsync(entity.ToModel(), cancellationToken);
    }

    public void Update(User entity)
    {
        context.EnqueueDomainEvents(entity.GetDomainEvents());
        entity.ClearDomainEvents();

        var model = entity.ToModel();
        var tracked = context.ChangeTracker.Entries<UserModel>()
            .FirstOrDefault(e => e.Entity.Id == entity.Id);

        if (tracked is not null)
            tracked.CurrentValues.SetValues(model);
        else
            context.Users.Update(model);
    }

    public void Remove(User entity)
    {
        var tracked = context.ChangeTracker.Entries<UserModel>()
            .FirstOrDefault(e => e.Entity.Id == entity.Id);

        if (tracked is not null)
            tracked.State = EntityState.Deleted;
        else
            context.Users.Remove(entity.ToModel());
    }
}
