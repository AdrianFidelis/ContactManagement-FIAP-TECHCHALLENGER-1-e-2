using Contact.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;

namespace Contact.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => /api/contacts
    public class ContactsController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ContactsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContactRequest contact)
        {
            var client = _httpClientFactory.CreateClient("PersistenceApi");

          
            var response = await client.PostAsJsonAsync("api/persist", contact);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }

            var error = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, error);
        }
    }
}
