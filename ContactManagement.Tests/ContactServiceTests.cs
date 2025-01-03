using Xunit;
using Moq;
using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ContactManagement.Tests
{
    public class ContactServiceTests
    {
        private readonly Mock<IContactRepository> _mockRepo;
        private readonly List<Contact> _contacts;

        public ContactServiceTests()
        {
            _mockRepo = new Mock<IContactRepository>();
            _contacts = new List<Contact>
            {
                new Contact { Id = 1, Name = "João Silva", Email = "joao@email.com", Phone = "11987654321" },
                new Contact { Id = 2, Name = "Maria Souza", Email = "maria@email.com", Phone = "21987654321" }
            };
        }

        [Fact]
        public async Task GetAll_ShouldReturnContacts()
        {
            // Arrange (Configuração)
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_contacts);

            // Act (Execução)
            var result = await _mockRepo.Object.GetAllAsync();

            // Assert (Verificação)
            Assert.Equal(2, result.Count());
            Assert.Equal("João Silva", result.First().Name);
        }

        [Fact]
        public async Task GetById_ShouldReturnCorrectContact()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(_contacts.First());

            // Act
            var result = await _mockRepo.Object.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("João Silva", result.Name);
        }

        [Fact]
        public async Task AddContact_ShouldIncreaseListSize()
        {
            // Arrange
            var newContact = new Contact { Id = 3, Name = "Carlos", Email = "carlos@email.com", Phone = "31987654321" };

            _mockRepo.Setup(repo => repo.AddAsync(newContact)).Callback<Contact>(c => _contacts.Add(c));

            // Act
            await _mockRepo.Object.AddAsync(newContact);

            // Assert
            Assert.Equal(3, _contacts.Count);
            Assert.Contains(_contacts, c => c.Name == "Carlos");
        }
    }
}
