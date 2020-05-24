using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace MSE_Calculator
{
    /// <summary>
    /// Interaction logic for Graphic.xaml
    /// </summary>
    public partial class Graphic : Window
    {
        public List<double> Values { get; set; }
        public List<double> PresizeValues { get; set; }
        public List<double> Points { get; set; }
        public List<double> Errors { get; set; }
        public List<double> Indicators { get; set; } 
        public Dictionary<int, Tuple<double[], double[]>> CompleteResults { get; set; }
        public Dictionary<int, Tuple<double[], double[]>> CompletePresizeResults { get; set; } 
        public double N { get; set; }
        private Dictionary<double, double> results=new Dictionary<double, double>();
        private Dictionary<double, double> presizeResults = new Dictionary<double, double>();
        private DataTable table=new DataTable();
        private int numberOfGraphic;
        public Graphic()
        {
            numberOfGraphic = 0;
            InitializeComponent();
        }

        private void LoadDynamicsGraphic()
        {
            List<int> values = CompleteResults.Keys.ToList();
            List<Point> pointsOfFunction=new List<Point>();
            for (int i = 0; i < values.Count; i++)
            {
                pointsOfFunction.Add(new Point(i+1,values[i]));
            }
            ClearGraphic();
            DrawGraphic(pointsOfFunction,"динаміка");
        }

        private void DrawGraphic(List<Point> pointsOfFunction, string description)
        {
            EnumerableDataSource<Point> enumerableDataSource = new EnumerableDataSource<Point>(pointsOfFunction);
            enumerableDataSource.SetXMapping(u => u.X);
            enumerableDataSource.SetYMapping(u => u.Y);
            IPointDataSource iPointDataSource = enumerableDataSource;
            graphic.AddLineGraph(iPointDataSource, new Pen(new SolidColorBrush(Colors.Black), 2.0), new TrianglePointMarker { Size = 10.0 }, new PenDescription(description));
            graphic.FitToView();
        }
        private void LoadErrorsGraphic()
        {
            List<int> values = CompleteResults.Keys.ToList();
            List<Point> pointsOfFunction = new List<Point>();
            for (int i = 0; i < values.Count; i++)
            {
                pointsOfFunction.Add(new Point(values[i],Errors[i]));
            }
            ClearGraphic();
            DrawGraphic(pointsOfFunction,"норма");
        }
        private void LoadIndicatorsGraphic()
        {
            
            List<double> values = CompleteResults.LastOrDefault().Value.Item1.ToList();
            List<Point> pointsOfFunction = new List<Point>();
            for (int i = 0; i < values.Count-1; i++)
            {
                pointsOfFunction.Add(new Point(i+1, Indicators[i]));
            }
            ClearGraphic();
            DrawGraphic(pointsOfFunction,"розподіл");
        }

        private void UpdateResults()
        {
            results.Clear();
            presizeResults.Clear();
            for (int i = 0; i < Values.Count; i++)
            {
                results.Add(Points[i], Values[i]);
                presizeResults.Add(Points[i], PresizeValues[i]);
            }
        }
        private void DrawTable()
        {
            table.Columns.Clear();
            table.Rows.Clear();
            table.Columns.Add("x");
            table.Columns.Add("un(x)");
            table.Columns.Add("u(x)");
            for (int i = 0; i < results.Count; i++)
            {
                table.Rows.Add(new TableRow());
                table.Rows[i][0] = string.Format("{0:0.000}", results.ElementAt(i).Key);
                table.Rows[i][1] = string.Format("{0:0.00000}", results.ElementAt(i).Value);
                table.Rows[i][2] = string.Format("{0:0.00000}", presizeResults.ElementAt(i).Value);
            }
            dataGridResults.ItemsSource = table.DefaultView;
        }
        private void LoadSolutionGraphic()
        {
            ClearGraphic();
            List<Point> pointsOfFunction = new List<Point>();
            List<Point> pointsOfPresizeFunction = new List<Point>();
            foreach (var pair in results)
            {
                pointsOfFunction.Add(new Point(pair.Key, pair.Value));
            }
            foreach (var pair in presizeResults)
            {
                pointsOfPresizeFunction.Add(new Point(pair.Key, pair.Value));
            }

            

            EnumerableDataSource<Point> enumerableDataSource = new EnumerableDataSource<Point>(pointsOfFunction);
            enumerableDataSource.SetXMapping(u => u.X);
            enumerableDataSource.SetYMapping(u => u.Y);
            IPointDataSource iPointDataSource = enumerableDataSource;
            graphic.AddLineGraph(iPointDataSource, new Pen(new SolidColorBrush(Colors.Black), 2.0), new CirclePointMarker { Size = 10.0 }, new PenDescription("un(x)"));
            EnumerableDataSource<Point> enumerableDataSource1 = new EnumerableDataSource<Point>(pointsOfPresizeFunction);
            enumerableDataSource1.SetXMapping(u => u.X);
            enumerableDataSource1.SetYMapping(u => u.Y);
            IPointDataSource iPointDataSource1 = enumerableDataSource1;
            
            graphic.AddLineGraph(iPointDataSource1, new Pen(new SolidColorBrush(Colors.Red), 2.0), new CirclePointMarker { Size = 10.0 }, new PenDescription("u(x)"));
            graphic.FitToView();
        }
        private void graphic_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var key in CompleteResults)
            {
                ListBoxItem item=new ListBoxItem();
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
                string selecteValue =this.listBoxIterations.SelectedValue.ToString();
                Points = CompleteResults[Convert.ToInt32(selecteValue)].Item1.ToList();
                Values = CompleteResults[Convert.ToInt32(selecteValue)].Item2.ToList();
                PresizeValues = CompletePresizeResults[Convert.ToInt32(selecteValue)].Item2.ToList();
                UpdateResults();
                DrawTable();
                LoadSolutionGraphic();
            }
        }

        private void buttonPrevGraphic_Click(object sender, RoutedEventArgs e)
        {
            numberOfGraphic--;
            buttonNextGraphic.IsEnabled = true;
            if (numberOfGraphic == 0)
            {
                buttonPrevGraphic.IsEnabled = false;
                listBoxIterations.Visibility=Visibility.Visible;
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
            labelIteration.Visibility=Visibility.Hidden;
            buttonPrevGraphic.IsEnabled = true;
            if (numberOfGraphic == 3)
            {
                buttonNextGraphic.IsEnabled = false;
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
                    labelGraphic.Content = "Динаміка зростання кількості елементів сіток";
                    LoadDynamicsGraphic();
                    break;
                case 2:
                    labelGraphic.Content = "Норма апостеріорного оцінювача похибки на кожному розбитті";
                    LoadErrorsGraphic();
                    break;
                case 3:
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
