using Fibonacci.API;

namespace Fibonacci.Calculation.Services;

public interface IFibonacciCalculationService
{
    T CalculateNext<T>(T message) where T : FibonacciMessage, new();
}