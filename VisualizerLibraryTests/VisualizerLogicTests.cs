using FluentAssertions;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using VisualizerLibrary;

namespace VisualizerLibraryTests
{
    [TestClass]
    public class VisualizerLogicTests
    {
        [TestMethod]
        public void ConnectToDatabaseNoServerAndServerMissingException()
        {
            string server = "";
            string database = "";

            Action act = () => VisualizerLogic.ConnectToNavDatabase(server, database);

            act.Should().Throw<ArgumentException>().WithMessage("No server specified");
        }

        [TestMethod]
        public void ConnectToDatabaseServerSetNoDatabaseAndDatabaseMissingException()
        {
            string server = "server";
            string database = "";

            Action act = () => VisualizerLogic.ConnectToNavDatabase(server, database);

            act.Should().Throw<ArgumentException>().WithMessage("No database specified");
        }

        [TestMethod]
        public void ConnectToDatabaseInvalidServerAndDatabaseSetAndNoConnection()
        {
            string server = "server";
            string database = "database";

            Action act = () => VisualizerLogic.ConnectToNavDatabase(server, database);

            act.Should().Throw<SqlException>().WithMessage("*The server was not found or was not accessible*");
        }
    }
}