using System.IO;
using System.Windows;
using System.Windows.Controls;
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
        private VisualizerConnection _connection = new("", "", "", "", "");
        private bool _readyToChart = false;

        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));

            System.Globalization.CultureInfo culture = new("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            InitializeComboBoxItems();
        }

        private void UpdateConnection()
        {
            _connection = new(NavServerTextBox.Text, NavDatabaseTextBox.Text, CompanyTextBox.Text, VisualizerServerTextBox.Text, VisualizerDatabaseTextBox.Text);
        }

        private void AddCostAmountSeriesToChart()
        {
            CostAmountDvcChart.Title = Properties.Resources.CHART_COST_AMT_TITLE;

            List<ValuesPerDateCummulatedModel> values = GetValuesPerDateCummulatedByCurrentPeriodType();

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
                    throw new InvalidDataException(Properties.Resources.ERROR_INVALID_OPTION);
            }
        }

        private List<ValuesPerDateCummulatedModel> GetValuesPerDateCummulatedByCurrentPeriodType()
        {
            try
            {
                if (PeriodTypeComboBox.Items.GetItemAt(PeriodTypeComboBox.SelectedIndex).ToString() == Properties.Resources.CB_PERIOD_TYPE_DAY)
                    return _connection.GetValuesPerDateCummulatedForDates();
                else if (PeriodTypeComboBox.Items.GetItemAt(PeriodTypeComboBox.SelectedIndex).ToString() == Properties.Resources.CB_PERIOD_TYPE_WEEK)
                    return _connection.GetValuesPerDateCummulatedForWeeks();
                else if (PeriodTypeComboBox.Items.GetItemAt(PeriodTypeComboBox.SelectedIndex).ToString() == Properties.Resources.CB_PERIOD_TYPE_MONTH)
                    return _connection.GetValuesPerDateCummulatedForMonths();
                else if (PeriodTypeComboBox.Items.GetItemAt(PeriodTypeComboBox.SelectedIndex).ToString() == Properties.Resources.CB_PERIOD_TYPE_QUARTER)
                    return _connection.GetValuesPerDateCummulatedForQuarters();
                else
                    return _connection.GetValuesPerDateCummulatedForYears();
            }
            catch
            {
                ShowError(Properties.Resources.ERROR_UPDATE_CHART);
                return [];
            }
        }

        private static void ShowError(string msg)
        {
            MessageBox.Show(msg, Properties.Resources.ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private List<ValuesPerDatePlainModel> GetValuesPerDatePlainByCurrentPeriodType()
        {
            try
            {
                if (PeriodTypeComboBox.Items.GetItemAt(PeriodTypeComboBox.SelectedIndex).ToString() == Properties.Resources.CB_PERIOD_TYPE_DAY)
                    return _connection.GetValuesPerDatePlainForDates();
                else if (PeriodTypeComboBox.Items.GetItemAt(PeriodTypeComboBox.SelectedIndex).ToString() == Properties.Resources.CB_PERIOD_TYPE_WEEK)
                    return _connection.GetValuesPerDatePlainForWeeks();
                else if (PeriodTypeComboBox.Items.GetItemAt(PeriodTypeComboBox.SelectedIndex).ToString() == Properties.Resources.CB_PERIOD_TYPE_MONTH)
                    return _connection.GetValuesPerDatePlainForMonths();
                else if (PeriodTypeComboBox.Items.GetItemAt(PeriodTypeComboBox.SelectedIndex).ToString() == Properties.Resources.CB_PERIOD_TYPE_QUARTER)
                    return _connection.GetValuesPerDatePlainForQuarters();
                else
                    return _connection.GetValuesPerDatePlainForYears();
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message);
            }
        }

        private void AddSalesAmountSeriesToChart()
        {
            CostAmountDvcChart.Title = Properties.Resources.CHART_SALES_AMT_TITLE;

            List<ValuesPerDatePlainModel> values = GetValuesPerDatePlainByCurrentPeriodType();

            if (values.Count == 0)
                return;

            List<KeyValuePair<string, decimal>> keyValuePairsSalesAmountActual = [];
            List<KeyValuePair<DateTime, decimal>> keyValuePairsSalesAmountActualDate = [];

            if (values[0].SingleDate)
            {
                foreach (var value in values)
                    keyValuePairsSalesAmountActualDate.Add(new KeyValuePair<DateTime, decimal>(value.Date, value.SalesAmountActual));
            }
            else
            {
                for (int i = 0; i < values.Count; i++)
                {
                    var value = values[i];

                    if (i == 0)
                        keyValuePairsSalesAmountActual.Add(new KeyValuePair<string, decimal>(value.LongChartLabel, value.SalesAmountActual));
                    else
                        keyValuePairsSalesAmountActual.Add(new KeyValuePair<string, decimal>(value.ChartLabel, value.SalesAmountActual));
                }
            }

            Style style = new();
            style.Setters.Add(new Setter(BackgroundProperty, Brushes.DarkBlue));

            if (ChartTypeComboBox.Items.GetItemAt(ChartTypeComboBox.SelectedIndex).ToString() == Properties.Resources.CB_CHART_TYPES_COLUMN)
            {
                style.TargetType = typeof(ColumnDataPoint);

                ColumnSeries cs = new()
                {
                    IndependentValueBinding = new Binding("Key"),
                    DependentValueBinding = new Binding("Value"),
                    DataPointStyle = style,
                    ItemsSource = values[0].SingleDate ? keyValuePairsSalesAmountActualDate : keyValuePairsSalesAmountActual,
                    Title = Properties.Resources.CHART_SALES_AMT_ACT_LEGEND
                };

                CostAmountDvcChart.Series.Add(cs);
            }
            else
                throw new InvalidDataException(Properties.Resources.ERROR_INVALID_OPTION);
        }

        private void InitializeComboBoxItems()
        {
            PeriodTypeComboBox.Items.Add(Properties.Resources.CB_PERIOD_TYPE_DAY);
            PeriodTypeComboBox.Items.Add(Properties.Resources.CB_PERIOD_TYPE_WEEK);
            PeriodTypeComboBox.Items.Add(Properties.Resources.CB_PERIOD_TYPE_MONTH);
            PeriodTypeComboBox.Items.Add(Properties.Resources.CB_PERIOD_TYPE_QUARTER);
            PeriodTypeComboBox.Items.Add(Properties.Resources.CB_PERIOD_TYPE_YEAR);
            PeriodTypeComboBox.SelectedIndex = 0;

            KeyFigureComboBox.Items.Add(Properties.Resources.CB_KEY_FIGURE_COST_AMOUNT);
            KeyFigureComboBox.Items.Add(Properties.Resources.CB_KEY_FIGURE_SALES_AMOUNT);
            KeyFigureComboBox.SelectedIndex = 0;

            UpdateChartTypeComboBoxItems(Properties.Resources.CB_KEY_FIGURE_COST_AMOUNT);
        }

        private void UpdateChart()
        {
            if (!_readyToChart)
                return;
            
            CostAmountDvcChart.Series.Clear();

            UpdateConnection();
            _connection.SetDateFilter(StartDatePicker.SelectedDate, EndDatePicker.SelectedDate);

            if (KeyFigureComboBox.Items.GetItemAt(KeyFigureComboBox.SelectedIndex).ToString() == Properties.Resources.CB_KEY_FIGURE_COST_AMOUNT)
                AddCostAmountSeriesToChart();
            else if (KeyFigureComboBox.Items.GetItemAt(KeyFigureComboBox.SelectedIndex).ToString() == Properties.Resources.CB_KEY_FIGURE_SALES_AMOUNT)
                AddSalesAmountSeriesToChart();
            else
                throw new InvalidDataException(Properties.Resources.ERROR_INVALID_OPTION);
        }

        private void UpdateDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateVisualizerDatabaseFromNAVDatabase();
        }

        private void UpdateVisualizerDatabaseFromNAVDatabase()
        {
            UpdateConnection();
            if (_connection.NavServer == "")
            {
                ShowError(Properties.Resources.ERROR_NAV_SERVER_MISSING);
                return;
            }
            if (_connection.NavDatabase == "")
            {
                ShowError(Properties.Resources.ERROR_NAV_DATABASE_MISSING);
                return;
            }
            if (_connection.Company == "")
            {
                ShowError(Properties.Resources.ERROR_COMPANY_MISSING);
                return;
            }
            if (_connection.VisualizerServer == "")
            {
                ShowError(Properties.Resources.ERROR_VISUALIZER_SERVER_MISSING);
                return;
            }
            if (_connection.VisualizerDatabase == "")
            {
                ShowError(Properties.Resources.ERROR_VISUALIZER_DATABASE_MISSING);
                return;
            }
            try
            {
                _connection.UpdateVisualizerDatabaseFromNavDatabase();
            }
            catch (Exception exp)
            {
                ShowError(exp.Message);
                return;
            }
            ShowMessage(Properties.Resources.MSG_UPDATE_DONE);
            UpdateChart();
        }

        private static void ShowMessage(string msg)
        {
            MessageBox.Show(msg, Properties.Resources.MSG_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateChartTypeComboBoxItems(string keyFigure)
        {
            if (keyFigure == "")
                return;
            
            ChartTypeComboBox.Items.Clear();
            if (keyFigure == Properties.Resources.CB_KEY_FIGURE_COST_AMOUNT)
            {
                ChartTypeComboBox.Items.Add(Properties.Resources.CB_CHART_TYPES_LINE);
            }
            else if (keyFigure == Properties.Resources.CB_KEY_FIGURE_SALES_AMOUNT)
            {
                ChartTypeComboBox.Items.Add(Properties.Resources.CB_CHART_TYPES_COLUMN);
            }
            if (ChartTypeComboBox.Items.Count > 0)
                ChartTypeComboBox.SelectedIndex = 0;
        }

        private void KeyFigureComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _readyToChart = false;
            UpdateChartTypeComboBoxItems(e.AddedItems[0]?.ToString() ?? "");
            _readyToChart = true;
            if (((ComboBox)sender).IsLoaded)
                UpdateChart();
        }

        private void ChartTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).IsLoaded)
                UpdateChart();
        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void PeriodTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChart();
        }
    }
}