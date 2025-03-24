using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
public class ContactsIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ContactsIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }


    private async Task<int> CriarContatoTesteAsync()
    {
        var novoContato = new
        {
            Name = "Contato Teste",
            Email = "teste@email.com",
            Phone = new
            {
                CountryCode = 55,
                RegionalCode = 11,
                NumberPhone = GerarNumeroTelefoneAleatorio()
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(novoContato), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/contacts", content);
        var body = await response.Content.ReadAsStringAsync();
        dynamic json = JsonConvert.DeserializeObject(body);
        return (int)json.id;
    }

    [Fact]
    public async Task DeveCriarUmContato_ComSucesso()
    {
        var novoContato = new
        {
            Name = "João",
            Email = "joao@email.com",
            Phone = new
            {
                CountryCode = 55,
                RegionalCode = 11,
                NumberPhone = GerarNumeroTelefoneAleatorio()
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(novoContato), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/contacts", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro no servidor: {response.StatusCode}\nDetalhes: {errorMessage}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task DeveListarTodosOsContatos()
    {
        await CriarContatoTesteAsync();

        var response = await _client.GetAsync("/api/contacts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Contato Teste");
    }

    [Fact]
    public async Task DeveObterContatoPorId()
    {
        int id = await CriarContatoTesteAsync();

        var response = await _client.GetAsync($"/api/contacts/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Contato Teste");
    }

    [Fact]
    public async Task DeveAtualizarContato()
    {

        int id = await CriarContatoTesteAsync();


        var getResponse = await _client.GetAsync($"/api/contacts/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await getResponse.Content.ReadAsStringAsync();


        dynamic contato = JsonConvert.DeserializeObject(json);
        contato.name = "Contato Atualizado";
        contato.email = "atualizado@email.com";


        var content = new StringContent(JsonConvert.SerializeObject(contato), Encoding.UTF8, "application/json");


        var putResponse = await _client.PutAsync($"/api/contacts/{id}", content);


        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeveRemoverContato()
    {
        int id = await CriarContatoTesteAsync();

        var response = await _client.DeleteAsync($"/api/contacts/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeveRetornarErro_QuandoDadosInvalidos()
    {
        var contatoInvalido = new
        {
            Name = "", // inválido
            Email = "emailinvalido",
            Phone = new { } // faltando campos obrigatórios
        };

        var content = new StringContent(JsonConvert.SerializeObject(contatoInvalido), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/contacts", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private string GerarNumeroTelefoneAleatorio()
    {
        var random = new Random();
        return random.Next(100000000, 999999999).ToString(); // 9 dígitos
    }
}
