using Dapper;
using Microsoft.Data.SqlClient;

namespace VisualizerLibrary
{
    public static class VisualizerDatabaseLogic
    {
        public static bool CreateVisualizerDatabaseTables(string server, string database)
        {
            string query = "CREATE TABLE [dbo].[Database Status](\r\n\t[ID] [int] IDENTITY(1,1) NOT NULL,\r\n\t[Calculation Timestamp] [datetime] NULL,\r\n CONSTRAINT [PK_Database Status] PRIMARY KEY CLUSTERED \r\n(\r\n\t[ID] ASC\r\n)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]\r\n) ON [PRIMARY]";

            try
            {
                using SqlConnection cnn = new($@"Data Source={server};Initial Catalog={database};Integrated Security=SSPI;TrustServerCertificate=true;");
                cnn.Query(query);
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
                cnn.Query("DROP TABLE [Database Status]");
            } catch
            {
                return false;
            }

            return true;
        }
    }
}
