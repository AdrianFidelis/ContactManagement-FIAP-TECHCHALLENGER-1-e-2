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

    [HttpGet("filterByDDD/{ddd}")]
    public async Task<IActionResult> GetContactsByDdd(int ddd)
    {
        // Chave do cache específica para o DDD
        var cacheKey = $"contactsByDDD_{ddd}";

        // Tenta recuperar do cache
        if (!_cache.TryGetValue(cacheKey, out List<Contact> contacts))
        {
            // Se não encontrado no cache, busca no repositório
            contacts = (await _repository.GetAllByDddAsync(ddd)).ToList();

            // Caso não existam contatos, retorna NotFound
            if (!contacts.Any())
            {
                return NotFound($"Nenhum contato encontrado para o DDD {ddd}.");
            }

            // Configurações de cache (opcionalmente ajustável)
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)) // Tempo de vida no cache
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));  // Tempo sem acesso antes de expirar

            // Armazena no cache
            _cache.Set(cacheKey, contacts, cacheOptions);
        }

        return Ok(contacts);
    }

}
