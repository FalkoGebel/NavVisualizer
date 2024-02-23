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
        private static string VisualizerServerFromFile = "";
        private static string VisualizerDatabaseFromFile = "";
        private static VisualizerConnection Sut;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            string[] connectionFileData = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NavVisualizerTestConnectionData.txt");

            NavServerFromFile = connectionFileData[0];
            NavDatabaseFromFile = connectionFileData[1];
            CompanyFromFile = connectionFileData[2];
            VisualizerServerFromFile = connectionFileData[3];
            VisualizerDatabaseFromFile = connectionFileData[4];


            Sut = new(NavServerFromFile, NavDatabaseFromFile, CompanyFromFile, VisualizerServerFromFile, VisualizerDatabaseFromFile);
        }

        [TestMethod]
        public void GetAllValueEntriesAndNumberIsCorrect()
        {
            int expectedNumber = 389;
            List<ValueEntryModel> ValueEntries = NavDatabaseLogic.GetValueEntries(NavServerFromFile, NavDatabaseFromFile, CompanyFromFile);
            ValueEntries.Count.Should().Be(expectedNumber);
        }

        [TestMethod]
        public void GetAllValueEntriesViualizerConnectionAndNumberIsCorrect()
        {
            int expectedNumber = 389;
            List<ValueEntryModel> ValueEntries = Sut.GetValueEntries();
            ValueEntries.Count.Should().Be(expectedNumber);
        }

        [TestMethod]
        public void GetValueEntriesPerEndOfFirstQuqarter2019ViualizerConnectionAndNumberIsCorrect()
        {
            int expectedNumber = 346;
            DateTime? start = null;
            DateTime end = new(2019, 03, 31);
            Sut.SetDateFilter(start, end);
            List<ValueEntryModel> ValueEntries = Sut.GetValueEntries();
            ValueEntries.Count.Should().Be(expectedNumber);
        }

        [TestMethod]
        public void GetValueEntriesFirstQuarter2019ViualizerConnectionAndNumberIsCorrect()
        {
            int expectedNumber = 346;
            DateTime start = new(2019, 01, 01);
            DateTime end = new(2019, 03, 31);
            Sut.SetDateFilter(start, end);
            List<ValueEntryModel> ValueEntries = Sut.GetValueEntries();
            ValueEntries.Count.Should().Be(expectedNumber);
        }

        [TestMethod]
        public void GetAllValueEntriesViualizerConnectionAndFirstDateIsCorrect()
        {
            DateTime expectedFirstDate = new(2018, 06, 01);
            List<ValueEntryModel> ValueEntries = Sut.GetValueEntries();
            ValueEntries[0].PostingDate.Should().Be(expectedFirstDate);
        }
    }
}
