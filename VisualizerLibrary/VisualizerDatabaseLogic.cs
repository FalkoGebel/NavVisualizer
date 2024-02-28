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

                // Table "Values Per Date Cummulated"
                cnn.Query("CREATE TABLE [dbo].[Values Per Date Cummulated](\r\n\t[ID] [int] IDENTITY(1,1) NOT NULL,\r\n\t[Date] [date] NOT NULL,\r\n\t[End Of Week] [bit] NOT NULL,\r\n\t[End Of Month] [bit] NOT NULL,\r\n\t[End Of Quarter] [bit] NOT NULL,\r\n\t[End Of Year] [bit] NOT NULL,\r\n\t[Cost Amount (Actual)] [decimal](38, 20) NOT NULL,\r\n\t[Cost Amount (Expected)] [decimal](38, 20) NOT NULL\r\n) ON [PRIMARY]");

                // Table "Values Per Date Plain"
                cnn.Query("CREATE TABLE [dbo].[Values Per Date Plain](\r\n\t[ID] [int] IDENTITY(1,1) NOT NULL,\r\n\t[Date] [date] NOT NULL,\r\n\t[Single Date] [bit] NOT NULL,\r\n\t[End Of Week] [bit] NOT NULL,\r\n\t[End Of Month] [bit] NOT NULL,\r\n\t[End Of Quarter] [bit] NOT NULL,\r\n\t[End Of Year] [bit] NOT NULL,\r\n\t[Sales Amount (Actual)] [decimal](38, 20) NOT NULL\r\n) ON [PRIMARY]");
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

                // Table "Values Per Date Cummulated"
                cnn.Query("DROP TABLE [Values Per Date Cummulated]");

                // Table "Values Per Date Plain"
                cnn.Query("DROP TABLE [Values Per Date Plain]");
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static void FillValuesPerDateCummulatedTable(string visualizerServer, string visualizerDatabase, List<ValuesPerDateCummulatedModel> valuesPerDates)
        {
            if (valuesPerDates.Count == 0)
                return;

            const string query =
                @"INSERT INTO [dbo].[Values Per Date Cummulated]
	                (
                     [Date]
	                ,[End Of Week]
	                ,[End Of Month]
	                ,[End Of Quarter]
	                ,[End Of Year]
                    ,[Cost Amount (Actual)]
                    ,[Cost Amount (Expected)]
                    )
                VALUES
	                (
                    @Date,
	                @EndOfWeek,
	                @EndOfMonth,
	                @EndOfQuarter,
	                @EndOfYear,
                    @CostAmountActual,
                    @CostAmountExpected
                    )";

            using SqlConnection cnn = GetOpenConnectionToVisualizerDatabase(visualizerServer, visualizerDatabase);
            cnn.Execute(query, param: valuesPerDates);
        }

        internal static List<ValuesPerDateCummulatedModel> GetValuesPerDateCummulatedEntries(string server, string database)
        {
            string query =
                @$"SELECT [Date] AS Date
                         ,[End Of Week] AS EndOfWeek
                         ,[End Of Month] AS EndOfMonth
                         ,[End Of Quarter] AS EndOfQuarter
                         ,[End Of Year] AS EndOfYear
                         ,[Cost Amount (Actual)] AS CostAmountActual
                         ,[Cost Amount (Expected)] AS CostAmountExpected
                FROM[dbo].[Values Per Date Cummulated]";

            using SqlConnection cnn = GetOpenConnectionToVisualizerDatabase(server, database);
            return [.. cnn.Query<ValuesPerDateCummulatedModel>(query).AsList().OrderBy(e => e.Date)];
        }

        internal static List<ValuesPerDatePlainModel> GetValuesPerDatePlainEntries(string server, string database)
        {
            string query =
                @$"SELECT [Date] AS Date
                         ,[Single Date] AS SingleDate
                         ,[End Of Week] AS EndOfWeek
                         ,[End Of Month] AS EndOfMonth
                         ,[End Of Quarter] AS EndOfQuarter
                         ,[End Of Year] AS EndOfYear
                         ,[Sales Amount (Actual)] AS SalesAmountActual
                FROM[dbo].[Values Per Date Plain]";

            using SqlConnection cnn = GetOpenConnectionToVisualizerDatabase(server, database);
            return [.. cnn.Query<ValuesPerDatePlainModel>(query).AsList().OrderBy(e => e.Date)];
        }

        internal static void FillValuesPerDatePlainTable(string visualizerServer, string visualizerDatabase, List<ValuesPerDatePlainModel> valuesPerDatesPlain)
        {
            if (valuesPerDatesPlain.Count == 0)
                return;

            const string query =
                @"INSERT INTO [dbo].[Values Per Date Plain]
	                (
                     [Date]
	                ,[Single Date]
                    ,[End Of Week]
	                ,[End Of Month]
	                ,[End Of Quarter]
	                ,[End Of Year]
                    ,[Sales Amount (Actual)]
                    )
                VALUES
	                (
                    @Date,
                    @SingleDate,
	                @EndOfWeek,
	                @EndOfMonth,
	                @EndOfQuarter,
	                @EndOfYear,
                    @SalesAmountActual
                    )";

            using SqlConnection cnn = GetOpenConnectionToVisualizerDatabase(visualizerServer, visualizerDatabase);
            cnn.Execute(query, param: valuesPerDatesPlain);
        }
    }
}
