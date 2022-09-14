using System.ComponentModel.DataAnnotations;

namespace Fibonacci.API
{
    public record FibonacciMessage
    {
        [Required]
        public int TargetFibonacciPositionNumber { get; init; }

        public int CurrentFibonacciPositionNumber { get; init; }

        public int CurrentValue { get; init; }

        [Required]
        public string? RoutingKey { get; init; }
    }
}