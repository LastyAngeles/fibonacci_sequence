using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;

namespace Fibonacci.Client.Services
{
    public class QueRouterService : IQueRouterService
    {
        private readonly IBus _bus;
        //TODO: think about storing those values in redis cache, for example (Maxim Meshkov 14.09.22)
        //TODO: such cache would simplify things a lot (Maxim Meshkov 14.09.22)
        private readonly IList<string> _declaredRoutingKeys = new List<string>();

        public QueRouterService(IBus bus)
        {
            _bus = bus;
        }

        public async Task<IList<Queue>> Initialize(int queCount, CancellationToken cancellationToken = default)
        {
            var exchange = await _bus.Advanced.ExchangeDeclareAsync("Fibonacci.Exchange", ExchangeType.Direct, cancellationToken: cancellationToken);
            return await DeclareQuesAsync(exchange, queCount, cancellationToken);
        }

        private async Task<IList<Queue>> DeclareQuesAsync(Exchange exchange, int queCount, CancellationToken cancellationToken = default)
        {
            var ques = new List<Queue>();
            for (var i = 0; i < queCount; i++)
            {
                var que = await _bus.Advanced.QueueDeclareAsync($"Fibonacci.Que.{i}", true, false, false, cancellationToken);
                var routingKey = $"Route.{i}";
                await _bus.Advanced.BindAsync(exchange, que, routingKey, cancellationToken);
                _declaredRoutingKeys.Add(routingKey);
                ques.Add(que);
            }
            return ques;
        }

        public IList<string> GetActiveRoutingKeys() => _declaredRoutingKeys;
    }
}