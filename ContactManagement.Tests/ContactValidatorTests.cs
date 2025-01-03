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
        [InlineData("email@email")]        // ❌ Sem extensão
        [InlineData("email@email.")]       // ❌ Apenas um ponto no final
        [InlineData("email@email..com")]   // ❌ Dois pontos consecutivos (CORRIGIDO)
        [InlineData("email@.com")]         // ❌ Apenas TLD sem domínio
        [InlineData("email@email.com")]    // ✅ Correto
        [InlineData("email@email.com.br")] // ✅ Correto
        [InlineData("email123@site.net")]  // ✅ Correto
        public void Email_ShouldBeInvalid_WhenMissingDomainExtension(string invalidEmail)
        {
            var contact = new Contact
            {
                Name = "Teste",
                Email = invalidEmail,
                Phone = new Phone { CountryCode = 55, RegionalCode = 11, NumberPhone = 987654321 }
            };

            var result = _validator.TestValidate(contact);

            if (invalidEmail == "email@email.com" || invalidEmail == "email@email.com.br" || invalidEmail == "email123@site.net")
            {
                result.ShouldNotHaveValidationErrorFor(c => c.Email); // ✅ Deve ser válido
            }
            else
            {
                result.ShouldHaveValidationErrorFor(c => c.Email)
                      .WithErrorMessage("E-mail inválido. Deve conter um domínio válido como .com ou .com.br"); // ✅ Deve falhar
            }
        }


        // Teste para Código do País inválido
        [Theory]
        [InlineData(0)]    // ❌ Código do país inválido (menor que 1)
        [InlineData(1000)] // ❌ Código do país inválido (maior que 999)
        public void Phone_ShouldBeInvalid_WhenCountryCodeIsIncorrect(int countryCode)
        {
            var contact = new Contact
            {
                Name = "João Silva", // ✅ Nome válido
                Email = "joao@email.com", // ✅ E-mail válido
                Phone = new Phone { CountryCode = countryCode, RegionalCode = 11, NumberPhone = 987654321 }
            };

            var result = _validator.TestValidate(contact);

            result.ShouldHaveValidationErrorFor(c => c.Phone.CountryCode)
                .WithErrorMessage("O código do país deve ter no máximo 3 dígitos.");
        }

        // Teste para Código Regional (DDD) inválido
        [Theory]
        [InlineData(0)]  // ❌ Código regional inválido (menor que 10)
        [InlineData(100)] // ❌ Código regional inválido (maior que 99)
        public void Phone_ShouldBeInvalid_WhenRegionalCodeIsIncorrect(int regionalCode)
        {
            var contact = new Contact
            {
                Name = "João Silva", // ✅ Nome válido
                Email = "joao@email.com", // ✅ E-mail válido
                Phone = new Phone { CountryCode = 55, RegionalCode = regionalCode, NumberPhone = 987654321 }
            };

            var result = _validator.TestValidate(contact);

            result.ShouldHaveValidationErrorFor(c => c.Phone.RegionalCode)
                .WithErrorMessage("O DDD deve ter exatamente 2 dígitos.");
        }

        // Teste para Número do Telefone inválido
        [Theory]
        [InlineData(12345678)]   // ❌ Muito curto (8 dígitos)
        [InlineData(1234567890)] // ❌ Muito longo (10 dígitos)
        public void Phone_ShouldBeInvalid_WhenNumberPhoneIsIncorrect(int numberPhone)
        {
            var contact = new Contact
            {
                Name = "João Silva", // ✅ Nome válido
                Email = "joao@email.com", // ✅ E-mail válido
                Phone = new Phone { CountryCode = 55, RegionalCode = 11, NumberPhone = numberPhone }
            };

            var result = _validator.TestValidate(contact);

            result.ShouldHaveValidationErrorFor(c => c.Phone.NumberPhone)
                .WithErrorMessage("O número do telefone deve ter exatamente 9 dígitos.");
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
