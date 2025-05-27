using Application.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenericController<T, TCreateDto, TReadDto, TUpdateDto> : ControllerBase 
    where T : IHasId, IHasTitle
{
    protected readonly ILogger<GenericController<T,TCreateDto, TReadDto, TUpdateDto>> _logger;
    protected readonly IGenericService<T> _service;
    protected readonly IMapper _mapper;

    public GenericController(
        IGenericService<T> service,
        ILogger<GenericController<T, TCreateDto, TReadDto, TUpdateDto>> logger, IMapper mapper)
    {
        _service = service;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    public virtual async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int count = 10, [FromQuery] string query = "")
    {
        try
        {
            var result = await _service.GetAllAsync(skip, count, true, query);
            var readDtos = result.Select(x => _mapper.Map<TReadDto>(x)).ToList();
            return Ok(readDtos);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while fetching {Type} entities", typeof(T).Name);
            return StatusCode(500, new { message = "An error occurred while fetching data" });
        }
    }

    [HttpGet("{id}")]
    public virtual async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            var readDto = _mapper.Map<TReadDto>(result);
            return Ok(readDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while fetching {Type} entity with id {Id}", typeof(T).Name, id);
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting {Type} entity with id {Id}", typeof(T).Name, id);
            return StatusCode(500);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TUpdateDto updateDto)
    {
        try
        {
            var entity = _mapper.Map<T>(updateDto);
            entity.Id = id;
            await _service.UpdateAsync(entity);
            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating {Type} entity with id {Id}", typeof(T).Name, id);
            return StatusCode(500);
        }
    }

    [HttpPost]
    public virtual async Task<IActionResult> Add([FromBody] TCreateDto createDto)
    {
        try
        {
            var entity = _mapper.Map<T>(createDto);
            await _service.AddAsync(entity);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding new {Type} entity", typeof(T).Name);
            return StatusCode(500);
        }
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> AddRange([FromBody] IEnumerable<TCreateDto> createDtos)
    {
        try
        {
            var entities = createDtos.Select(x => _mapper.Map<T>(x));
            
            await _service.AddRangeAsync(entities);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while bulk-adding {Type} entities", typeof(T).Name);
            return StatusCode(500);
        }
    }
}
