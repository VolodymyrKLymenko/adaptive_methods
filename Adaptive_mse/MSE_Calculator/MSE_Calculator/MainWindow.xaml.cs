using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using PoohMathParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MSE_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Graphic window;
        private List<double> cResults;
        private List<double> points;
        private List<double> errors;
        private List<double> indicators;
        private Dictionary<int, Tuple<double[], double[]>> results;
        private static int CountOfRuns = 1;

        public MainWindow()
        {
            InitializeComponent();
            cResults = new List<double>();
            points = new List<double>();
            results = new Dictionary<int, Tuple<double[], double[]>>();
            errors = new List<double>();
            indicators = new List<double>();
            numberOfGraphic = 0;
        }

        private string FixMinus(string text)
        {
            if (text[0] == '-')
            {
                return "0" + text.Replace("(-", "(0-");
            }
            return text.Replace("(-", "(0-");
        }

        private void buttonCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string miu = FixMinus(textBoxMuFunction.Text);
                string beta = FixMinus(textBoxBetaFunction.Text);
                string sigma = FixMinus(textBoxSigmaFunction.Text);
                string f = FixMinus(textBoxFFunction.Text);

                var calculator = new Core.MSE_Calculator(
                                        new MathExpression(miu),
                                        new MathExpression(beta),
                                        new MathExpression(sigma),
                                        new MathExpression(f),
                                        -1.0, 1.0, 0.0, 0.0);

                var res = calculator.CalculateApproximation(100);
                var newPoints = new Point[100];
                for (int i = 0; i < 100; i++)
                {
                    newPoints[i] = new Point(res.Item1[i], res.Item2[i]);
                }

                EnumerableDataSource<Point> enumerableDataSource = new EnumerableDataSource<Point>(newPoints);
                enumerableDataSource.SetXMapping(u => u.X);
                enumerableDataSource.SetYMapping(u => u.Y);
                IPointDataSource iPointDataSource = enumerableDataSource;
                graphic.AddLineGraph(iPointDataSource, new Pen(new SolidColorBrush(Colors.Brown), 1.5), new CirclePointMarker { Size = 8.0 }, new PenDescription("test"));
                graphic.FitToView();

                results.Clear();
                errors.Clear();
                progressBarResult.Value = 0.0;

                string uPresize = FixMinus(textBoxUPresize.Text);

                cResults = res.Item2.ToList();
                indicators = res.Item1.ToList();
                results.Add(CountOfRuns++, new Tuple<double[], double[]>(res.Item1, res.Item2));
                points = res.Item1.ToList();
                progressBarResult.Value = 100.0;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            buttonGraphic.IsEnabled = true;
        }

        private void buttonGraphic_Click(object sender, RoutedEventArgs e)
        {
            progressBarResult.Value = 0.0;
            //window=new Graphic();
            Values = cResults;
            Points = points;
            Errors = errors;
            Indicators = indicators;
            CompleteResults = results;
            N = Convert.ToDouble(textBoxN.Text);
            listBoxIterations.Items.Clear();

            foreach (var key in CompleteResults)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = key.Key;
                this.listBoxIterations.Items.Add(key.Key);
            }
            listBoxIterations.SelectedIndex = 0;
            DrawTable();
            LoadSolutionGraphic();
            //window.Show();
        }

        public List<double> Values { get; set; }
        public List<double> PresizeValues { get; set; }
        public List<double> PresizePoints { get; set; }
        public List<double> Points { get; set; }
        public List<double> Errors { get; set; }
        public List<double> Indicators { get; set; }
        public Dictionary<int, Tuple<double[], double[]>> CompleteResults { get; set; }
        public double N { get; set; }
        private Dictionary<double, double> results2 = new Dictionary<double, double>();
        private DataTable table = new DataTable();
        private int numberOfGraphic;

        private void LoadDynamicsGraphic()
        {
            List<int> values = CompleteResults.Keys.ToList();
            List<Point> pointsOfFunction = new List<Point>();
            for (int i = 0; i < values.Count; i++)
            {
                pointsOfFunction.Add(new Point(i + 1, values[i]));
            }
            ClearGraphic();
            DrawGraphic(pointsOfFunction, "динаміка");
        }

        private void DrawGraphic(List<Point> pointsOfFunction, string description)
        {
            EnumerableDataSource<Point> enumerableDataSource = new EnumerableDataSource<Point>(pointsOfFunction);
            enumerableDataSource.SetXMapping(u => u.X);
            enumerableDataSource.SetYMapping(u => u.Y);
            IPointDataSource iPointDataSource = enumerableDataSource;
            graphic.AddLineGraph(iPointDataSource, new Pen(new SolidColorBrush(Colors.Brown), 1.5), new CirclePointMarker { Size = 8.0 }, new PenDescription(description));
            graphic.FitToView();
        }
        private void LoadErrorsGraphic()
        {
            List<int> values = CompleteResults.Keys.ToList();
            List<Point> pointsOfFunction = new List<Point>();
            for (int i = 0; i < values.Count; i++)
            {
                pointsOfFunction.Add(new Point(values[i], Errors[i]));
            }
            ClearGraphic();
            DrawGraphic(pointsOfFunction, "норма");
        }
        private void LoadIndicatorsGraphic()
        {

            List<double> values = CompleteResults[Convert.ToInt32(this.listBoxIterations.SelectedValue.ToString())].Item1.ToList();
            List<Point> pointsOfFunction = new List<Point>();
            for (int i = 0; i < values.Count - 1; i++)
            {
                pointsOfFunction.Add(new Point(i + 1, Indicators[i]));
            }
            ClearGraphic();
            DrawGraphic(pointsOfFunction, "розподіл");
        }

        private void UpdateResults()
        {
            results2.Clear();
            for (int i = 0; i < Values.Count; i++)
            {
                results2.Add(Points[i], Values[i]);
            }
        }
        private void DrawTable()
        {
            table.Columns.Clear();
            table.Rows.Clear();
            table.Columns.Add("x");
            table.Columns.Add("un(x)");
            for (int i = 0; i < results2.Count; i++)
            {
                table.Rows.Add(new TableRow());
                table.Rows[i][0] = string.Format("{0:0.000}", results2.ElementAt(i).Key);
                table.Rows[i][1] = string.Format("{0:0.00000}", results2.ElementAt(i).Value);
            }
            System.Console.WriteLine(results2);
            dataGridResults.ItemsSource = table.DefaultView;
        }
        private void LoadSolutionGraphic()
        {
            ClearGraphic();
            List<Point> pointsOfFunction = new List<Point>();

            foreach (var pair in results2)
            {
                pointsOfFunction.Add(new Point(pair.Key, pair.Value));
            }

            EnumerableDataSource<Point> enumerableDataSource = new EnumerableDataSource<Point>(pointsOfFunction);
            enumerableDataSource.SetXMapping(u => u.X);
            enumerableDataSource.SetYMapping(u => u.Y);

            IPointDataSource iPointDataSource = enumerableDataSource;
            graphic.AddLineGraph(iPointDataSource, new Pen(new SolidColorBrush(Colors.Pink), 3.0), new CirclePointMarker { Size = 8.0 }, new PenDescription("un(x)"));

            graphic.FitToView();
        }
        private void graphic_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var key in CompleteResults)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = key.Key;
                this.listBoxIterations.Items.Add(key.Key);
            }
            this.listBoxIterations.SelectedIndex = 0;
            DrawTable();
            LoadSolutionGraphic();
        }
        private void ClearGraphic()
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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listBoxIterations != null && this.listBoxIterations.SelectedIndex != -1)
            {
                if (numberOfGraphic == 2)
                {
                    string selecteValue = this.listBoxIterations.SelectedValue.ToString();
                    Points = CompleteResults[Convert.ToInt32(selecteValue)].Item1.ToList();
                    Values = CompleteResults[Convert.ToInt32(selecteValue)].Item2.ToList();
                    UpdateResults();
                    DrawTable();
                    LoadIndicatorsGraphic();
                }
                else
                {
                    string selecteValue = this.listBoxIterations.SelectedValue.ToString();
                    Points = CompleteResults[Convert.ToInt32(selecteValue)].Item1.ToList();
                    Values = CompleteResults[Convert.ToInt32(selecteValue)].Item2.ToList();
                    UpdateResults();
                    DrawTable();
                    LoadSolutionGraphic();
                }
            }
        }

        private void buttonPrevGraphic_Click(object sender, RoutedEventArgs e)
        {
            numberOfGraphic--;
            buttonNextGraphic.IsEnabled = true;
            if (numberOfGraphic == 0)
            {
                buttonPrevGraphic.IsEnabled = false;
                listBoxIterations.Visibility = Visibility.Visible;
                labelIteration.Visibility = Visibility.Visible;
            }
            else if (numberOfGraphic == 2)
            {
                buttonPrevGraphic.IsEnabled = true;
                listBoxIterations.Visibility = Visibility.Visible;
                labelIteration.Visibility = Visibility.Visible;
            }
            else
            {
                buttonPrevGraphic.IsEnabled = true;
                listBoxIterations.Visibility = Visibility.Hidden;
                labelIteration.Visibility = Visibility.Hidden;
            }
            SwitchGraphic(numberOfGraphic);
        }

        private void buttonNextGraphic_Click(object sender, RoutedEventArgs e)
        {
            numberOfGraphic++;
            listBoxIterations.Visibility = Visibility.Hidden;
            labelIteration.Visibility = Visibility.Hidden;
            buttonPrevGraphic.IsEnabled = true;
            if (numberOfGraphic == 2)
            {
                buttonNextGraphic.IsEnabled = false;
                listBoxIterations.Visibility = Visibility.Visible;
                labelIteration.Visibility = Visibility.Visible;
            }
            else
            {
                buttonNextGraphic.IsEnabled = true;
            }
            SwitchGraphic(numberOfGraphic);
        }

        private void SwitchGraphic(int number)
        {
            switch (number)
            {
                case 0:
                    labelGraphic.Content = "Графіки наближеного і точного розв'язків задачі";
                    LoadSolutionGraphic();
                    break;
                case 1:
                    labelGraphic.Content = "Норма апостеріорного оцінювача похибки на кожному розбитті";
                    LoadErrorsGraphic();
                    break;
                case 2:
                    labelGraphic.Content = "Розподіл значень індикаторів якості підсумкової апроксимації";
                    LoadIndicatorsGraphic();
                    break;
                default:
                    labelGraphic.Content = "Графіки наближеного і точного розв'язків задачі";
                    LoadSolutionGraphic();
                    break;
            }
        }

    }
}
