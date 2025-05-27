using Domain.Interfaces;

namespace Application.Interfaces;


public interface IGenericService<T> where T : IHasId, IHasTitle
{
    Task<T?> GetByIdAsync(Guid id);
    Task<List<T>> GetAllAsync(int page = 1, int pageCount = 10, bool asNoTracking = false, string? query = "");
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task AddRangeAsync(IEnumerable<T> entities);
}