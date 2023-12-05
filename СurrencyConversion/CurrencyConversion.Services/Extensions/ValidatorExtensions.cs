using CurrencyConversion.Services.Common.Exceptions;
using FluentValidation;

namespace CurrencyConversion.Services.Extensions;

internal static class ValidatorExtensions
{
    public static void ThrowIfErrors<T>(this IValidator<T> validator, T request)
    {
        var result = validator.Validate(request);
        var error = result.Errors.FirstOrDefault();
        if (error != null)
        {
            throw new AlertException(error.ErrorMessage);
        }
    }
}