using CurrencyConversion.Api.Contracts.Accounts.Requests;
using FluentValidation;

namespace CurrencyConversion.Services.Accounts.Validators;

internal class TopUpRequestValidator : AbstractValidator<TopUpRequest>
{
    public TopUpRequestValidator()
    {
        RuleFor(y => y.Sum)
            .GreaterThan(0)
            .WithMessage("Сумма пополнения должна быть больше 0");
    }
}