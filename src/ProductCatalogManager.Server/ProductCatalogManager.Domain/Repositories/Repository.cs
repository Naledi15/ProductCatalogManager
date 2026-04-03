using ProductCatalogManager.Domain.Interfaces;

namespace ProductCatalogManager.Domain.Repositories;

public abstract class Repository<T> : IRepository<T> where T : class
{
    private readonly Dictionary<int, T> _store = [];
    private int _nextId = 1;

    protected abstract int GetId(T entity);
    protected abstract T WithId(T entity, int id);

    public Task<T?> GetByIdAsync(int id)
    {
        _store.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<T>> GetAllAsync() =>
        Task.FromResult<IEnumerable<T>>([.. _store.Values]);

    public Task<T> AddAsync(T entity)
    {
        var newEntity = WithId(entity, _nextId++);
        _store[GetId(newEntity)] = newEntity;
        return Task.FromResult(newEntity);
    }

    public Task<T> UpdateAsync(T entity)
    {
        var id = GetId(entity);
        if (!_store.ContainsKey(id))
            throw new KeyNotFoundException($"Entity with id {id} not found.");

        _store[id] = entity;
        return Task.FromResult(entity);
    }

    public Task DeleteAsync(int id)
    {
        _store.Remove(id);
        return Task.CompletedTask;
    }
}
