using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Fibonacci.Calculation.Test
{
    public class FibonacciCalculationTest
    {
        private readonly SimpleFibonacciCalculator _simpleFibonacciCalculator;

        public FibonacciCalculationTest()
        {
            _simpleFibonacciCalculator = new SimpleFibonacciCalculator();
        }

        [Theory]
        [MemberData(nameof(GetFibonacciNumberAndPosition))]
        public void FibonacciCalculationAppliesCorrectlyTest(int number, int position)
        {
            // Act
            var ret = _simpleFibonacciCalculator.CalculateNthFibonacciNumber(position);

            // Assert
            ret.Should().Be(number);
        }

        [Fact]
        public void FibonacciCalculationWithNegativeNumberReturnZeroTest()
        {
            // Arrange
            var n = -100;

            // Act
            var ret = _simpleFibonacciCalculator.CalculateNthFibonacciNumber(n);

            // Assert
            ret.Should().Be(0);
        }

        #region TestData
        public static IEnumerable<object[]> GetFibonacciNumberAndPosition()
        {
            return new List<object[]>
            {
                new object[] { 0, 0 },
                new object[] { 1, 1 },
                new object[] { 1, 2 },
                new object[] { 2, 3 },
                new object[] { 3, 4 },
                new object[] { 5, 5 },
                new object[] { 8, 6 },
                new object[] { 13, 7 },
                new object[] { 21, 8 },
                new object[] { 34, 9 },
                new object[] { 55, 10 },
                new object[] { 89, 11 },
                new object[] { 144, 12 },
                new object[] { 233, 13 },
                new object[] { 377, 14 },
                new object[] { 610, 15 }
            };
        }
        #endregion
    }
}