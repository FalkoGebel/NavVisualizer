using FluentAssertions;
using Microsoft.Data.SqlClient;
using VisualizerLibrary;

namespace VisualizerLibraryTests
{
    [TestClass]
    public class VisualizerLogicConnectionTests
    {
        private static string _serverFromFile = "";
        private static string _databaseFromFile = "";
        
        [ClassInitialize]
#pragma warning disable IDE0060 // Nicht verwendete Parameter entfernen
        public static void ClassInitialize(TestContext context)
#pragma warning restore IDE0060 // Nicht verwendete Parameter entfernen
        {
            string[] connectionFileData = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NavVisualizerTestConnectionData.txt");

            _serverFromFile = connectionFileData[0];
            _databaseFromFile = connectionFileData[1];
        }

        [TestMethod]
        public void ConnectToDatabaseNoServerAndServerMissingException()
        {
            string server = "";
            string database = "";

            Action act = () => NavDatabaseLogic.GetOpenConnectionToNavDatabase(server, database);

            act.Should().Throw<ArgumentException>().WithMessage("No server specified");
        }

        [TestMethod]
        public void ConnectToDatabaseServerSetNoDatabaseAndDatabaseMissingException()
        {
            string server = "server";
            string database = "";

            Action act = () => NavDatabaseLogic.GetOpenConnectionToNavDatabase(server, database);

            act.Should().Throw<ArgumentException>().WithMessage("No database specified");
        }

        [TestMethod]
        public void ConnectToDatabaseInvalidServerAndDatabaseSetAndNoConnection()
        {
            string server = "server";
            string database = "database";

            Action act = () => NavDatabaseLogic.GetOpenConnectionToNavDatabase(server, database);

            act.Should().Throw<SqlException>().WithMessage("*The server was not found or was not accessible*");
        }

        [TestMethod]
        public void ConnectToDatabaseWithValidDataFromDesktopFileAndConnectionNotNullAndOpen()
        {
            SqlConnection cnn = NavDatabaseLogic.GetOpenConnectionToNavDatabase(_serverFromFile, _databaseFromFile);
            cnn.Should().NotBeNull();
            cnn.State.Should().Be(System.Data.ConnectionState.Open);
            cnn.Close();
        }
    }
}