using Adaptive_MSE.Core;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using PoohMathParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Adaptive_MSE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonCalculate_Click(object sender, RoutedEventArgs e)
        {
            var x = 2;

            var formula1 = new MathExpression("40");
            var formula2 = new MathExpression("40*x");
            var formula3 = new MathExpression("40 * x");
            var formula4 = new MathExpression("3*x^2");

            var res1 = formula1.Calculate(x);
            var res2 = formula2.Calculate(x);
            var res3 = formula3.Calculate(x);
            var res4 = formula4.Calculate(x);


            try
            {
                var N = int.Parse(textBoxN.Text);
                var miu = new MathExpression(fixMinus(textBoxMuFunction.Text));
                var beta = new MathExpression(fixMinus(textBoxBetaFunction.Text));
                var sigma = new MathExpression(fixMinus(textBoxSigmaFunction.Text));
                var f = new MathExpression(fixMinus(textBoxFFunction.Text));

                var calculator = new Calculator_MSE(miu, beta, sigma, f, -1.0, 1.0, 0.0, 0.0);

                var result = calculator.CalculateApproximation(N);

                var points = new List<Point>();
                for (int i = 0; i < result.XValues.Length; i++)
                {
                    points.Add(new Point(result.XValues[i], result.UValues[i]));
                }

                drawGraphic(points, "u(x)");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            buttonClear.IsEnabled = true;
        }
       
        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            clearGraphic();

            buttonClear.IsEnabled = false;
        }

        private string fixMinus(string text)
        {
            if (text[0] == '-')
            {
                return "0" + text.Replace("(-", "(0-");
            }
            return text.Replace("(-", "(0-");
        }

        private void clearGraphic()
        {
            var col = new Collection<IPlotterElement>();
            foreach (var ch in graphic.Children)
            {
                if (ch is LineGraph || ch is ElementMarkerPointsGraph || ch is MarkerPointsGraph)
                    col.Add(ch);
            }

            foreach (var x in col)
            {
                graphic.Children.Remove(x);
            }
        }

        private void drawGraphic(List<Point> pointsOfFunction, string description)
        {
            EnumerableDataSource<Point> enumerableDataSource = new EnumerableDataSource<Point>(pointsOfFunction);
            enumerableDataSource.SetXMapping(u => u.X);
            enumerableDataSource.SetYMapping(u => u.Y);
            IPointDataSource iPointDataSource = enumerableDataSource;
            graphic.AddLineGraph(iPointDataSource, new Pen(new SolidColorBrush(Colors.Brown), 1.5), new CirclePointMarker { Size = 8.0 }, new PenDescription(description));
            graphic.FitToView();
        }
    }
}
