namespace MSE_Calculator.Core
{
    public static class MatrixCalculation
    {
        public static double[] ProhoncaCalculation(double[][] A, double[] L)
        {
            double[] c = new double[L.Length - 1];
            double[] d = new double[L.Length];
            double[] x = new double[L.Length];

            c[0] = (A[0][0] != 0) ? A[0][1] / A[0][0] : 0;
            d[0] = (A[0][0] != 0) ? L[0] / A[0][0] : 0;

            for (int i = 1; i < L.Length; i++)
            {
                if (i != (L.Length - 1))
                {
                    c[i] = A[i][i + 1] / (A[i][i] - c[i - 1] * A[i][i - 1]);
                }

                d[i] = (L[i] - (d[i - 1] * A[i][i - 1])) / (A[i][i] - c[i - 1] * A[i][i - 1]);
            }

            x[L.Length - 1] = d[L.Length - 1];
            for (int i = L.Length - 2; i >= 0; i--)
            {
                x[i] = d[i] - (c[i] * x[i + 1]);
            }

            return x;
        }
    }
}
