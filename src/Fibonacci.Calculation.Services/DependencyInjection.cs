using Microsoft.Extensions.DependencyInjection;

namespace Fibonacci.Calculation.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddCalculationServices(this IServiceCollection services)
    {
        services.AddSingleton<IFibonacciCalculator, SimpleFibonacciCalculator>();
        services.AddSingleton<IFibonacciCalculationService, FibonacciCalculationService>();
        return services;
    }
}