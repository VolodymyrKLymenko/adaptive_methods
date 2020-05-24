using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoohMathParser;

namespace Adaptive_MSE
{
    public class MeshGenerator
    {
        private int _n;
        private double _a;
        private double _b;
        private double _nu;
        private string muFunction;
        private string sigmaFunction;
        private string fFunction;
        private string betaFunction;
        private List<double> indicators;

        public List<double> Indicators
        {
            get
            {
                return indicators;
            }
        } 
        public MeshGenerator(int n, double a, double b, double nu,string mu, string beta, string sigma, string f)
        {
            _n = n;
            _a = a;
            _b = b;
            _nu = nu;
            muFunction = mu;
            betaFunction = beta;
            sigmaFunction = sigma;
            fFunction = f;
            indicators=new List<double>();
        }

        public List<double> GenerateFirstMesh()
        {
            List<double> points = new List<double> {};
            double h = Math.Abs(_b - _a) / _n;
            double x = _a;
            while (x < _b+h/2.0)
            {
                points.Add(x);
                x += h;
            }
            return points;
        } 

        public List<double> Generate(double[] mesh, double[] qCoeff)
        {
            List<double> points=new List<double>{mesh[0]};
            indicators.Clear();
            for (int i = 1; i < mesh.Length; i++)
            {
                double h = mesh[i] - mesh[i - 1];
                double indicator = GetIndicator(mesh[i-1],mesh[i],qCoeff[i-1],qCoeff[i],mesh,qCoeff)*100;
                indicators.Add(indicator);
                if (indicator >= _nu)
                {
                    points.Add(mesh[i] - h/2.0);
                }
                points.Add(mesh[i]);
            }
            return points;
        }

        private double GetIndicator(double x1, double x2, double q1, double q2,double[] mesh, double[] qs)
        {
            double n = mesh.Length;
            double sum = 0.0;
            for (int i = 0; i < n-1; i++)
            {
                double uNormSquared = GetUNormSquared(qs[i], qs[i+1], mesh[i + 1] - mesh[i]);

                //todo find out formula
                double eNormSquared = GetENormSquared(qs[i],qs[i+1],mesh[i+1]-mesh[i],mesh[i]);

                sum += uNormSquared + eNormSquared;
            }
            //todo find out formula
            double eNormCurr = GetENormSquared(q1,q2,x2-x1,x1);
            double sqrt = Math.Sqrt(1.0/n*sum);

            return eNormCurr/sqrt;
        }

        private double GetUNormSquared(double q1, double q2, double h)
        {
            //return (q2 - q1) / h;
            return Math.Pow((q2 - q1),2.0)/h;
        }

        private double GetENormSquared(double q1, double q2, double h, double x1)
        {
            double midPoint = x1 + h/2.0;
            MathExpression muFunc = new MathExpression(muFunction);
            MathExpression betaFunc = new MathExpression(betaFunction);
            MathExpression sigmaFunc = new MathExpression(sigmaFunction);
            MathExpression fFunc = new MathExpression(fFunction);
            double qPrime = (q2 - q1) / h;
            double qMiddle = (q2 + q1)/2.0;
            double Pe = h * betaFunc.Calculate(midPoint) / muFunc.Calculate(midPoint);
            double sh = h * h * sigmaFunc.Calculate(midPoint) / muFunc.Calculate(midPoint);
            double result =(5.0/6.0)* Math.Pow(h,3.0)*Math.Pow(fFunc.Calculate(midPoint)-qPrime*betaFunc.Calculate(midPoint)-sigmaFunc.Calculate(midPoint)*qMiddle,2.0)
                /(muFunc.Calculate(midPoint)*(10+Pe*sh));
            return result;
        }

        public double GenerateNorm(double[] mesh, double[] qCoeff)
        {
            double sum = 0.0;
            for (int i = 0; i < mesh.Length-1; i++)
            {
                sum += GetValueOnElement(mesh[i], mesh[i + 1], qCoeff[i], qCoeff[i + 1]);
            }
            return Math.Sqrt((5.0/6.0)*sum);
        }

        private double GetValueOnElement(double x1, double x2, double q1, double q2)
        {
            double h = Math.Abs(x1 - x2)/2.0;
            double midPoint = x1 + h/2.0;
            MathExpression muFunc = new MathExpression(muFunction);
            MathExpression betaFunc = new MathExpression(betaFunction);
            MathExpression sigmaFunc = new MathExpression(sigmaFunction);
            MathExpression fFunc = new MathExpression(fFunction);
            double mu = muFunc.Calculate(midPoint);
            double beta = betaFunc.Calculate(midPoint);
            double sigma = sigmaFunc.Calculate(midPoint);
            double f = fFunc.Calculate(midPoint);
            double pe = h*beta/mu;
            double sh = h*h*sigma/mu;
            double qPrime = Math.Abs(q2 - q1)/h;
            double qVal = (q2 + q1)/2.0;
            return (Math.Pow(h, 3.0)*Math.Pow((fFunc.Calculate(midPoint) - beta*qPrime - sigma*qVal), 2.0))/(mu*(10 + pe*sh));
        }
    }
}
