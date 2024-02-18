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
        private VisualizerConnection? Connection;
        
        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
        }

        private void UpdateConnection()
        {
            Connection = new(ServerTextBox.Text, DatabaseTextBox.Text, CompanyTextBox.Text);
        }
        
        private void UpdateCostAmountChartButton_Click(object sender, RoutedEventArgs e)
        {
            CostAmountDvcChartAcutalSeries.ItemsSource = null;

            List<ValueEntryModel> values = [];

            UpdateConnection();
            
            try
            {
                values = Connection.GetValueEntriesCalcSumsPerExistingDate();
                CostAmountDvcChartAcutalSeries.ItemsSource = values;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return;
            }
        }
    }
}