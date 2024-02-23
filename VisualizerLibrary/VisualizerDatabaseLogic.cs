using Dapper;
using Microsoft.Data.SqlClient;
using VisualizerLibrary.Models;

namespace VisualizerLibrary
{
    public static class VisualizerDatabaseLogic
    {
        public static SqlConnection GetOpenConnectionToVisualizerDatabase(string server, string database)
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

        public static bool CreateVisualizerDatabaseTables(string server, string database)
        {
            try
            {
                using SqlConnection cnn = new($@"Data Source={server};Initial Catalog={database};Integrated Security=SSPI;TrustServerCertificate=true;");

                // Table "Database Status"
                cnn.Query("CREATE TABLE [dbo].[Database Status](\r\n\t[ID] [int] IDENTITY(1,1) NOT NULL,\r\n\t[Calculation Timestamp] [datetime] NULL\r\n) ON [PRIMARY]");

                // Table "Values Per Date"
                cnn.Query("CREATE TABLE [dbo].[Values Per Date](\r\n\t[ID] [int] IDENTITY(1,1) NOT NULL,\r\n\t[Date] [date] NOT NULL,\r\n\t[End Of Week] [bit] NOT NULL,\r\n\t[End Of Month] [bit] NOT NULL,\r\n\t[End Of Quarter] [bit] NOT NULL,\r\n\t[End Of Year] [bit] NOT NULL,\r\n\t[Cost Amount (Actual)] [decimal](38, 20) NOT NULL\r\n) ON [PRIMARY]");
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool DropVisualizerDatabaseTables(string server, string database)
        {
            try
            {
                using SqlConnection cnn = new($@"Data Source={server};Initial Catalog={database};Integrated Security=SSPI;TrustServerCertificate=true;");

                // Table "Database Status"
                cnn.Query("DROP TABLE [Database Status]");

                // Table "Values Per Date"
                cnn.Query("DROP TABLE [Values Per Date]");
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static void FillValuesPerDateTable(string visualizerServer, string visualizerDatabase, List<ValuesPerDateModel> valuesPerDates)
        {
            if (valuesPerDates.Count == 0)
                return;

            const string query =
                @"INSERT INTO [dbo].[Values Per Date]
	                ([Date]
	                ,[End Of Week]
	                ,[End Of Month]
	                ,[End Of Quarter]
	                ,[End Of Year]
                    ,[Cost Amount (Actual)])
                VALUES
	                (@Date,
	                @EndOfWeek,
	                @EndOfMonth,
	                @EndOfQuarter,
	                @EndOfYear,
                    @CostAmountActual)";

            using SqlConnection cnn = GetOpenConnectionToVisualizerDatabase(visualizerServer, visualizerDatabase);
            cnn.Execute(query, param: valuesPerDates);
        }
    }
}
