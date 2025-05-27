using Application.Interfaces;
using Application.Repositories;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class GenericService<T> : IGenericService<T> where T : IHasId, IHasTitle
{
    protected readonly IGenericRepository<T> _repository;
    protected readonly ILogger<GenericService<T>> _logger;

    
    public GenericService(IGenericRepository<T> repository, ILogger<GenericService<T>> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    
    public async Task<T?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _repository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while getting {Entity}:{Id}: {ErrorMessage}", typeof(T).Name, id, ex.Message);
            throw;
        }
    }

    
    public async Task<List<T>> GetAllAsync(int page = 1, int pageCount = 10, bool asNoTracking = false, string? query = "")
    {
        try
        {
            List<T> entities;
        
            if (!string.IsNullOrWhiteSpace(query))
            {
                entities = await _repository.GetAllAsync(page, pageCount, asNoTracking, query);
            }
            else
            {
                entities = await _repository.GetAllAsync(page, pageCount, asNoTracking);
            }
            
            if (entities.Count == 0)
            {
                _logger.LogInformation("No {Entity} entities found (Page: {Page}, Count: {Count}, Query: '{Query}')", 
                    typeof(T).Name, page, pageCount, query);
            }
            else
            {
                _logger.LogDebug("Retrieved {Count} {Entity} entities (Page: {Page})", 
                    entities.Count, typeof(T).Name, page);
            }

            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while getting all {Entity} entities: {ErrorMessage}", typeof(T).Name, ex.Message);
            throw;
        }
    }

    
    public async Task AddAsync(T entity)
    {
        try
        {
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating {Entity}: {ErrorMessage}", typeof(T).Name, ex.Message);
            throw;
        }
    }

    public async Task UpdateAsync(T entity)
    {
        try
        {
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while updating {Entity}: {ErrorMessage}", typeof(T).Name, ex.Message);
            throw;
        }
    }

    
    public async Task DeleteAsync(Guid id)
    {
        try
        {
            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while deleting project: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        try
        {
            await _repository.AddRangeAsync(entities);
            await _repository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}