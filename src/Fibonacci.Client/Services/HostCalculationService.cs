using Fibonacci.API;
using Fibonacci.Client.Models;
using Fibonacci.Client.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Fibonacci.Client.Services
{
    public class HostCalculationService : IHostCalculationService
    {
        private readonly HttpClient _httpClient;
        private readonly IQueRouterService _routerService;
        private readonly AppOptions _appOptions;
        private readonly ILogger<HostCalculationService> _logger;

        public HostCalculationService(IHttpClientFactory httpClientFactory, IQueRouterService routerService,
            IOptions<AppOptions> appOptions, ILogger<HostCalculationService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("FibonacciHttpClient");
            _routerService = routerService;
            _appOptions = appOptions.Value;
            _logger = logger;
        }

        public async Task RequestFibonacciNumberAsync(FibonacciMessage message,
            CancellationToken cancellationToken = default)
        {
            var uri = BuildUri();
            var response = await _httpClient.GetAsync(uri, cancellationToken);

            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("Failed to retrieve data from host. Status code is: {StatusCode}",
                    response.StatusCode);

            Uri BuildUri()
            {
                var uriBuilder = new UriBuilder($"{_appOptions.BaseHostUri}/api/fibonacci/number");
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                query[nameof(FibonacciMessage.TargetFibonacciPositionNumber)] =
                    message.TargetFibonacciPositionNumber.ToString();
                query[nameof(FibonacciMessage.CurrentFibonacciPositionNumber)] =
                    message.CurrentFibonacciPositionNumber.ToString();
                query[nameof(FibonacciMessage.CurrentValue)] = message.CurrentValue.ToString();
                query[nameof(FibonacciMessage.RoutingKey)] = message.RoutingKey;

                uriBuilder.Query = query.ToString();
                return uriBuilder.Uri;
            }
        }

        public async Task StartCalculationAsync(EntryCalculationModel model,
            CancellationToken cancellationToken = default)
        {
            if (model is null)
            {
                var error = new ArgumentNullException(nameof(model));
                _logger.LogError(error, "Calculation model is null.");
                throw error;
            }

            if (model.FibonacciNumberPosition is 0 or 1)
            {
                _logger.LogInformation("Fibonacci number calculation finished. Fibonacci number is: {fibonacciNumber}",
                    model.FibonacciNumberPosition);
                return;
            }

            var message = new FibonacciMessage
            {
                TargetFibonacciPositionNumber = model.FibonacciNumberPosition,
                CurrentFibonacciPositionNumber = CalculationConst.InitialFibonacciPosition,
                CurrentValue = CalculationConst.InitialFibonacciValue
            };

            var routingQues = _routerService.GetActiveRoutingKeys();

            if (routingQues.Count != _appOptions.ThreadCount)
                _logger.LogWarning(
                    "{RoutingService} must produce ques according to the number of declared threads. Current ques number: {QueNumber}. Current thread number: {ThreadNumber}",
                    nameof(IQueRouterService), routingQues.Count, _appOptions.ThreadCount);

            var calculationTasks = routingQues.Select(routingKey =>
                RequestFibonacciNumberAsync(message with { RoutingKey = routingKey }, cancellationToken));

            await Task.WhenAll(calculationTasks);
        }
    }
}