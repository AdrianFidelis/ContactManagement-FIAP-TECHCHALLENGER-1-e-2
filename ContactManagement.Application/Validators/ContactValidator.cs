using FluentValidation;
using ContactManagement.Domain.Entities;
using System.Text.RegularExpressions;

namespace ContactManagement.Application.Validators
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("O nome é obrigatório")
                .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório")
                .Must(BeAValidEmail).WithMessage("E-mail inválido. Deve conter um domínio válido como .com ou .com.br");

            RuleFor(c => c.Phone)
                .NotNull().WithMessage("O telefone é obrigatório")
                .SetValidator(new PhoneValidator()); 
        }
        private bool BeAValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+(\.[a-zA-Z]{2,})+$");

            return emailRegex.IsMatch(email);
        }


    }
}
