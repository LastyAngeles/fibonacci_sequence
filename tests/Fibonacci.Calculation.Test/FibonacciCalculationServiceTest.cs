using System;
using Fibonacci.API;
using Fibonacci.Calculation.Services;
using FluentAssertions;
using Xunit;

namespace Fibonacci.Calculation.Test;

public class FibonacciCalculationServiceTest
{
    private readonly IFibonacciCalculationService _fibonacciCalculationService;

    public FibonacciCalculationServiceTest()
    {
        _fibonacciCalculationService = new FibonacciCalculationService(new SimpleFibonacciCalculator());
    }

    [Fact]
    public void CalculateNextAppliesCorrectlyTest()
    {
        // Arrange
        var initMessage = new FibonacciMessage
            { CurrentFibonacciPositionNumber = 1, CurrentValue = 1, TargetFibonacciPositionNumber = 5 };

        // Act
        var calculationResult = _fibonacciCalculationService.CalculateNext(initMessage);

        // Assert
        calculationResult.Should().BeEquivalentTo(new FibonacciMessage
            { CurrentFibonacciPositionNumber = 2, CurrentValue = 1, TargetFibonacciPositionNumber = 5 });
    }

    [Fact]
    public void NullCalculationArgumentProduceErrorTest()
    {
        // Act
        var act = () => _fibonacciCalculationService.CalculateNext<FibonacciMessage>(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void EquivalentTargetAndCurrentPositionWouldNotTriggerFurtherCalculationTest()
    {
        // Arrange
        var initMessage = new FibonacciMessage
            { CurrentFibonacciPositionNumber = 5, CurrentValue = 5, TargetFibonacciPositionNumber = 5 };

        // Act
        var calculationResult = _fibonacciCalculationService.CalculateNext(initMessage);


        // Assert
        calculationResult.Should().Be(initMessage);
    }
}