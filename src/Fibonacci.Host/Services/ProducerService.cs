using System;
using EasyNetQ;
using System.Threading;
using System.Threading.Tasks;
using Fibonacci.API;
using Fibonacci.Calculation.Services;
using Microsoft.Extensions.Logging;

namespace Fibonacci.Host.Services;

public class ProducerService : IProducerService
{
    private readonly IBus _bus;
    private readonly IFibonacciCalculationService _fibonacciCalculationService;
    private readonly ILogger<ProducerService> _logger;

    public ProducerService(IBus bus, IFibonacciCalculationService fibonacciCalculationService, ILogger<ProducerService> logger)
    {
        _bus = bus;
        _fibonacciCalculationService = fibonacciCalculationService;
        _logger = logger;
    }

    public Task PublishFibonacciMessageAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : FibonacciMessage, new()
    {
        if (message is null)
        {
            var error = new ArgumentNullException(nameof(message));
            _logger.LogError(error, "Calculation model is null.");
            throw error;
        }

        var nextMessage = _fibonacciCalculationService.CalculateNext(message);

        if (nextMessage.CurrentFibonacciPositionNumber == nextMessage.TargetFibonacciPositionNumber)
            _logger.LogInformation("Fibonacci number calculation finished. Fibonacci number is: {fibonacciNumber}", nextMessage.CurrentValue);

        return _bus.PubSub.PublishAsync(nextMessage, cancellationToken);
    }
}