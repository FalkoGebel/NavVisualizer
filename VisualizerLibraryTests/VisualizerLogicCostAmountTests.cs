using FluentAssertions;
using VisualizerLibrary;
using VisualizerLibrary.Models;

namespace VisualizerLibraryTests
{
    [TestClass]
    public class VisualizerLogicCostAmountTests
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

        [TestInitialize]
        public void TestInitialize()
        {
            Sut.SetDateFilter(null, null);
        }
        
        [TestMethod]
        public void CostAmountActualSeriesForExistingDatesWithoutDateFilterAndCorrectValues()
        {
            DateTime firstDate = new(2018, 06, 01);
            decimal firstValue = 97430.3m;

            List<ValueEntryModel> valueEntries = Sut.GetValueEntriesCalcSumsPerExistingDate();

            valueEntries[0].PostingDate.Should().Be(firstDate);
            valueEntries[0].CostAmountActual.Should().Be(firstValue);
        }

        [TestMethod]
        public void CostAmountActualSeriesForExistingDatesWithoutDateFilterAndCorrectNumber()
        {
            int expected = 31;

            List<ValueEntryModel> valueEntries = Sut.GetValueEntriesCalcSumsPerExistingDate();

            valueEntries.Count.Should().Be(expected);
        }

        [TestMethod]
        public void CostAmountActualSeriesForExistingDatesWithDateFilterAndCorrectNumber()
        {
            int expected = 30;

            Sut.SetDateFilter(new(2018, 07, 01), null);
            List<ValueEntryModel> valueEntries = Sut.GetValueEntriesCalcSumsPerExistingDate();

            valueEntries.Count.Should().Be(expected);
        }

        [TestMethod]
        public void CostAmountActualSeriesForExistingDatesWithDateFilter2AndCorrectNumber()
        {
            int expected = 29;

            Sut.SetDateFilter(new(2018, 07, 01), new(2019, 09, 07));
            List<ValueEntryModel> valueEntries = Sut.GetValueEntriesCalcSumsPerExistingDate();

            valueEntries.Count.Should().Be(expected);
        }
    }
}
