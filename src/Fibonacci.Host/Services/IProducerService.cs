using System.Threading;
using System.Threading.Tasks;
using Fibonacci.API;

namespace Fibonacci.Host.Services;

public interface IProducerService
{
    Task PublishFibonacciMessageAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : FibonacciMessage, new();
}