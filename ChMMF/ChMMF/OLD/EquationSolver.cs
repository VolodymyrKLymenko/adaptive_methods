using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoohMathParser;
namespace Adaptive_MSE
{
    class EquationSolver
    {
        private string tFunction;
        private string sigmaFunction;
        private string fFunction;
        private string bFunction;
        private double q;
        private int n;
        public EquationSolver(string t, string b, string sigma, string f, double qValue, int nValue)
        {
            tFunction = t;
            bFunction = b;
            sigmaFunction = sigma;
            fFunction = f;
            q = qValue;
            n = nValue;
        }

        private double[][] GenerateAMatrix()
        {
            double h = 1.0 / n;
            MathExpression tFunc = new MathExpression(tFunction);
            MathExpression bFunc = new MathExpression(bFunction);
            MathExpression sigmaFunc = new MathExpression(sigmaFunction);

            double[][] res = new double[n][];
            for (int i = 0; i < n; i++)
            {
                res[i] = new double[n];
                for (int j = 0; j < n; j++)
                {
                    if (i != n - 1)
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
                                (1.0 / 2.0) * bFunc.Calculate(h * (i + 1) - (h / 2.0))+
                                        (h / 6.0) * sigmaFunc.Calculate(h * (i + 1) - (h / 2.0));
                        }
                        else
                        {
                            res[i][j] = 0;
                        }
                    }
                    else
                    {
                        if (j < n - 2)
                        {
                            res[i][j] = 0;
                        }
                        else if (j == n - 2)
                        {
                            res[i][j] = -(1.0 / h) * tFunc.Calculate(h * (i + 1) - (h / 2.0)) -
                                (1.0 / 2.0) * bFunc.Calculate(h * (i + 1) - (h / 2.0)) +
                                        (h / 6.0) * sigmaFunc.Calculate(h * (i + 1) - (h / 2.0));
                        }
                        else if (j == n - 1)
                        {
                            res[i][j] = (1.0 / h) * tFunc.Calculate(h * (i + 1) - (h / 2.0)) + 
                                (1.0 / 2.0) * bFunc.Calculate(h * (i + 1) - (h / 2.0))+
                                (h / 3.0) * sigmaFunc.Calculate(h * (i + 1) - (h / 2.0));
                        }
                    }
                }
            }

            return res;

        }
        private double[] GenerateFVector()
        {
            MathExpression fFunc = new MathExpression(fFunction);
            double h = 1.0 / n;
            double[] res = new double[n];
            for (int i = 0; i < n - 1; i++)
            {
                res[i] = (h / 2.0) * (fFunc.Calculate(h * (i + 1) - (h / 2.0)) + fFunc.Calculate(h * (i + 1) + (h / 2.0)));
            }
            res[n - 1] = (h / 2.0) * (fFunc.Calculate(h * n - (h / 2.0))) - q;
            return res;
        }

        public double[] CalculateCoeficients()
        {
            double[][] aMatrix = GenerateAMatrix();
            double[] fMatrix = GenerateFVector();
            GausMethod g = new GausMethod(n, n);
            g.RightPart = fMatrix;
            g.Matrix = aMatrix;
            g.SolveMatrix();
            return g.Answer;

        }
    }
}
