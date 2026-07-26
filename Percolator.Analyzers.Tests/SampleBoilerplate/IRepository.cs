using System;
using System.Threading.Tasks;

namespace SampleCode;

public class Customer(Guid id)
{
    public Guid Id { get; } = id;
}

public class Order(Guid id)
{
    public Guid Id { get; } = id;
}

public interface IRepository<T>
{
    Task<T> Load();
    Task Save();
}