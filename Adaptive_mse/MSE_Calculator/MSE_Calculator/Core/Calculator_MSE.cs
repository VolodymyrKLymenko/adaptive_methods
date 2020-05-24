using PoohMathParser;
using System;

namespace MSE_Calculator.Core
{
    public class MSE_Calculator
    {
        private readonly MathExpression _miu;
        private readonly MathExpression _beta;
        private readonly MathExpression _sigma;
        private readonly MathExpression _f;

        private readonly double _a;
        private readonly double _b;

        private readonly double _ua;
        private readonly double _ub;

        public MSE_Calculator(
            MathExpression miu,
            MathExpression beta,
            MathExpression sigma,
            MathExpression f,
            double a, double b, double ua, double ub)
        {
            _miu = miu;
            _beta = beta;
            _sigma = sigma;
            _f = f;
            _a = a;
            _b = b;
            _ua = ua;
            _ub = ub;
        }

        public (double[], double[]) CalculateApproximation(int N)
        {
            double h = (_b - _a) / N;
            double[] discret_x = new double[N];
            for (double xi = _a, i = 0; i < N; i++, xi += h)
            {
                discret_x[(int)i] = xi;
            }

            double[] L = new double[N];
            double[][] A = new double[N][];
            for (int i = 0; i < N; i++)
            {
                A[i] = new double[N];

                if (i > 0)
                {
                    Func<double, double> a__i_minus_1__i = new Func<double, double>((x) => {
                        double phi_i = kurant_function(i, x);
                        double phi_i_minus_1 = kurant_function(i - 1, x);

                        double derivative_phi_i = derivative_kurant_function(i, x);
                        double derivative_phi_i_minus_1 = derivative_kurant_function(i - 1, x);

                        return _miu.Calculate(x) * derivative_phi_i_minus_1 * derivative_phi_i +
                                        _beta.Calculate(x) * derivative_phi_i_minus_1 * phi_i +
                                            _sigma.Calculate(x) * phi_i_minus_1 * phi_i;
                    });

                    A[i][i - 1] = IntegralCalculator.Integrate(a__i_minus_1__i, discret_x[i - 1], discret_x[i]);
                }

                Func<double, double> a__i__i = new Func<double, double>((x) => {
                    double phi_i = kurant_function(i, x);

                    double derivative_phi_i = derivative_kurant_function(i, x);

                    return _miu.Calculate(x) * derivative_phi_i * derivative_phi_i +
                                    _beta.Calculate(x) * derivative_phi_i * phi_i +
                                        _sigma.Calculate(x) * phi_i * phi_i;
                });

                A[i][i] = IntegralCalculator.Integrate(
                    a__i__i,
                    i > 0 ? discret_x[i - 1] : _a,
                    (i < N - 1) ? discret_x[i + 1] : _b);

                if (i < N - 1)
                {
                    Func<double, double> a__i__i_plus_1 = new Func<double, double>((x) => {
                        double phi_i = kurant_function(i, x);
                        double phi_i_plus_1 = kurant_function(i + 1, x);

                        double derivative_phi_i = derivative_kurant_function(i, x);
                        double derivative_phi_i_plus_1 = derivative_kurant_function(i + 1, x);

                        return _miu.Calculate(x) * derivative_phi_i * derivative_phi_i_plus_1 +
                                        _beta.Calculate(x) * derivative_phi_i * phi_i_plus_1 +
                                            _sigma.Calculate(x) * phi_i * phi_i_plus_1;
                    });

                    A[i][i + 1] = IntegralCalculator.Integrate(a__i__i_plus_1, discret_x[i], discret_x[i + 1]);
                }

                Func<double, double> l_func = new Func<double, double>((x) => _f.Calculate(x) * kurant_function(i, x));
                L[i] = IntegralCalculator.Integrate(l_func, _a, _b);
            }

            double[] q = MatrixCalculation.ProhoncaCalculation(A, L);

            var res = new Func<double, double>(x =>
            {
                var sum = 0.0;
                for (int i = 0; i < N; i++)
                {
                    sum += q[i] * kurant_function(i, x);
                }

                return _ua + sum;
            });

            var resVec = new double[N];
            for (int i = 0; i < N; i++)
            {
                resVec[i] = res(discret_x[i]);
            }

            return (discret_x, resVec);

            double kurant_function(int i, double x)
            {
                if (i == 0 || i == N - 1)
                {
                    return 1;
                }
                else if (x >= 0 && x < discret_x[i - 1])
                {
                    return 0;
                }
                else if (x > discret_x[i - 1] && x <= discret_x[i])
                {
                    return ((x - discret_x[i - 1]) / h);
                }
                else if (x > discret_x[i] && x <= discret_x[i + 1])
                {
                    return ((discret_x[i + 1] - x) / h);
                }
                else
                {
                    return 0;
                }
            }
            double derivative_kurant_function(int i, double x)
            {
                if (i == 0 || i == N - 1)
                {
                    return 1;
                }
                else if (x >= 0 && x < discret_x[i - 1])
                {
                    return 0;
                }
                else if (x > discret_x[i - 1] && x <= discret_x[i])
                {
                    return (1 / h);
                }
                else if (x > discret_x[i] && x <= discret_x[i + 1])
                {
                    return ((-1) / h);
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
