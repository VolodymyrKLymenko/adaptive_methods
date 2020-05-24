using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PoohMathParser;
namespace Adaptive_MSE
{
    class ProgonkaMethod
    {
        private string muFunction;
        private string sigmaFunction;
        private string fFunction;
        private string betaFunction;
        private double alpha;
        private double[][] aMatrix;
        private double[] fVector;
        public ProgonkaMethod(string mu, string beta, string sigma, string f, double alphaValue)
        {
            muFunction = mu;
            betaFunction = beta;
            sigmaFunction = sigma;
            fFunction = f;
            alpha = alphaValue;
        }

        private double[][] GenerateAMatrix(double[] mesh)
        {
            MathExpression muFunc = new MathExpression(muFunction);
            MathExpression betaFunc = new MathExpression(betaFunction);
            MathExpression sigmaFunc = new MathExpression(sigmaFunction);
            int n = mesh.Length - 1;
            double[][] res = new double[n][];
            double hPrev = 0.0;
            double hNext = 0.0;
            for (int i = 0; i < n; i++)
            {
                hPrev = mesh[i+1] - mesh[i];
                if (i != n - 1)
                {
                    hNext = mesh[i + 2] - mesh[i + 1];
                }
                double xiPrev = mesh[i + 1]-hPrev/2.0;
                double xiNext = mesh[i + 1] + hNext / 2.0;
                double pePrev = hPrev * betaFunc.Calculate(xiPrev) / muFunc.Calculate(xiPrev);
                double peNext = hNext * betaFunc.Calculate(xiNext) / muFunc.Calculate(xiNext);
                double shPrev = hPrev*hPrev*sigmaFunc.Calculate(xiPrev)/muFunc.Calculate(xiPrev);
                double shNext = hNext * hNext * sigmaFunc.Calculate(xiNext) / muFunc.Calculate(xiNext);
                res[i] = new double[n];
                for (int j = 0; j < n; j++)
                {
                    if (i != n - 1)
                    {
                        if (i == j)
                        {
                            res[i][j] = -(muFunc.Calculate(xiPrev)/hPrev) *
                                (-1 + (1.0 / 6.0) * pePrev*(2*shPrev+3)) -
                                (muFunc.Calculate(xiNext) / hNext) *
                                (-1 + (1.0 / 6.0) * peNext * (2*shNext - 3));
                        }
                        else if (j == i + 1)
                        {
                            res[i][j] = (muFunc.Calculate(xiNext) / hNext) *
                                (-1 + (1.0 / 6.0) * peNext * (shNext + 3));
                        }
                        else if (j == i - 1)
                        {
                            res[i][j] = (muFunc.Calculate(xiPrev)/hPrev) *
                                (-1 + (1.0 / 6.0) * pePrev*(shPrev-3));
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
                            res[i][j] = (muFunc.Calculate(xiPrev) / hPrev) *
                                (-1 + (1.0 / 6.0) * pePrev * (2 * shPrev - 3));
                        }
                        else if (j == n - 1)
                        {
                            res[i][j] = (muFunc.Calculate(xiPrev) / hPrev) *
                                (-1 + (1.0 / 6.0) * pePrev * (2*shPrev + 3))
                                +alpha;
                        }
                    }
                }
            }

            return res;

        }
        private double[] GenerateFVector(double[] mesh)
        {
            MathExpression fFunc = new MathExpression(fFunction);
            double[] res = new double[mesh.Length-1];
            double h = 0.0;
            for (int i = 1; i < res.Length; i++)
            {
                h = Math.Abs(mesh[i] - mesh[i-1]);
                double hNext = Math.Abs(mesh[i+1] - mesh[i]);
                res[i-1] = (h / 2.0)*fFunc.Calculate(mesh[i] - (h / 2.0)) + (hNext / 2.0)*fFunc.Calculate(mesh[i] + (hNext / 2.0));
            }
            h = mesh[mesh.Length - 1] - mesh[mesh.Length - 2];
            res[res.Length - 1] = (h / 2.0) * (fFunc.Calculate(mesh[mesh.Length-1] - (h / 2.0)));
            return res;
        }

        private List<double[]> CalculateCoefficients(double[] mesh)
        {
            aMatrix = GenerateAMatrix(mesh);
            fVector = GenerateFVector(mesh);
            //GausMethod method=new GausMethod(fVector.Length,fVector.Length);
            //method.Matrix = aMatrix;
            //method.RightPart = fVector;
            //method.SolveMatrix();
            //var answ = method.Answer;
            List<double[]> coef=new List<double[]>();
            coef.Add(new double[]{-aMatrix[0][1]/aMatrix[0][0],fVector[0]/aMatrix[0][0]});
            for (int i = 1; i < fVector.Length-1; i++)
            {
                coef.Add(new double[] { -aMatrix[i][i + 1] / (aMatrix[i][i - 1] * coef.Last()[0] + aMatrix[i][i]), (fVector[i]-aMatrix[i][i-1]*coef.Last()[1]) / (aMatrix[i][i - 1] * coef.Last()[0] + aMatrix[i][i]) }); 
            }
            return coef;
        }
        public double[] Calculate(double[] mesh)
        {
            List<double[]> coef = CalculateCoefficients(mesh);
            int n = mesh.Length - 1;
            double[] c=new double[n];
            c[0] = (fVector[n - 1] - aMatrix[n - 1][n - 2]*coef.Last()[1])/
                   (aMatrix[n - 1][n - 1] + aMatrix[n - 1][n - 2]*coef.Last()[0]);
            for (int i = 1; i < n; i++)
            {
                c[i] = coef[n-i-1][0]*c[i - 1] + coef[n-i-1][1];
            }
            
            return c.Reverse().ToArray();
        }
    }
}
