﻿using Dapper;
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

        public static List<ValueEntryModel> GetValueEntries(string serverFromFile, string databaseFromFile, string companyFromFile)
        {
            List<ValueEntryModel> output;
            string query = $"SELECT [Entry No_], [Posting Date] FROM [{companyFromFile}$Value Entry];";

            using (SqlConnection cnn = GetOpenConnectionToNavDatabase(serverFromFile, databaseFromFile))
            {
                output = cnn.Query<ValueEntryModel>(query).ToList();
            }

            return output;
        }
    }
}
