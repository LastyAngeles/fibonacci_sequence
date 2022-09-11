using Fibonacci.API;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Fibonacci.Host.Services;

namespace Fibonacci.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ModelExceptionFilter]
    public class FibonacciController : ControllerBase
    {
        private readonly IProducerService _producerService; 

        public FibonacciController(IProducerService producerService)
        {
            _producerService = producerService;
        }

        [HttpGet("number")]
        public async Task<IActionResult> GetFibonacciNumberAsync([FromQuery] FibonacciMessage message, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(message);

            await _producerService.PublishFibonacciMessageAsync(message, cancellationToken);

            return Ok();
        }
    }
}