using Fibonacci.Client.Models;
using Fibonacci.Client.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Fibonacci.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ModelExceptionFilter]
    public class FibonacciController : ControllerBase
    {
        private readonly IHostCalculationService _hostCalculationService;
        private readonly ILogger<FibonacciController> _logger;

        public FibonacciController(IHostCalculationService hostCalculationService, ILogger<FibonacciController> logger)
        {
            _hostCalculationService = hostCalculationService;
            _logger = logger;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartCalculationAsync([FromBody] EntryCalculationModel calculationModel, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Start calculation of Fibonacci number. Requested number position is: '{fibonacciNumberPosition}'", calculationModel.FibonacciNumberPosition);

            await _hostCalculationService.StartCalculationAsync(calculationModel, cancellationToken);

            return Ok();
        }
    }
}
