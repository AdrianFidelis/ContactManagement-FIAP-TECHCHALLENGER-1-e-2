using Xunit;
using ContactManagement.Application.Validators;
using ContactManagement.Domain.Entities;
using FluentValidation.TestHelper;

namespace ContactManagement.Tests
{
    public class ContactValidatorTests
    {
        private readonly ContactValidator _validator;

        public ContactValidatorTests()
        {
            _validator = new ContactValidator();
        }

        // Teste para Nome Obrigatório
        [Fact]
        public void Name_ShouldBeRequired()
        {
            var contact = new Contact
            {
                Name = "",
                Phone = new Phone { CountryCode = 55, RegionalCode = 11, NumberPhone = 987654321 }
            };

            var result = _validator.TestValidate(contact);
            result.ShouldHaveValidationErrorFor(c => c.Name)
                  .WithErrorMessage("O nome é obrigatório");
        }

        // Teste para E-mail Válido
        [Theory]
        [InlineData("emailinvalido")]
        [InlineData("email@")]
        [InlineData("@email.com")]
        [InlineData("email@email")]
        public void Email_ShouldBeValid(string invalidEmail)
        {
            var contact = new Contact
            {
                Email = invalidEmail,
                Phone = new Phone { CountryCode = 55, RegionalCode = 11, NumberPhone = 987654321 }
            };

            var result = _validator.TestValidate(contact);
            result.ShouldHaveValidationErrorFor(c => c.Email)
                  .WithErrorMessage("E-mail inválido");
        }

        // Teste para Telefone no Padrão Brasileiro
        [Theory]
        [InlineData(0, 11, 987654321)] // Código do país inválido
        [InlineData(55, 99, 987)]      // Número de telefone muito curto
        [InlineData(55, 123, 987654321)] // DDD inválido
        public void Phone_ShouldBeInvalid_WhenIncorrectFormat(int countryCode, int regionalCode, int numberPhone)
        {
            var contact = new Contact
            {
                Phone = new Phone { CountryCode = countryCode, RegionalCode = regionalCode, NumberPhone = numberPhone }
            };

            var result = _validator.TestValidate(contact);
            result.ShouldHaveValidationErrorFor(c => c.Phone);
        }

        [Theory]
        [InlineData(55, 11, 987654321)]
        [InlineData(1, 21, 998765432)]
        public void Phone_ShouldBeValid_WhenCorrectFormat(int countryCode, int regionalCode, int numberPhone)
        {
            var contact = new Contact
            {
                Phone = new Phone { CountryCode = countryCode, RegionalCode = regionalCode, NumberPhone = numberPhone }
            };

            var result = _validator.TestValidate(contact);
            result.ShouldNotHaveValidationErrorFor(c => c.Phone);
        }

        // Teste para um Contato Válido (Não Deve Retornar Erros)
        [Fact]
        public void ValidContact_ShouldNotHaveErrors()
        {
            var contact = new Contact
            {
                Name = "João Silva",
                Email = "joao@email.com",
                Phone = new Phone { CountryCode = 55, RegionalCode = 11, NumberPhone = 987654321 }
            };

            var result = _validator.TestValidate(contact);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
