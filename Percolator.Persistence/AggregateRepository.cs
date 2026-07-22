using Percolator.Domain;

namespace Percolator.Persistence;

#pragma warning disable PA00001

public class AggregateRepository<TAggregate> : IAggregateRepository<TAggregate> where TAggregate : class, IAggregate
{
    public Task<TAggregate> LoadAsync(TAggregate aggregate)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(TAggregate aggregate)
    {
        throw new NotImplementedException();
    }
}