using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class GenericController<T> : ControllerBase where T : IHasId, IHasTitle
{
    protected readonly ILogger<GenericController<T>> _logger;
    protected readonly IGenericService<T> _service;

    public GenericController(IGenericService<T> service, ILogger<GenericController<T>> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int skip = 0, 
        [FromQuery] int count = 10, 
        [FromQuery] string query = "")
    {
        try
        {
            var result = await _service.GetAllAsync(skip, count, true, query);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while fetching {Type} entities: {Message}",typeof(T).Name, e.Message);
            return StatusCode(500, new {message = "An error occurred while fetching data"});
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while fetching {Type} entities: {Message}",typeof(T).Name, e.Message);
            throw;
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting {Type} entities: {Message}",typeof(T).Name, e.Message);
            throw;
        }
    }
    
    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Update(Guid id, T entity)
    {
        try
        {
            entity.Id = id;
            await _service.UpdateAsync(entity);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error while updating {Type} entities: {Message}",typeof(T).Name, e.Message);
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add(T entity)
    {
        try
        {
            await _service.AddAsync(entity);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error while adding {Type} entity: {Message}",typeof(T).Name, e.Message);
            throw;
        }
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> AddRange(IEnumerable<T> entities)
    {
        try
        {
            await _service.AddRangeAsync(entities);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error while adding {Type} entities: {Message}",typeof(T).Name, e.Message);
            throw;
        }
    }
}