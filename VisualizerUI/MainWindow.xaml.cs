using System.IO;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Media;
using VisualizerLibrary;
using VisualizerLibrary.Models;

namespace VisualizerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VisualizerConnection Connection = new("", "", "", "", "");
        private readonly bool ReadyToChart = false;

        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));

            System.Globalization.CultureInfo culture = new("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // TODO - for testing purposes only ... start
            string[] connectionFileData = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NavVisualizerTestConnectionData.txt");

            NavServerTextBox.Text = connectionFileData[0];
            NavDatabaseTextBox.Text = connectionFileData[1];
            CompanyTextBox.Text = connectionFileData[2];
            VisualizerServerTextBox.Text = connectionFileData[3];
            VisualizerDatabaseTextBox.Text = connectionFileData[4];
            // for testing purposes only ... end

            InitializeComboBoxItems();

            ReadyToChart = true;
            
            UpdateChart();
        }

        private void UpdateConnection()
        {
            Connection = new(NavServerTextBox.Text, NavDatabaseTextBox.Text, CompanyTextBox.Text, VisualizerServerTextBox.Text, VisualizerDatabaseTextBox.Text);
        }

        private void AddCostAmountSeriesToChart()
        {
            CostAmountDvcChart.Title = Properties.Resources.CHART_COST_AMT_TITLE;

            List<ValuesPerDateModel> values;

            try
            {
                values = Connection.GetValuesPerDateForDates();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return;
            }

            List<KeyValuePair<DateTime, decimal>> keyValuePairsCostAmountActual = [];
            List<KeyValuePair<DateTime, decimal>> keyValuePairsCostAmountExpected = [];
            List<KeyValuePair<DateTime, decimal>> keyValuePairsCostAmountTotal = [];
            
            foreach (var value in values)
            {
                keyValuePairsCostAmountActual.Add(new KeyValuePair<DateTime, decimal>(value.Date, value.CostAmountActual));
                keyValuePairsCostAmountExpected.Add(new KeyValuePair<DateTime, decimal>(value.Date, value.CostAmountExpected));
                keyValuePairsCostAmountTotal.Add(new KeyValuePair<DateTime, decimal>(value.Date, value.CostAmountActual + value.CostAmountExpected));
            }

            for (int i = 0; i < 3; i++)
            {
                Style style = new();

                switch (i)
                {
                    case 0: style.Setters.Add(new Setter(BackgroundProperty, Brushes.Green)); break;
                    case 1: style.Setters.Add(new Setter(BackgroundProperty, Brushes.Red)); break;
                    default: style.Setters.Add(new Setter(BackgroundProperty, Brushes.Black)); break;
                }

                if (ChartTypeComboBox.Items.GetItemAt(ChartTypeComboBox.SelectedIndex).ToString() == Properties.Resources.CB_CHART_TYPES_LINE)
                {
                    style.TargetType = typeof(LineDataPoint);
                    style.Setters.Add(new Setter(TemplateProperty, null));

                    LineSeries ls = new()
                    {
                        IndependentValueBinding = new Binding("Key"),
                        DependentValueBinding = new Binding("Value"),
                        DataPointStyle = style
                    };

                    switch (i)
                    {
                        case 0:
                            ls.Title = Properties.Resources.CHART_COST_AMT_ACT_LEGEND;
                            ls.ItemsSource = keyValuePairsCostAmountActual;
                            break;
                        case 1:
                            ls.Title = Properties.Resources.CHART_COST_AMT_EXP_LEGEND;
                            ls.ItemsSource = keyValuePairsCostAmountExpected;
                            break;
                        default:
                            ls.Title = Properties.Resources.CHART_COST_AMT_TOT_LEGEND;
                            ls.ItemsSource = keyValuePairsCostAmountTotal;
                            break;
                    }
                    CostAmountDvcChart.Series.Add(ls);
                }
                else if (ChartTypeComboBox.Items.GetItemAt(ChartTypeComboBox.SelectedIndex).ToString() == Properties.Resources.CB_CHART_TYPES_COLUMN)
                {
                    style.TargetType = typeof(ColumnDataPoint);

                    ColumnSeries cs = new()
                    {
                        IndependentValueBinding = new Binding("Key"),
                        DependentValueBinding = new Binding("Value"),
                        DataPointStyle = style
                    };

                    switch (i)
                    {
                        case 0:
                            cs.Title = Properties.Resources.CHART_COST_AMT_ACT_LEGEND;
                            cs.ItemsSource = keyValuePairsCostAmountActual;
                            break;
                        case 1:
                            cs.Title = Properties.Resources.CHART_COST_AMT_EXP_LEGEND;
                            cs.ItemsSource = keyValuePairsCostAmountExpected;
                            break;
                        default:
                            cs.Title = Properties.Resources.CHART_COST_AMT_TOT_LEGEND;
                            cs.ItemsSource = keyValuePairsCostAmountTotal;
                            break;
                    }
                    
                    CostAmountDvcChart.Series.Add(cs);
                }
                else
                    MessageBox.Show("Not Implemented");
            }
        }

        private void InitializeComboBoxItems()
        {
            KeyFigureComboBox.Items.Add(Properties.Resources.CB_KEY_FIGURE_COST_AMOUNT);
            KeyFigureComboBox.SelectedIndex = 0;

            ChartTypeComboBox.Items.Add(Properties.Resources.CB_CHART_TYPES_LINE);
            ChartTypeComboBox.Items.Add(Properties.Resources.CB_CHART_TYPES_COLUMN);
            ChartTypeComboBox.SelectedIndex = 0;
        }

        private void UpdateChart()
        {
            if (!ReadyToChart)
                return;
            
            CostAmountDvcChart.Series.Clear();

            UpdateConnection();
            Connection.SetDateFilter(StartDatePicker.SelectedDate, EndDatePicker.SelectedDate);

            if (KeyFigureComboBox.Items.GetItemAt(KeyFigureComboBox.SelectedIndex).ToString() == Properties.Resources.CB_KEY_FIGURE_COST_AMOUNT)
                AddCostAmountSeriesToChart();
            else
                MessageBox.Show("Not Implemented");
        }

        private void UpdateChartButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateChart();
        }

        private void UpdateCostAmountChartButton_Click_1(object sender, RoutedEventArgs e)
        {
            UpdateConnection();
            Connection.UpdateVisualizerDatabaseFromNavDatabase();
            MessageBox.Show("Update gelaufen");
            UpdateChart();
        }

        private void KeyFigureComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void ChartTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void StartDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void EndDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateChart();
        }
    }
}