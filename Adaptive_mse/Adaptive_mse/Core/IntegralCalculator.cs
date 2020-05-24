using PoohMathParser;
using System;

namespace MSE_Calculator.Core
{
    public static class IntegralCalculator
    {
        public static int percision = 1000;

        public static double Integrate(MathExpression function, double lowerBound, double higherBound)
        {
            double h = (higherBound - lowerBound) / percision;

            double result = 0;

            for (double i = lowerBound; i < higherBound; i += h)
            {
                double a = function.Calculate(i);
                double b = function.Calculate(i + h);

                double S = ((a + b) / 2) * h;
                result += S;
            }

            return result;
        }

        public static double Integrate(Func<double, double> function, double lowerBound, double higherBound)
        {
            double h = (higherBound - lowerBound) / percision;

            double result = 0;

            for (double i = lowerBound; i < higherBound; i += h)
            {
                double a = function(i);
                double b = function(i + h);

                double S = ((a + b) / 2) * h;
                result += S;
            }

            return result;
        }
    }
}
