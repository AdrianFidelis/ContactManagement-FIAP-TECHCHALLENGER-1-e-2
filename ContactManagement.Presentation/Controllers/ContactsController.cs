using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ContactManagement.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactRepository _repository;
    private readonly IMemoryCache _cache;

    public ContactsController(IContactRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!_cache.TryGetValue("contacts", out List<Contact> contacts))
        {
            //  Converte para List<Contact> para evitar erro CS0266
            contacts = (await _repository.GetAllAsync()).ToList();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            _cache.Set("contacts", contacts, cacheOptions);
        }

        return Ok(contacts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        //  Busca no cache primeiro
        var cacheKey = $"contact_{id}";
        if (!_cache.TryGetValue(cacheKey, out Contact contact))
        {
            contact = await _repository.GetByIdAsync(id);
            if (contact == null) return NotFound();

            //  Armazena no cache com expiração de 10 minutos
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(cacheKey, contact, cacheOptions);
        }

        return Ok(contact);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? ddd)
    {
        if (!_cache.TryGetValue("contacts", out List<Contact> contacts))
        {
            contacts = (await _repository.GetAllAsync()).ToList();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            _cache.Set("contacts", contacts, cacheOptions);
        }

        if (ddd.HasValue)
        {
            contacts = contacts.Where(c => c.Phone.RegionalCode == ddd.Value).ToList();
        }

        return Ok(contacts);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Contact contact)
    {
        if (contact.Phone == null) return BadRequest("O telefone é obrigatório.");

        await _repository.AddAsync(contact);

        // Remove o cache para forçar atualização
        _cache.Remove("contacts");

        return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Contact contact)
    {
        if (id != contact.Id) return BadRequest();
        if (contact.Phone == null) return BadRequest("O telefone é obrigatório.");

        await _repository.UpdateAsync(contact);

        //  Remove o cache do contato atualizado e da lista
        _cache.Remove("contacts");
        _cache.Remove($"contact_{id}");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);

        //  Remove o cache do contato deletado e da lista
        _cache.Remove("contacts");
        _cache.Remove($"contact_{id}");

        return NoContent();
    }
}
