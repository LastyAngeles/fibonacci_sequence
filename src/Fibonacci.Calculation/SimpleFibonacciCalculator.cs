namespace Fibonacci.Calculation;

public class SimpleFibonacciCalculator : IFibonacciCalculator
{
    public int CalculateNthFibonacciNumber(int n)
    {
        if (n < 0)
            return 0;

        if (n <= 1)
            return n;

        return CalculateNthFibonacciNumber(n - 1) + CalculateNthFibonacciNumber(n - 2);
    }
}