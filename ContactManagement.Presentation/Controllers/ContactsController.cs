using ContactManagement.Domain.Entities;
using ContactManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagement.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly ContactRepository _repository;

    public ContactsController(ContactRepository repository)
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
    public async Task<IActionResult> Create(Contact contact)
    {
        await _repository.AddAsync(contact);
        return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Contact contact)
    {
        if (id != contact.Id) return BadRequest();
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
