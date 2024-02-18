using Dapper;
using Microsoft.Data.SqlClient;
using VisualizerLibrary.Models;

namespace VisualizerLibrary
{
    public static class VisualizerLogic
    {
        public static SqlConnection GetOpenConnectionToNavDatabase(string server, string database)
        {
            if (server == string.Empty)
                throw new ArgumentException(Properties.Resources.EXP_SERVER_MISSING);

            if (database == string.Empty)
                throw new ArgumentException(Properties.Resources.EXP_DATABASE_MISSING);

            string connectionString = $@"Data Source={server};Initial Catalog={database};Integrated Security=SSPI;TrustServerCertificate=true;";

            SqlConnection cnn = new(connectionString);
            cnn.Open();

            return cnn;
        }

        public static List<ValueEntryModel> GetValueEntries(string serverFromFile, string databaseFromFile, string companyFromFile, string DateFilter = "")
        {
            List<ValueEntryModel> output;
            string query = $"SELECT [Entry No_] AS EntryNo, [Posting Date] As PostingDate, [Cost Amount (Actual)] As CostAmountActual FROM [{companyFromFile}$Value Entry]";

            if (DateFilter != string.Empty)
            {
                string[] dates = DateFilter.Split("..");

                query += " WHERE ";
                
                if (dates[0] !=  string.Empty)
                    query += $"[Posting Date] >= '{dates[0]}'";

                if (dates[0] != string.Empty && dates[1] != string.Empty)
                    query += " AND ";
                
                if (dates[1] != string.Empty)
                    query += $"[Posting Date] < '{DateTime.Parse(dates[1]).AddDays(1):yyyy-MM-dd}'";
            }

            using (SqlConnection cnn = GetOpenConnectionToNavDatabase(serverFromFile, databaseFromFile))
            {
                output = cnn.Query<ValueEntryModel>(query + ";").AsList();
            }

            return output;
        }
    }
}
