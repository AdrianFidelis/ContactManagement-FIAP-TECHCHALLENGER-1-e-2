using Contact.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Contact.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ContactsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContactRequest request)
        {
            var client = _httpClientFactory.CreateClient("PersistenceAPI");

            var response = await client.PostAsJsonAsync("/api/contacts", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ContactResponse>();
                return Ok(result);
            }

            return StatusCode((int)response.StatusCode, "Erro ao salvar contato");
        }
    }
}
