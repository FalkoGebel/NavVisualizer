using FluentAssertions;
using VisualizerLibrary;
using VisualizerLibrary.Models;

namespace VisualizerLibraryTests
{
    [TestClass]
    public class VisualizerConnectionTests
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
            Sut.UpdateVisualizerDatabaseFromNavDatabase();
        }

        [TestMethod]
        public void SetDateFilterToNothingAndEmptyDateFilter()
        {
            DateTime? start = null;
            DateTime? end = null;

            Sut.SetDateFilter(start, end);

            Sut.StartDate.Should().BeNull();
            Sut.EndDate.Should().BeNull();
        }

        [TestMethod]
        public void SetDateFilterOnlyStartAndCorrectFilter()
        {
            DateTime start = new(2019, 02, 01);
            DateTime? end = null;

            Sut.SetDateFilter(start, end);
            
            Sut.StartDate.Should().Be(start);
            Sut.EndDate.Should().BeNull();
        }

        [TestMethod]
        public void SetDateFilterOnlyEndAndCorrectFilter()
        {
            DateTime? start = null;
            DateTime end = new(2019, 02, 01);
            string expected = "..2019-02-01";

            Sut.SetDateFilter(start, end);

            Sut.StartDate.Should().BeNull();
            Sut.EndDate.Should().Be(end);
        }

        [TestMethod]
        public void SetDateFilterStartAndEndAreEqualAndCorrectFilter()
        {
            DateTime start = new(2019, 02, 01);
            DateTime end = new(2019, 02, 01);
            string expected = "2019-02-01..2019-02-01";

            Sut.SetDateFilter(start, end);

            Sut.StartDate.Should().Be(start);
            Sut.EndDate.Should().Be(end);
        }

        [TestMethod]
        public void SetDateFilterStartAndEndAreNotEqualAndCorrectFilter()
        {
            DateTime start = new(2019, 02, 01);
            DateTime end = new(2019, 05, 01);
            string expected = "2019-02-01..2019-05-01";

            Sut.SetDateFilter(start, end);

            Sut.StartDate.Should().Be(start);
            Sut.EndDate.Should().Be(end);
        }

        [TestMethod]
        public void SetDateFilterEndBeforeStartAndCorrectException()
        {
            DateTime start = new(2019, 02, 01);
            DateTime end = new(2019, 01, 01);

            Action act = () => Sut.SetDateFilter(start, end);

            act.Should().Throw<ArgumentException>().WithMessage("End date must not be earlier than start date");
        }

        [TestMethod]
        public void GetCorrectNumberOfEntriesFromValuesPerDataAllDates()
        {
            List<ValuesPerDateModel> values = Sut.GetValuesPerDateForDates();
            values.Count.Should().Be(465);
        }

        [TestMethod]
        public void GetValuesPerDataAllDatesAndCorrectValuesOnFirstEntry()
        {
            DateTime expectedDate = new(2018, 06, 01);
            decimal expectedCostAmountActual = 97430.30M;
            List<ValuesPerDateModel> values = Sut.GetValuesPerDateForDates();
            values[0].Date.Should().Be(expectedDate);
            values[0].EndOfWeek.Should().BeFalse();
            values[0].EndOfMonth.Should().BeFalse();
            values[0].EndOfQuarter.Should().BeFalse();
            values[0].EndOfYear.Should().BeFalse();
            values[0].CostAmountActual.Should().Be(expectedCostAmountActual);
        }

        [TestMethod]
        public void GetValuesPerDataAllDatesAndCorrectValuesOn30thEntry()
        {
            DateTime expectedDate = new(2018, 06, 30);
            decimal expectedCostAmountActual = 97430.30M;
            List<ValuesPerDateModel> values = Sut.GetValuesPerDateForDates();
            values[29].Date.Should().Be(expectedDate);
            values[29].EndOfWeek.Should().BeFalse();
            values[29].EndOfMonth.Should().BeTrue();
            values[29].EndOfQuarter.Should().BeTrue();
            values[29].EndOfYear.Should().BeFalse();
            values[29].CostAmountActual.Should().Be(expectedCostAmountActual);
        }

        [TestMethod]
        public void GetValuesPerDataAllDatesAndCorrectValuesOn214thEntry()
        {
            DateTime expectedDate = new(2018, 12, 31);
            decimal expectedCostAmountActual = 1763089.56M;
            List<ValuesPerDateModel> values = Sut.GetValuesPerDateForDates();
            values[213].Date.Should().Be(expectedDate);
            values[213].EndOfWeek.Should().BeFalse();
            values[213].EndOfMonth.Should().BeTrue();
            values[213].EndOfQuarter.Should().BeTrue();
            values[213].EndOfYear.Should().BeTrue();
            values[213].CostAmountActual.Should().Be(expectedCostAmountActual);
        }

        [TestMethod]
        public void GetCorrectNumberOfEntriesFromValuesPerDataWeeks()
        {
            List<ValuesPerDateModel> values = Sut.GetValuesPerDateForWeeks();
            values.Count.Should().Be(67);
        }

        [TestMethod]
        public void GetCorrectNumberOfEntriesFromValuesPerDataMonths()
        {
            List<ValuesPerDateModel> values = Sut.GetValuesPerDateForMonths();
            values.Count.Should().Be(15);
        }

        [TestMethod]
        public void GetCorrectNumberOfEntriesFromValuesPerDataQuarters()
        {
            List<ValuesPerDateModel> values = Sut.GetValuesPerDateForQuarters();
            values.Count.Should().Be(5);
        }

        [TestMethod]
        public void GetCorrectNumberOfEntriesFromValuesPerDataYears()
        {
            List<ValuesPerDateModel> values = Sut.GetValuesPerDateForYears();
            values.Count.Should().Be(1);
        }
    }
}
