using PoohMathParser;
using System;
using System.Collections.Generic;

namespace Adaptive_MSE.Core
{
    public class Calculator_MSE
    {
        private readonly MathExpression _miu;
        private readonly MathExpression _beta;
        private readonly MathExpression _sigma;
        private readonly MathExpression _f;

        private readonly double _a;
        private readonly double _b;

        private readonly double _ua;
        private readonly double _ub;

        public Calculator_MSE(
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

        public CalculationResult CalculateApproximation(int N)
        {
            N = 5;
            double h = ((_b - _a) / (N - 1));
            List<double> discret_x = new List<double>();
            for (double xi = _a, i = 0; i < N; i++, xi += h)
            {
                discret_x.Add(xi);
            }
            discret_x[N - 1] = _b;

            /*List<double> res = new List<double> { 0.0, 1.2126, 1.35124, 1.4543, 0.0 };

            discret_x.Insert(1, (discret_x[1] + discret_x[0]) / 2.0);
            res.Insert(1, 1.1614);
            discret_x.Insert(1, (discret_x[1] + discret_x[0]) / 2.0);
            res.Insert(1, 1.12914);
            discret_x.Insert(1, (discret_x[1] + discret_x[0]) / 2.0);
            res.Insert(1, 1.11314);
            discret_x.Insert(1, (discret_x[1] + discret_x[0]) / 2.0);
            res.Insert(1, 1.01314);


            discret_x.Insert(8, (discret_x[7] + discret_x[8]) / 2.0);
            res.Insert(8, 1.68214);
            discret_x.Insert(9, (discret_x[8] + discret_x[9]) / 2.0);
            res.Insert(9, 1.80214);
            discret_x.Insert(10, (discret_x[9] + discret_x[10]) / 2.0);
            res.Insert(10, 1.84214);
            discret_x.Insert(11, (discret_x[10] + discret_x[11]) / 2.0);
            res.Insert(11, 1.78214);


            return new CalculationResult(discret_x.ToArray(), res.ToArray());*/

            return CalculateApproximation(N, discret_x.ToArray());
        }

        private CalculationResult CalculateApproximation(int N, double[] discret_x)
        {
            double[] L = new double[N];
            double[][] A = new double[N][];
            for (int i = 0; i < N; i++)
            {
                A[i] = new double[N];

                if (i > 0)
                {
                    Func<double, double> a__i_minus_1__i = new Func<double, double>((x) =>
                    {
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

                Func<double, double> a__i__i = new Func<double, double>((x) =>
                {
                    double phi_i = kurant_function(i, x);

                    double derivative_phi_i = derivative_kurant_function(i, x);

                    return _miu.Calculate(x) * derivative_phi_i * derivative_phi_i +
                                    _beta.Calculate(x) * derivative_phi_i * phi_i +
                                        _sigma.Calculate(x) * phi_i * phi_i;
                });

                A[i][i] = IntegralCalculator.Integrate(a__i__i, i > 0 ? discret_x[i - 1] : _a, (i < N - 1) ? discret_x[i + 1] : _b);

                if (i < N - 1)
                {
                    Func<double, double> a__i__i_plus_1 = new Func<double, double>((x) =>
                    {
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
                L[i] = IntegralCalculator.Integrate(l_func, _a, _b) /*+ _ua * _miu.Calculate(1) * kurant_function(i, 1)*/;
            }

            //A = GenerateAMatrix(N, _miu, _beta, _sigma);

            //L = GenerateFVector(N, _f);

            double[] q = MatrixCalculation.ProhoncaCalculation(A, L);

            var res = new Func<double, double>(x =>
            {
                var sum = 0.0;
                for (int i = 0; i < N; i++)
                {
                    sum += (q[i] * kurant_function(i, x));
                }

                return _ua + sum;
            });

            var resVec = new double[N];
            for (int i = 0; i < N; i++)
            {
                if (i == N - 1)
                {
                    resVec[i] = 0.0;
                }
                else
                {
                    resVec[i] = res(discret_x[i]);
                }
            }

            return new CalculationResult(discret_x, resVec);

            double kurant_function(int i, double x)
            {
                if (i == 0)
                {
                    return 0;
                }
                if (i == N - 1)
                {
                    return 0;
                }

                if (x >= _a && x <= discret_x[i - 1])
                {
                    return 0;
                }
                else if (x > discret_x[i - 1] && x <= discret_x[i])
                {
                    return ((x - discret_x[i + 1]) / (discret_x[i + 1] - discret_x[i]));
                    //return ((x - discret_x[i - 1]) / (discret_x[i + 1] - discret_x[i]));
                }
                else if (x > discret_x[i] && x <= discret_x[i + 1])
                {
                    return (1 - ((x - discret_x[i + 1]) / (discret_x[i + 1] - discret_x[i])));
                    //return ((discret_x[i + 1] - x) / (discret_x[i + 1] - discret_x[i]));
                }
                else
                {
                    return 0;
                }
            }
            double derivative_kurant_function(int i, double x)
            {
                if (i == 0)
                {
                    return 0;
                }
                else if (i == N - 1)
                {
                    return 0;
                }
                else if (x >= _a && x <= discret_x[i - 1])
                {
                    return 0;
                }
                else if (x > discret_x[i - 1] && x <= discret_x[i])
                {
                    return (1 / (discret_x[i + 1] - discret_x[i]));
                }
                else if (x > discret_x[i] && x <= discret_x[i + 1])
                {
                    return -(1 / (discret_x[i + 1] - discret_x[i]));
                }
                else
                {
                    return 0;
                }
            }
        }

        private double[][] GenerateAMatrix(int N, MathExpression tFunc, MathExpression bFunc, MathExpression sigmaFunc)
        {
            double h = 1.0 / N;

            double[][] res = new double[N][];
            for (int i = 0; i < N; i++)
            {
                res[i] = new double[N];
                for (int j = 0; j < N; j++)
                {
                    if (i != N - 1)
                    {
                        if (i == j)
                        {
                            res[i][j] = (1.0 / h) * (tFunc.Calculate(h * (i + 1) - (h / 2.0)) + tFunc.Calculate(h * (i + 1) + (h / 2.0))) +
                                        (h / 3.0) *
                                        (sigmaFunc.Calculate(h * (i + 1) - (h / 2.0)) + sigmaFunc.Calculate(h * (i + 1) + (h / 2.0))) +
                                        (1.0 / 2.0) * (bFunc.Calculate(h * (i + 1) - (h / 2.0)) - bFunc.Calculate(h * (i + 1) + (h / 2.0)));
                        }
                        else if (j == i + 1)
                        {
                            res[i][j] = -(1.0 / h) * tFunc.Calculate(h * (i + 1) + (h / 2.0)) +
                                (1.0 / 2.0) * bFunc.Calculate(h * (i + 1) + (h / 2.0)) +
                                        (h / 6.0) * sigmaFunc.Calculate(h * (i + 1) + (h / 2.0));
                        }
                        else if (j == i - 1)
                        {
                            res[i][j] = -(1.0 / h) * tFunc.Calculate(h * (i + 1) - (h / 2.0)) -
                                (1.0 / 2.0) * bFunc.Calculate(h * (i + 1) - (h / 2.0)) +
                                        (h / 6.0) * sigmaFunc.Calculate(h * (i + 1) - (h / 2.0));
                        }
                        else
                        {
                            res[i][j] = 0;
                        }
                    }
                    else
                    {
                        if (j < N - 2)
                        {
                            res[i][j] = 0;
                        }
                        else if (j == N - 2)
                        {
                            res[i][j] = -(1.0 / h) * tFunc.Calculate(h * (i + 1) - (h / 2.0)) -
                                (1.0 / 2.0) * bFunc.Calculate(h * (i + 1) - (h / 2.0)) +
                                        (h / 6.0) * sigmaFunc.Calculate(h * (i + 1) - (h / 2.0));
                        }
                        else if (j == N - 1)
                        {
                            res[i][j] = (1.0 / h) * tFunc.Calculate(h * (i + 1) - (h / 2.0)) +
                                (1.0 / 2.0) * bFunc.Calculate(h * (i + 1) - (h / 2.0)) +
                                (h / 3.0) * sigmaFunc.Calculate(h * (i + 1) - (h / 2.0));
                        }
                    }
                }
            }

            return res;

        }

        public double[] GenerateFVector(int N, MathExpression fFunc)
        {
            double h = 1.0 / N;
            double[] res = new double[N];
            for (int i = 0; i < N - 1; i++)
            {
                res[i] = (h / 2.0) * (fFunc.Calculate(h * (i + 1) - (h / 2.0)) + fFunc.Calculate(h * (i + 1) + (h / 2.0)));
            }
            res[N - 1] = (h / 2.0) * (fFunc.Calculate(h * N - (h / 2.0))) /*- q*/;
            return res;
        }
    }

    public class CalculationResult
    {
        public double[] XValues { get; set; }
        public double[] UValues { get; set; }

        public CalculationResult(double[] xValues, double[] uValues)
        {
            XValues = xValues;
            UValues = uValues;
        }
    }
}
