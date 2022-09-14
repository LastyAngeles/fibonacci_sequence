using Fibonacci.API;
using Fibonacci.Client.Models;
using Fibonacci.Client.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Fibonacci.Client.Services
{
    public class HostCalculationService : IHostCalculationService
    {
        private readonly HttpClient _httpClient;
        private readonly AppOptions _appOptions;
        private readonly ILogger<HostCalculationService> _logger;

        public HostCalculationService(IHttpClientFactory httpClientFactory, ILogger<HostCalculationService> logger,
            IOptions<AppOptions> appOptions)
        {
            _httpClient = httpClientFactory.CreateClient("FibonacciHttpClient");
            _logger = logger;
            _appOptions = appOptions.Value;
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
                CurrentFibonacciPositionNumber = 1,
                CurrentValue = 1
            };

            await RequestFibonacciNumberAsync(message, cancellationToken);
        }
    }
}