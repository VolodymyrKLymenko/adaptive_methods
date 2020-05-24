using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaptive_MSE
{
    public class GausMethod
    {
       public int RowCount;
    public int ColumCount;
    public double[][] Matrix { get; set; }
    public double[] RightPart { get; set; }
    public double[] Answer { get; set; }
 
    public GausMethod(int row, int colum)
    {
      RightPart = new double[row];
      Answer = new double[row];
      Matrix = new double[row][];
      for (int i = 0; i < row; i++)
        Matrix[i] = new double[colum];
      RowCount = row;
      ColumCount = colum;
 
      //обнулим массив
      for (int i = 0; i < row; i++)
      {
        Answer[i] = 0;
        RightPart[i] = 0;
        for (int j = 0; j < colum; j++)
          Matrix[i][j] = 0;
      }
    }
 
    private void SortRows(int sortIndex)
    {
 
      double maxElement = Matrix[sortIndex][sortIndex];
      int maxElementIndex = sortIndex;
      for (int i = sortIndex + 1; i < RowCount; i++)
      {
        if (Matrix[i][sortIndex] > maxElement)
        {
          maxElement = Matrix[i][sortIndex];
          maxElementIndex = i;
        }
      }
 
      
      if (maxElementIndex > sortIndex)
      {
        double temp = RightPart[maxElementIndex];
        RightPart[maxElementIndex] = RightPart[sortIndex];
        RightPart[sortIndex] = temp;
 
        for (int i = 0; i < ColumCount; i++)
        {
          temp = Matrix[maxElementIndex][i];
          Matrix[maxElementIndex][i] = Matrix[sortIndex][i];
          Matrix[sortIndex][i] = temp;
        }
      }
    }
 
    public int SolveMatrix()
    {
      if (RowCount != ColumCount)
        return 1; //нет решения
 
      for (int i = 0; i < RowCount - 1; i++)
      {
        SortRows(i);
        for (int j = i + 1; j < RowCount; j++)
        {
          if (Matrix[i][i] != 0.0) //если главный элемент не 0, то производим вычисления
          {
            double multElement = Matrix[j][i] / Matrix[i][i];
            for (int k = i; k < ColumCount; k++)
              Matrix[j][k] -= Matrix[i][k] * multElement;
            RightPart[j] -= RightPart[i] * multElement;
          }
          
        }
      }
 
      
      for (int i = (int)(RowCount - 1); i >= 0; i--)
      {
        Answer[i] = RightPart[i];
 
        for (int j = (int)(RowCount - 1); j > i; j--)
          Answer[i] -= Matrix[i][j] * Answer[j];
 
        if (Matrix[i][i] == 0.0)
          if (RightPart[i] == 0.0)
            return 2; 
          else
            return 1; 
 
        Answer[i] /= Matrix[i][i];
 
      }
      return 0;
    }

    }
}
