using FluentValidation;
using ContactManagement.Domain.Entities;

namespace ContactManagement.Application.Validators
{
    public class PhoneValidator : AbstractValidator<Phone>
    {
        public PhoneValidator()
        {
            RuleFor(p => p.CountryCode)
                .GreaterThan(0).WithMessage("O código do país deve ser informado.")
                .InclusiveBetween(1, 999).WithMessage("O código do país deve ter no máximo 3 dígitos.");

            RuleFor(p => p.RegionalCode)
                .GreaterThan(0).WithMessage("O código regional (DDD) deve ser informado.")
                .InclusiveBetween(10, 99).WithMessage("O DDD deve ter exatamente 2 dígitos.");

            RuleFor(p => p.NumberPhone)
                .InclusiveBetween(100000000, 999999999)
                .WithMessage("O número do telefone deve ter exatamente 9 dígitos.");
        }
    }
}
