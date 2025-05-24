using ContactEntite = ContactManagement.Domain.Entities.Contact;
using ContactManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Contact.Persistence.API.Controllers
{
    [ApiController]
    [Route("api/persist")]
    public class PersistController : ControllerBase
    {
        private readonly IContactRepository _repository;

        public PersistController(IContactRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContactEntite contact)
        {
            await _repository.AddAsync(contact);
            return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var contacts = await _repository.GetAllAsync();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contact = await _repository.GetByIdAsync(id);
            if (contact == null)
                return NotFound();
            return Ok(contact);
        }
    }
}
