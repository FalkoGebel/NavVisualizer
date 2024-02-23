using System.IO;
using System.Windows;
using VisualizerLibrary;

namespace VisualizerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VisualizerConnection? Connection;
        
        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        private void UpdateConnection()
        {
            Connection = new(NavServerTextBox.Text, NavDatabaseTextBox.Text, CompanyTextBox.Text, VisualizerServerTextBox.Text, VisualizerDatabaseTextBox.Text);
        }
        
        private void UpdateCostAmountChartButton_Click(object sender, RoutedEventArgs e)
        {
            CostAmountActualDvcChartSeries.ItemsSource = null;
            CostAmountExpectedDvcChartSeries.ItemsSource = null;
            CostAmountTotalDvcChartSeries.ItemsSource = null;

            UpdateConnection();
            
            try
            {
                Connection.SetDateFilter(StartDatePicker.SelectedDate, EndDatePicker.SelectedDate);
                CostAmountActualDvcChartSeries.ItemsSource = Connection.GetValueEntriesCalcSumsPerExistingDate();
                CostAmountExpectedDvcChartSeries.ItemsSource = CostAmountActualDvcChartSeries.ItemsSource;
                CostAmountTotalDvcChartSeries.ItemsSource = CostAmountActualDvcChartSeries.ItemsSource;
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