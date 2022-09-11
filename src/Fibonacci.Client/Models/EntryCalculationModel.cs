using System.ComponentModel.DataAnnotations;

namespace Fibonacci.Client.Models
{
    public class EntryCalculationModel
    {
        [Required]
        [Range(minimum: 0, maximum: short.MaxValue)]
        public short FibonacciNumberPosition { get; set; }
    }
}