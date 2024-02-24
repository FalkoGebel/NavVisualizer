using FluentAssertions;
using VisualizerLibrary;
using VisualizerLibrary.Models;

namespace VisualizerLibraryTests
{
    [TestClass]
    public class VisualizerLogicValueEntryTests
    {
        private static string NavServerFromFile = "";
        private static string NavDatabaseFromFile = "";
        private static string CompanyFromFile = "";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            string[] connectionFileData = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NavVisualizerTestConnectionData.txt");

            NavServerFromFile = connectionFileData[0];
            NavDatabaseFromFile = connectionFileData[1];
            CompanyFromFile = connectionFileData[2];
        }

        [TestMethod]
        public void GetAllValueEntriesAndNumberIsCorrect()
        {
            int expectedNumber = 389;
            List<ValueEntryModel> ValueEntries = NavDatabaseLogic.GetValueEntries(NavServerFromFile, NavDatabaseFromFile, CompanyFromFile);
            ValueEntries.Count.Should().Be(expectedNumber);
        }
    }
}
