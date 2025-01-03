using FluentValidation;
using ContactManagement.Domain.Entities;

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
                .EmailAddress().WithMessage("E-mail inválido");

            // ✅ Agora validamos o telefone corretamente com um validador separado
            RuleFor(c => c.Phone)
                .NotNull().WithMessage("O telefone é obrigatório")
                .SetValidator(new PhoneValidator()); // ✅ Aplica o `PhoneValidator`
        }
    }
}
