using Percolator.Domain;

namespace Percolator.Persistence;

public interface IAggregateRepository<TAggregate> where TAggregate : class, IAggregate
{
    Task<TAggregate> LoadAsync(TAggregate aggregate);
    Task SaveAsync(TAggregate aggregate);
}