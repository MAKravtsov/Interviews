using CurrencyConversion.Api.Contracts.Accounts.Requests;
using FluentValidation;

namespace CurrencyConversion.Services.Accounts.Validators;

internal class ConvertRequestValidator : AbstractValidator<ConvertRequest>
{
    public ConvertRequestValidator()
    {
        RuleFor(y => y.SumFrom)
            .GreaterThan(0)
            .WithMessage("Сумма конвертации должна быть больше 0");

        RuleFor(y => y.Rate)
            .GreaterThan(0)
            .WithMessage("Курс конвертации должен быть больше нуля");
        
        RuleFor(y => y.CommissionPercent)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Процент комиссии должен быть больше нуля");
    }
}