using FluentAssertions;
using VisualizerLibrary;
using VisualizerLibrary.Models;

namespace VisualizerLibraryTests
{
    [TestClass]
    public class VisualizerLogicValueEntryTests
    {
        private static string ServerFromFile = "";
        private static string DatabaseFromFile = "";
        private static string CompanyFromFile = "";
        private static VisualizerConnection Sut;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            string[] connectionFileData = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NavVisualizerTestConnectionData.txt");

            ServerFromFile = connectionFileData[0];
            DatabaseFromFile = connectionFileData[1];
            CompanyFromFile = connectionFileData[2];

            Sut = new(ServerFromFile, DatabaseFromFile, CompanyFromFile);
        }

        [TestMethod]
        public void GetAllValueEntriesAndNumberIsCorrect()
        {
            int expectedNumber = 389;
            List<ValueEntryModel> ValueEntries = VisualizerLogic.GetValueEntries(ServerFromFile, DatabaseFromFile, CompanyFromFile);
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
        public void GetValueEntriesFirstQuarter2019ViualizerConnectionAndNumberIsCorrect()
        {
            int expectedNumber = 17;
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
