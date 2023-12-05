using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.AutoNSubstitute;
using CurrencyConversion.Api.Contracts.Accounts.Requests;
using CurrencyConversion.Data.Contexts;
using CurrencyConversion.Data.Entities;
using CurrencyConversion.Services.Accounts;
using CurrencyConversion.Services.Common.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Moq.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace CurrencyConversion.UnitTests.Accounts;

public class AccountsHandlerTests
{
    private readonly IFixture _fixture;
    private readonly IValidator<ConvertRequest> _convertRequestValidator;
    private readonly IValidator<TopUpRequest> _topUpRequestValidator;
    private readonly Mock<DatabaseContext> _contextMock;

    public AccountsHandlerTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoNSubstituteCustomization())
            .Customize(new AutoMoqCustomization());
        _contextMock = _fixture.Freeze<Mock<DatabaseContext>>();
        _convertRequestValidator = _fixture.Freeze<IValidator<ConvertRequest>>();
        _topUpRequestValidator = _fixture.Freeze<IValidator<TopUpRequest>>();
    }

    [Fact]
    public async Task TopUp_ValidationFailed_Throw()
    {
        // Arrange
        var request = _fixture.Create<TopUpRequest>();
        
        var errorValidationResult = new ValidationResult
        {
            Errors = new List<ValidationFailure>
            {
                new("testProp", "Some error")
            }
        };
        _topUpRequestValidator.Validate(request).Returns(errorValidationResult);

        var sut = _fixture.Create<AccountsHandler>();
        
        // Act
        var action = () => sut.TopUp(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<AlertException>(action);
    }

    [Fact]
    public async Task TopUp_AddAccount_IfNotExists()
    {
        // Arrange
        var request = _fixture.Create<TopUpRequest>();

        var accounts = new List<AccountEntity>();
        MockDbContext(accounts);

        _topUpRequestValidator.Validate(request).Returns(new ValidationResult());
        
        var sut = _fixture.Create<AccountsHandler>();

        // Act
        await sut.TopUp(request, CancellationToken.None);

        // Assert
        Assert.Single(accounts);
        
        Assert.Equal(accounts[0].CurrencyId, request.CurrencyId);
        Assert.Equal(accounts[0].UserId, request.UserId);
        Assert.Equal(accounts[0].Sum, request.Sum);
    }
    
    [Fact]
    public async Task TopUp_UpdateAccount_IfExists()
    {
        // Arrange
        const decimal requestSum = 100;
        var request = _fixture.Build<TopUpRequest>()
            .With(y => y.Sum, requestSum)
            .Create();

        const decimal sum = 200;
        var accounts = new List<AccountEntity>
        {
            new()
            {
                CurrencyId = request.CurrencyId,
                UserId = request.UserId,
                Sum = sum,
            }
        };
        MockDbContext(accounts);

        _topUpRequestValidator.Validate(request).Returns(new ValidationResult());
        
        var sut = _fixture.Create<AccountsHandler>();

        // Act
        await sut.TopUp(request, CancellationToken.None);

        // Assert
        Assert.Single(accounts);
        
        Assert.Equal(accounts[0].CurrencyId, request.CurrencyId);
        Assert.Equal(accounts[0].UserId, request.UserId);
        Assert.Equal(accounts[0].Sum, requestSum + sum);
    }
    
    [Fact]
    public async Task TopUp_ResultSumLessThanZero_Throw()
    {
        // Arrange
        const decimal requestSum = 100;
        var request = _fixture.Build<TopUpRequest>()
            .With(y => y.Sum, requestSum)
            .Create();

        const decimal sum = -200;
        var accounts = new List<AccountEntity>
        {
            new()
            {
                CurrencyId = request.CurrencyId,
                UserId = request.UserId,
                Sum = sum,
            }
        };
        MockDbContext(accounts);

        _topUpRequestValidator.Validate(request).Returns(new ValidationResult());
        
        var sut = _fixture.Create<AccountsHandler>();

        // Act
        var action = () => sut.TopUp(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<AlertException>(action);
    }

    [Fact]
    public async Task Convert_ValidationFailed_Throw()
    {
        // Arrange
        var request = _fixture.Create<ConvertRequest>();
        
        var errorValidationResult = new ValidationResult
        {
            Errors = new List<ValidationFailure>
            {
                new("testProp", "Some error")
            }
        };
        _convertRequestValidator.Validate(request).Returns(errorValidationResult);

        var sut = _fixture.Create<AccountsHandler>();
        
        // Act
        var action = () => sut.Convert(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<AlertException>(action);
    }
    
    [Fact]
    public async Task Convert_NotExistsAccountTo_Throw()
    {
        // Arrange
        var request = _fixture.Create<ConvertRequest>();

        var accounts = new List<AccountEntity>();
        MockDbContext(accounts);

        _convertRequestValidator.Validate(request).Returns(new ValidationResult());
        
        var sut = _fixture.Create<AccountsHandler>();

        // Act
        var action = () => sut.Convert(request, CancellationToken.None);
        
        // Assert
        await Assert.ThrowsAsync<AlertException>(action);
    }

    [Fact]
    public async Task Convert_CreateAccountTo_IfNotExists()
    {
        // Arrange
        var request = _fixture.Build<ConvertRequest>()
            .With(y => y.CommissionPercent, 20)
            .Create();

        var accounts = new List<AccountEntity>
        {
            new()
            {
                CurrencyId = request.CurrencyIdFrom,
                UserId = request.UserId,
                Sum = request.SumFrom,
            }
        };
        MockDbContext(accounts);

        _convertRequestValidator.Validate(request).Returns(new ValidationResult());
        
        var sut = _fixture.Create<AccountsHandler>();

        // Act
        await sut.Convert(request, CancellationToken.None);
        
        // Assert
        Assert.Equal(2, accounts.Count);

        var newAccount = accounts
            .FirstOrDefault(y => y.UserId == request.UserId && y.CurrencyId == request.CurrencyIdTo);
        Assert.NotNull(newAccount);
    }
    
    [Fact]
    public async Task Convert_UpdateAccountTo_IfExists()
    {
        // Arrange
        var request = _fixture.Build<ConvertRequest>()
            .With(y => y.CommissionPercent, 20)
            .Create();

        var accounts = new List<AccountEntity>
        {
            new()
            {
                CurrencyId = request.CurrencyIdFrom,
                UserId = request.UserId,
                Sum = request.SumFrom,
            },
            new()
            {
                CurrencyId = request.CurrencyIdTo,
                UserId = request.UserId,
            }
        };
        MockDbContext(accounts);

        _convertRequestValidator.Validate(request).Returns(new ValidationResult());
        
        var sut = _fixture.Create<AccountsHandler>();

        // Act
        await sut.Convert(request, CancellationToken.None);
        
        // Assert
        Assert.Equal(2, accounts.Count);
    }

    [Fact]
    public async Task Convert_NotEnoughMoneyInAccountTo_Throw()
    {
        var request = _fixture.Build<ConvertRequest>()
            .With(y => y.SumFrom, 150)
            .With(y => y.CommissionPercent, 20)
            .Create();

        var accounts = new List<AccountEntity>
        {
            new()
            {
                CurrencyId = request.CurrencyIdFrom,
                UserId = request.UserId,
                Sum = 100,
            }
        };
        MockDbContext(accounts);

        _convertRequestValidator.Validate(request).Returns(new ValidationResult());
        
        var sut = _fixture.Create<AccountsHandler>();

        // Act
        var action = () => sut.Convert(request, CancellationToken.None);
        
        // Assert
        await Assert.ThrowsAsync<AlertException>(action);
    }

    [Theory]
    [InlineData(100, 0, 50, 0.5, 4, 50, 24)]
    [InlineData(150, 50, 50, 2, 10, 100, 140)]
    [InlineData(200, 50, 200, 0.1, 1, 0, 69.8)]
    public async Task Convert_SumCalculation(
        decimal sumAccountFrom,
        decimal sumAccountTo,
        decimal convertSum,
        decimal rate,
        double commission,
        decimal resultSumAccountFrom,
        decimal resultSumAccountTo)
    {
        // Arrange
        var request = _fixture.Build<ConvertRequest>()
            .With(y => y.SumFrom, convertSum)
            .With(y => y.Rate, rate)
            .With(y => y.CommissionPercent, commission)
            .Create();

        var accounts = new List<AccountEntity>
        {
            new()
            {
                CurrencyId = request.CurrencyIdFrom,
                UserId = request.UserId,
                Sum = sumAccountFrom,
            },
            new()
            {
                CurrencyId = request.CurrencyIdTo,
                UserId = request.UserId,
                Sum = sumAccountTo,
            },
        };
        MockDbContext(accounts);

        _convertRequestValidator.Validate(request).Returns(new ValidationResult());
        
        var sut = _fixture.Create<AccountsHandler>();

        // Act
        await sut.Convert(request, CancellationToken.None);
        
        // Assert
        var fromAccount = accounts
            .First(y => y.CurrencyId == request.CurrencyIdFrom && y.UserId == request.UserId);
        
        Assert.Equal(resultSumAccountFrom, fromAccount.Sum);

        var toAccount = accounts
            .First(y => y.CurrencyId == request.CurrencyIdTo && y.UserId == request.UserId);
        
        Assert.Equal(resultSumAccountTo, toAccount.Sum);
    }

    private void MockDbContext(List<AccountEntity> accounts)
    {
        _contextMock
            .Setup(y => y.Accounts)
            .ReturnsDbSet(accounts);
        _contextMock
            .Setup(y => y.Accounts.AddAsync(It.IsAny<AccountEntity>(), CancellationToken.None))
            .Callback<AccountEntity, CancellationToken>((account, token) => accounts.Add(account));
        _contextMock
            .Setup(y => y.SaveChangesAsync(CancellationToken.None))
            .Returns(Task.FromResult(1));
        var context = _contextMock.Object;
        _contextMock
            .Setup(y => y.Database)
            .Returns(new MockDatabaseFacade(context));
        _fixture.Inject(context);
    }
    
    private class MockDatabaseFacade : DatabaseFacade
    {
        public MockDatabaseFacade(DatabaseContext context) : base(context)
        {
        }

        public override Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Mock.Of<IDbContextTransaction>());
    }
}