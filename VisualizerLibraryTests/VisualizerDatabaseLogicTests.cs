using FluentAssertions;
using VisualizerLibrary;

namespace VisualizerLibraryTests
{
    [TestClass]
    public class VisualizerDatabaseLogicTests
    {
        private static string ServerFromFile = "";
        private static string DatabaseFromFile = "";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            string[] connectionFileData = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NavVisualizerTestConnectionData.txt");

            ServerFromFile = connectionFileData[3];
            DatabaseFromFile = connectionFileData[4];
        }

        [TestMethod]
        public void CreateAndDropAllVisualizerDatabaseTablesAndResultIsTrue()
        {
            bool resultCreate = VisualizerDatabaseLogic.CreateVisualizerDatabaseTables(ServerFromFile, DatabaseFromFile);
            resultCreate.Should().BeTrue();
            bool resultDrop = VisualizerDatabaseLogic.DropVisualizerDatabaseTables(ServerFromFile, DatabaseFromFile);
            resultDrop.Should().BeTrue();
        }
    }
}
