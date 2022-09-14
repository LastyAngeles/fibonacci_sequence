using EasyNetQ;
using Fibonacci.API;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.Consumer;
using Fibonacci.Calculation.Services;
using Fibonacci.Client.Options;
using Microsoft.Extensions.Options;

namespace Fibonacci.Client.Services
{
    public class ConsumeCalculationBackgroundService : BackgroundService
    {
        private readonly IBus _bus;
        private readonly IFibonacciCalculationService _fibonacciCalculationService;
        private readonly IHostCalculationService _hostCalculationService;
        private readonly AppOptions _appOptions;
        private readonly IQueRouterService _routerService;
        private readonly ILogger<ConsumeCalculationBackgroundService> _logger;

        public ConsumeCalculationBackgroundService(IBus bus, IFibonacciCalculationService fibonacciCalculationService,
            IHostCalculationService hostCalculationService, IOptions<AppOptions> appOptions, IQueRouterService routerService,
            ILogger<ConsumeCalculationBackgroundService> logger)
        {
            _bus = bus;
            _fibonacciCalculationService = fibonacciCalculationService;
            _hostCalculationService = hostCalculationService;
            _appOptions = appOptions.Value;
            _routerService = routerService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var declaredQues = await _routerService.Initialize(_appOptions.ThreadCount, cancellationToken);

            foreach (var que in declaredQues)
                _bus.Advanced.Consume(que, x => x.Add<FibonacciMessage>((message, _) => HandleMessage(message.Body, cancellationToken)));
        }

        private void HandleMessage(FibonacciMessage message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Message from host retrieved. Current Fibonacci number is: {FibonacciNumber}",
                message.CurrentValue);

            var nextMessageToCalculate = _fibonacciCalculationService.CalculateNext(message);

            if (nextMessageToCalculate.CurrentFibonacciPositionNumber == nextMessageToCalculate.TargetFibonacciPositionNumber)
            {
                _logger.LogInformation("Fibonacci number calculation finished, Fibonacci number is: {fibonacciNumber}",
                    nextMessageToCalculate.CurrentValue);
                return;
            }

            _hostCalculationService.RequestFibonacciNumberAsync(nextMessageToCalculate, cancellationToken);
        }
    }
}
