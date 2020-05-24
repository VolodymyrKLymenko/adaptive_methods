using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoohMathParser;

namespace Adaptive_MSE
{
    public class PresizeSolver
    {
        private string _presizeFunction;

        public PresizeSolver(string presizeFunction)
        {
            _presizeFunction = presizeFunction;
        }

        public List<double> Generate(double[] mesh)
        {
            MathExpression fFunc = new MathExpression(_presizeFunction);
            List<double> results=new List<double>();
            for (int i = 0; i < mesh.Length; i++)
            {
                results.Add(fFunc.Calculate(mesh[i]));
            }
            return results;
        }
    }
}
