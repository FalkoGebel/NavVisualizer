using FluentAssertions;
using VisualizerLibrary;
using VisualizerLibrary.Models;

namespace VisualizerLibraryTests
{
    [TestClass]
    public class VisualizerLogicValueEntryTests
    {
        private static string _navServerFromFile = "";
        private static string _navDatabaseFromFile = "";
        private static string _companyFromFile = "";

        [ClassInitialize]
#pragma warning disable IDE0060 // Nicht verwendete Parameter entfernen
        public static void ClassInitialize(TestContext context)
#pragma warning restore IDE0060 // Nicht verwendete Parameter entfernen
        {
            string[] connectionFileData = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NavVisualizerTestConnectionData.txt");

            _navServerFromFile = connectionFileData[0];
            _navDatabaseFromFile = connectionFileData[1];
            _companyFromFile = connectionFileData[2];
        }

        [TestMethod]
        public void GetAllValueEntriesAndNumberIsCorrect()
        {
            int expectedNumber = 389;
            List<ValueEntryModel> ValueEntries = NavDatabaseLogic.GetValueEntries(_navServerFromFile, _navDatabaseFromFile, _companyFromFile);
            ValueEntries.Count.Should().Be(expectedNumber);
        }
    }
}
