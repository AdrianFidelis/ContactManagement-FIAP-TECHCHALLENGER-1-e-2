using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

public class ContactsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ContactsIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]    
    public async Task DeveCriarUmContato_ComSucesso()
    {
        // Arrange
        var novoContato = new
        {
            Name = "João",
            Email = "joao@email.com",
            Phone = new
            {
                CountryCode = 55,      // Código do Brasil, por exemplo
                RegionalCode = 11,     // DDD de São Paulo
                NumberPhone = "987654321" // 9 dígitos conforme esperado
            }
        };



        var content = new StringContent(JsonConvert.SerializeObject(novoContato), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/contacts", content);

        // Se houver erro, mostrar a resposta da API
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro no servidor: {response.StatusCode}\nDetalhes: {errorMessage}");
        }

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

}
