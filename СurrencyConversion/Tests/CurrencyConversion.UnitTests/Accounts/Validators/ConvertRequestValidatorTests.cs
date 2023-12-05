using AutoFixture;
using CurrencyConversion.Api.Contracts.Accounts.Requests;
using CurrencyConversion.Services.Accounts.Validators;
using Xunit;

namespace CurrencyConversion.UnitTests.Accounts.Validators;

public class ConvertRequestValidatorTests
{
    private readonly IFixture _fixture;
    private readonly ConvertRequestValidator _sut;

    public ConvertRequestValidatorTests()
    {
        _fixture = new Fixture();
        _sut = new ConvertRequestValidator();
    }
    
    [Theory]
    [InlineData(-2, 2, 5, false)]
    [InlineData(0, 2, 5, false)]
    [InlineData(5, -2, 5, false)]
    [InlineData(5, 0, 5, false)]
    [InlineData(5, 2, -5, false)]
    [InlineData(5, 2, 0, false)]
    [InlineData(5, 2, 101, false)]
    [InlineData(1000, 2, 50, true)]
    [InlineData(500, 100, 90, true)]
    [InlineData(5, 2, 100, true)]
    public void Validate_ResultIsNotValid_IdParametersNotCorrect(
        decimal sumFrom,
        decimal rate,
        double? commission,
        bool isValid)
    {
        // Arrange
        var request = _fixture.Build<ConvertRequest>()
            .With(y => y.SumFrom, sumFrom)
            .With(y => y.Rate, rate)
            .With(y => y.CommissionPercent, commission)
            .Create();

        // Act
        var validationResult = _sut.Validate(request);

        // Assert
        Assert.Equal(isValid, validationResult.IsValid);
    }
}