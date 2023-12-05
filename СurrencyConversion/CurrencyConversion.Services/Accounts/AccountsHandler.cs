using CurrencyConversion.Api.Contracts.Accounts.Requests;
using CurrencyConversion.Api.Contracts.Accounts.Responses;
using CurrencyConversion.Data.Contexts;
using CurrencyConversion.Data.Entities;
using CurrencyConversion.Services.Common.Exceptions;
using CurrencyConversion.Services.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CurrencyConversion.Services.Accounts;

internal class AccountsHandler : IAccountsHandler
{
    private const double DefaultCommissionPercent = 0.05;
    
    private readonly DatabaseContext _context;
    private readonly IValidator<ConvertRequest> _convertRequestValidator;
    private readonly IValidator<TopUpRequest> _topUpRequestValidator;

    public AccountsHandler(
        DatabaseContext context,
        IValidator<ConvertRequest> convertRequestValidator,
        IValidator<TopUpRequest> topUpRequestValidator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _convertRequestValidator =
            convertRequestValidator ?? throw new ArgumentNullException(nameof(convertRequestValidator));
        _topUpRequestValidator =
            topUpRequestValidator ?? throw new ArgumentNullException(nameof(topUpRequestValidator));
    }
    
    public async Task<Guid> TopUp(TopUpRequest request, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        _topUpRequestValidator.ThrowIfErrors(request);

        var account =
            await CreateOrUpdateAccountByUserAndCurrency(request.UserId, request.CurrencyId, request.Sum, token);

        return account.Id;
    }

    public async Task<ConvertResponse> Convert(ConvertRequest request, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(request);

        _convertRequestValidator.ThrowIfErrors(request);
        
        await using (var tran = await _context.Database.BeginTransactionAsync(token))
        {
            try
            {
                var sumFrom = request.SumFrom * (-1);

                var accountFrom = await CreateOrUpdateAccountByUserAndCurrency(
                    request.UserId, request.CurrencyIdFrom, sumFrom, token);
                
                var sumTo = CalcSumTo(request.SumFrom, request.Rate, request.CommissionPercent);

                var accountTo = await CreateOrUpdateAccountByUserAndCurrency(
                    request.UserId, request.CurrencyIdTo, sumTo, token);
                
                await tran.CommitAsync(token);

                return new ConvertResponse
                {
                    BalanceFrom = accountFrom.Sum,
                    BalanceTo = accountTo.Sum,
                };
            }
            catch
            {
                await tran.RollbackAsync(token);
                throw;
            }
        }
    }

    private static decimal CalcSumTo(
        decimal sumFrom,
        decimal rate,
        double? commissionPercent)
    {
        var sum = sumFrom * rate;
        var commission = commissionPercent ?? DefaultCommissionPercent;
        return sum * (decimal)(100 - commission) / 100;
    }
    
    private async Task<AccountEntity> CreateOrUpdateAccountByUserAndCurrency(
        long userId,
        long currencyId,
        decimal sum,
        CancellationToken token)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(
            y => y.UserId == userId && y.CurrencyId == currencyId, token);

        if (account == null)
        {
            account = new AccountEntity
            {
                CurrencyId = currencyId,
                UserId = userId,
            };

            await _context.Accounts.AddAsync(account, token);
            await _context.SaveChangesAsync(token);
        }

        account.Sum += sum;

        if (account.Sum < 0)
        {
            throw new AlertException($"Недостаточно средств на счете {account.Id}");
        }

        await _context.SaveChangesAsync(token);
        return account;
    }
}