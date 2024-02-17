using Microsoft.Data.SqlClient;
using System.Windows;
using VisualizerLibrary;

namespace VisualizerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection cnn;
            
            try
            {
                cnn = VisualizerLogic.GetOpenConnectionToNavDatabase(ServerTextBox.Text, DatabaseTextBox.Text);
            }
            catch (Exception exp)
            {
                MessageBox.Show("Connection error: " + exp.Message);
                return;
            }

            MessageBox.Show("Connection was established");
            cnn.Close();
        }
    }
}