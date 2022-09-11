using Fibonacci.API;
using Fibonacci.Client.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Fibonacci.Client.Services
{
    public interface IHostCalculationService
    {
        Task StartCalculationAsync(EntryCalculationModel model, CancellationToken cancellationToken = default);

        Task RequestFibonacciNumberAsync(FibonacciMessage message, CancellationToken cancellationToken = default);
    }
}