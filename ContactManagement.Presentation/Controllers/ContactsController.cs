using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory; // Importação necessária

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

    /// 🔹 **GET ALL com Cache**
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        const string cacheKey = "contacts_list";

        if (!_cache.TryGetValue(cacheKey, out List<Contact> contacts))
        {
            // Dados não encontrados no cache, buscar no banco
            contacts = (await _repository.GetAllAsync()).ToList();

            // Configuração do cache: duração de 10 minutos
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            _cache.Set(cacheKey, contacts, cacheOptions);
        }

        return Ok(contacts);
    }

    /// 🔹 **GET BY ID com Cache**
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        string cacheKey = $"contact_{id}";

        if (!_cache.TryGetValue(cacheKey, out Contact contact))
        {
            // Dados não encontrados no cache, buscar no banco
            contact = await _repository.GetByIdAsync(id);

            if (contact == null) return NotFound();

            // Configuração do cache: duração de 10 minutos
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            _cache.Set(cacheKey, contact, cacheOptions);
        }

        return Ok(contact);
    }

    /// **Criar Contato**
    [HttpPost]
    public async Task<IActionResult> Create(Contact contact)
    {
        await _repository.AddAsync(contact);

        // Invalida a lista de contatos no cache após a criação de um novo contato
        _cache.Remove("contacts_list");

        return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
    }

    /// **Atualizar Contato**
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Contact contact)
    {
        if (id != contact.Id) return BadRequest();

        await _repository.UpdateAsync(contact);

        // Atualiza o cache individual e remove a lista para atualização futura
        _cache.Set($"contact_{id}", contact, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });

        _cache.Remove("contacts_list");

        return NoContent();
    }

    /// **Deletar Contato**
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);

        // Remove do cache ao excluir
        _cache.Remove($"contact_{id}");
        _cache.Remove("contacts_list");

        return NoContent();
    }
}
