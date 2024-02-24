using System.IO;
using System.Windows;
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

        }

        private void UpdateConnection()
        {
            Connection = new(NavServerTextBox.Text, NavDatabaseTextBox.Text, CompanyTextBox.Text, VisualizerServerTextBox.Text, VisualizerDatabaseTextBox.Text);
        }

        private void UpdateCostAmountChartButton_Click(object sender, RoutedEventArgs e)
        {
            CostAmountActualDvcChartSeries.ItemsSource = null;
            CostAmountExpectedDvcChartSeries.ItemsSource = null;

            UpdateConnection();

            try
            {
                Connection.SetDateFilter(StartDatePicker.SelectedDate, EndDatePicker.SelectedDate);
                List<ValuesPerDateModel> values = Connection.GetValuesPerDateForDates();
                List<KeyValuePair<DateTime, decimal>> keyValuePairsCostAmountActual = [];
                List<KeyValuePair<DateTime, decimal>> keyValuePairsCostAmountExpected = [];

                foreach (var value in values)
                {
                    keyValuePairsCostAmountActual.Add(new KeyValuePair<DateTime, decimal>(value.Date, value.CostAmountActual));
                    keyValuePairsCostAmountExpected.Add(new KeyValuePair<DateTime, decimal>(value.Date, value.CostAmountExpected));
                }

                CostAmountActualDvcChartSeries.ItemsSource = keyValuePairsCostAmountActual;
                CostAmountExpectedDvcChartSeries.ItemsSource = keyValuePairsCostAmountExpected;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void UpdateCostAmountChartButton_Click_1(object sender, RoutedEventArgs e)
        {
            UpdateConnection();
            Connection.UpdateVisualizerDatabaseFromNavDatabase();
            MessageBox.Show("Update gelaufen");
        }
    }
}