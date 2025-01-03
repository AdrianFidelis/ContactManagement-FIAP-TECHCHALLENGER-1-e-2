using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagement.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactRepository _repository;

    public ContactsController(IContactRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repository.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var contact = await _repository.GetByIdAsync(id);
        return contact == null ? NotFound() : Ok(contact);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Contact contact)
    {
        if (contact.Phone == null)
        {
            return BadRequest("O telefone é obrigatório.");
        }

        await _repository.AddAsync(contact);
        return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Contact contact)
    {
        if (id != contact.Id) return BadRequest();

        if (contact.Phone == null)
        {
            return BadRequest("O telefone é obrigatório.");
        }

        await _repository.UpdateAsync(contact);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
