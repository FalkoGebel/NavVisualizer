using Microsoft.Data.SqlClient;

namespace VisualizerLibrary
{
    public static class VisualizerLogic
    {
        public static void ConnectToNavDatabase(string server, string database)
        {
            if (server == string.Empty)
                throw new ArgumentException(Properties.Resources.EXP_SERVER_MISSING);

            if (database == string.Empty)
                throw new ArgumentException(Properties.Resources.EXP_DATABASE_MISSING);

            string connectionString = $@"Data Source={server};Initial Catalog={database};Integrated Security=SSPI;";

            SqlConnection cnn = new(connectionString);
            cnn.Open();
            cnn.Close();
        }
    }
}
