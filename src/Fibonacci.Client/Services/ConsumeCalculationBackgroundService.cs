using EasyNetQ;
using Fibonacci.API;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Fibonacci.Calculation.Services;

namespace Fibonacci.Client.Services
{
    public class ConsumeCalculationBackgroundService : BackgroundService
    {
        private readonly IBus _bus;
        private readonly IFibonacciCalculationService _fibonacciCalculationService;
        private readonly IHostCalculationService _hostCalculationService;
        private readonly ILogger<ConsumeCalculationBackgroundService> _logger;

        public ConsumeCalculationBackgroundService(IBus bus, IFibonacciCalculationService fibonacciCalculationService, IHostCalculationService hostCalculationService, ILogger<ConsumeCalculationBackgroundService> logger)
        {
            _bus = bus;
            _fibonacciCalculationService = fibonacciCalculationService;
            _hostCalculationService = hostCalculationService;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
           => _bus.PubSub.SubscribeAsync<FibonacciMessage>("FibonacciCalculationQueue", HandleMessageAsync, _ => { }, cancellationToken: stoppingToken);

        private async Task HandleMessageAsync(FibonacciMessage message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Message from host retrieved. Current Fibonacci number is: {FibonacciNumber}", message.CurrentValue);

            var nextMessageToCalculate = _fibonacciCalculationService.CalculateNext(message);

            if (nextMessageToCalculate.CurrentFibonacciPositionNumber == nextMessageToCalculate.TargetFibonacciPositionNumber)
            {
                _logger.LogInformation("Fibonacci number calculation finished, fibonacci number is: {fibonacciNumber}", nextMessageToCalculate.CurrentValue);
                return;
            }

            await _hostCalculationService.RequestFibonacciNumberAsync(nextMessageToCalculate, cancellationToken);
        }
    }
}
