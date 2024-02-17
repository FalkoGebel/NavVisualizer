using FluentAssertions;
using Microsoft.Data.SqlClient;
using VisualizerLibrary;

namespace VisualizerLibraryTests
{
    [TestClass]
    public class VisualizerLogicConnectionTests
    {
        private static string ServerFromFile = "";
        private static string DatabaseFromFile = "";
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            string[] connectionFileData = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NavVisualizerTestConnectionData.txt");

            ServerFromFile = connectionFileData[0];
            DatabaseFromFile = connectionFileData[1];
        }

        [TestMethod]
        public void ConnectToDatabaseNoServerAndServerMissingException()
        {
            string server = "";
            string database = "";

            Action act = () => VisualizerLogic.GetOpenConnectionToNavDatabase(server, database);

            act.Should().Throw<ArgumentException>().WithMessage("No server specified");
        }

        [TestMethod]
        public void ConnectToDatabaseServerSetNoDatabaseAndDatabaseMissingException()
        {
            string server = "server";
            string database = "";

            Action act = () => VisualizerLogic.GetOpenConnectionToNavDatabase(server, database);

            act.Should().Throw<ArgumentException>().WithMessage("No database specified");
        }

        [TestMethod]
        public void ConnectToDatabaseInvalidServerAndDatabaseSetAndNoConnection()
        {
            string server = "server";
            string database = "database";

            Action act = () => VisualizerLogic.GetOpenConnectionToNavDatabase(server, database);

            act.Should().Throw<SqlException>().WithMessage("*The server was not found or was not accessible*");
        }

        [TestMethod]
        public void ConnectToDatabaseWithValidDataFromDesktopFileAndConnectionNotNullAndOpen()
        {
            SqlConnection cnn = VisualizerLogic.GetOpenConnectionToNavDatabase(ServerFromFile, DatabaseFromFile);
            cnn.Should().NotBeNull();
            cnn.State.Should().Be(System.Data.ConnectionState.Open);
            cnn.Close();
        }
    }
}