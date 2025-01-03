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
                new Contact
                {
                    Id = 1,
                    Name = "João Silva",
                    Email = "joao@email.com",
                    Phone = new Phone { CountryCode = 55, RegionalCode = 11, NumberPhone = 987654321 }
                },
                new Contact
                {
                    Id = 2,
                    Name = "Maria Souza",
                    Email = "maria@email.com",
                    Phone = new Phone { CountryCode = 55, RegionalCode = 21, NumberPhone = 987654321 }
                },
                new Contact
                {
                    Id = 3,
                    Name = "Rodolfo Eugenia",
                    Email = "mariaemail.com",
                    Phone = new Phone { CountryCode = 55, RegionalCode = 21, NumberPhone = 987654321 }
                }
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
            Assert.Equal(3, result.Count());
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
            var newContact = new Contact
            {
                Id = 4,
                Name = "Carlos",
                Email = "carlos@email.com",
                Phone = new Phone { CountryCode = 55, RegionalCode = 31, NumberPhone = 987654321 }
            };

            _mockRepo.Setup(repo => repo.AddAsync(newContact)).Callback<Contact>(c => _contacts.Add(c));

            // Act
            await _mockRepo.Object.AddAsync(newContact);

            // Assert
            Assert.Equal(4, _contacts.Count);
            Assert.Contains(_contacts, c => c.Name == "Carlos");
        }

        [Fact]
        public async Task UpdateContact_ShouldModifyContactDetails()
        {
            // Arrange
            var contactToUpdate = _contacts.First();
            contactToUpdate.Name = "João Atualizado";

            _mockRepo.Setup(repo => repo.UpdateAsync(contactToUpdate))
                     .Callback<Contact>(c => _contacts[0] = c);

            // Act
            await _mockRepo.Object.UpdateAsync(contactToUpdate);

            // Assert
            Assert.Equal("João Atualizado", _contacts[0].Name);
        }

        [Fact]
        public async Task DeleteContact_ShouldRemoveContactFromList()
        {
            // Arrange
            var contactId = 1;
            _mockRepo.Setup(repo => repo.DeleteAsync(contactId))
                     .Callback<int>(id => _contacts.RemoveAll(c => c.Id == id));

            // Act
            await _mockRepo.Object.DeleteAsync(contactId);

            // Assert
            Assert.DoesNotContain(_contacts, c => c.Id == contactId);
        }
    }
}
