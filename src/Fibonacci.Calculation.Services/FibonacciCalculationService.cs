using System;
using Fibonacci.API;

namespace Fibonacci.Calculation.Services
{
    public class FibonacciCalculationService : IFibonacciCalculationService
    {
        private readonly IFibonacciCalculator _fibonacciCalculator;

        public FibonacciCalculationService(IFibonacciCalculator fibonacciCalculator)
        {
            _fibonacciCalculator = fibonacciCalculator;
        }

        public T CalculateNext<T>(T message)
            where T : FibonacciMessage, new()
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (message.CurrentFibonacciPositionNumber == message.TargetFibonacciPositionNumber)
                return message;

            var previousFibonacciNumber = _fibonacciCalculator.CalculateNthFibonacciNumber(message.CurrentFibonacciPositionNumber - 1);
            var nextFibonacciNumber = previousFibonacciNumber + message.CurrentValue;
            var nextFibonacciPositionNumber = message.CurrentFibonacciPositionNumber + 1;

            var nextMessageToCalculate = new T
            {
                TargetFibonacciPositionNumber = message.TargetFibonacciPositionNumber,
                CurrentFibonacciPositionNumber = nextFibonacciPositionNumber,
                CurrentValue = nextFibonacciNumber
            };

            return nextMessageToCalculate;
        }
    }
}