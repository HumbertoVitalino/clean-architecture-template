using CompanyName.ProjectName.Domain.Abstractions;
using CompanyName.ProjectName.Application.Interfaces;

namespace CompanyName.ProjectName.Application.Abstractions;

public interface IRepository<T, TId> where T : AggregateRoot<TId>
{
    IUnitOfWork UnitOfWork { get; }

    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}
