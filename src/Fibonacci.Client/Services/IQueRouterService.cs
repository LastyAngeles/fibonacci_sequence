using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.Topology;

namespace Fibonacci.Client.Services;

public interface IQueRouterService
{
    Task<IList<Queue>> Initialize(int queCount, CancellationToken cancellationToken = default);
    IList<string> GetActiveRoutingKeys();
}