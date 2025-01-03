using ContactManagement.Domain.Entities;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ContactManagement.Application.Validators
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            // validar o nome
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("O nome é obrigatório")
                .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres");

            // validar e-mail
            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório")
                .Must(BeAValidEmail).WithMessage("E-mail inválido");

            // validar número de telefone
            RuleFor(c => c.Phone)
                .NotEmpty().WithMessage("O telefone é obrigatório")
                .Matches(@"^\(?\d{2}\)?[\s-]?\d{4,5}[-]?\d{4}$")
                .WithMessage("O telefone deve estar no formato correto (com DDD e opcionalmente com traço ou espaço).");
        }
        private bool BeAValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return emailRegex.IsMatch(email);
        }
    }
}
