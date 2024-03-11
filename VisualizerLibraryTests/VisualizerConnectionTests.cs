using FluentAssertions;
using VisualizerLibrary;
using VisualizerLibrary.Models;

namespace VisualizerLibraryTests
{
    [TestClass]
    public class VisualizerConnectionTests
    {
        private static string _navServerFromFile = "";
        private static string _navDatabaseFromFile = "";
        private static string _companyFromFile = "";
        private static string _visualizerServerFromFile = "";
        private static string _visualizerDatabaseFromFile = "";
        private static VisualizerConnection _sut = new();

        [ClassInitialize]
#pragma warning disable IDE0060 // Nicht verwendete Parameter entfernen
        public static void ClassInitialize(TestContext context)
#pragma warning restore IDE0060 // Nicht verwendete Parameter entfernen
        {
            string[] connectionFileData = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NavVisualizerTestConnectionData.txt");

            _navServerFromFile = connectionFileData[0];
            _navDatabaseFromFile = connectionFileData[1];
            _companyFromFile = connectionFileData[2];
            _visualizerServerFromFile = connectionFileData[3];
            _visualizerDatabaseFromFile = connectionFileData[4];

            _sut = new(_navServerFromFile, _navDatabaseFromFile, _companyFromFile, _visualizerServerFromFile, _visualizerDatabaseFromFile);
            _sut.UpdateVisualizerDatabaseFromNavDatabase();
        }

        [TestMethod]
        public void SetDateFilterToNothingAndEmptyDateFilter()
        {
            DateTime? start = null;
            DateTime? end = null;

            _sut.SetDateFilter(start, end);

            _sut.StartDate.Should().BeNull();
            _sut.EndDate.Should().BeNull();
        }

        [TestMethod]
        public void SetDateFilterOnlyStartAndCorrectFilter()
        {
            DateTime start = new(2019, 02, 01);
            DateTime? end = null;

            _sut.SetDateFilter(start, end);
            
            _sut.StartDate.Should().Be(start);
            _sut.EndDate.Should().BeNull();
        }

        [TestMethod]
        public void SetDateFilterOnlyEndAndCorrectFilter()
        {
            DateTime? start = null;
            DateTime end = new(2019, 02, 01);

            _sut.SetDateFilter(start, end);

            _sut.StartDate.Should().BeNull();
            _sut.EndDate.Should().Be(end);
        }

        [TestMethod]
        public void SetDateFilterStartAndEndAreEqualAndCorrectFilter()
        {
            DateTime start = new(2019, 02, 01);
            DateTime end = new(2019, 02, 01);

            _sut.SetDateFilter(start, end);

            _sut.StartDate.Should().Be(start);
            _sut.EndDate.Should().Be(end);
        }

        [TestMethod]
        public void SetDateFilterStartAndEndAreNotEqualAndCorrectFilter()
        {
            DateTime start = new(2019, 02, 01);
            DateTime end = new(2019, 05, 01);

            _sut.SetDateFilter(start, end);

            _sut.StartDate.Should().Be(start);
            _sut.EndDate.Should().Be(end);
        }

        [TestMethod]
        public void SetDateFilterEndBeforeStartAndCorrectException()
        {
            DateTime start = new(2019, 02, 01);
            DateTime end = new(2019, 01, 01);

            Action act = () => _sut.SetDateFilter(start, end);

            act.Should().Throw<ArgumentException>().WithMessage("End date must not be earlier than start date");
        }

        [TestMethod]
        public void GetCorrectNumberOfEntriesFromValuesPerDataAllDates()
        {
            List<ValuesPerDateCummulatedModel> values = _sut.GetValuesPerDateCummulatedForDates();
            values.Count.Should().Be(465);
        }

        [TestMethod]
        public void GetValuesPerDataAllDatesAndCorrectValuesOnFirstEntry()
        {
            DateTime expectedDate = new(2018, 06, 01);
            decimal expectedCostAmountActual = 97430.30M;
            List<ValuesPerDateCummulatedModel> values = _sut.GetValuesPerDateCummulatedForDates();
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
            List<ValuesPerDateCummulatedModel> values = _sut.GetValuesPerDateCummulatedForDates();
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
            List<ValuesPerDateCummulatedModel> values = _sut.GetValuesPerDateCummulatedForDates();
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
            List<ValuesPerDateCummulatedModel> values = _sut.GetValuesPerDateCummulatedForWeeks();
            values.Count.Should().Be(67);
        }

        [TestMethod]
        public void GetCorrectNumberOfEntriesFromValuesPerDataMonths()
        {
            List<ValuesPerDateCummulatedModel> values = _sut.GetValuesPerDateCummulatedForMonths();
            values.Count.Should().Be(15);
        }

        [TestMethod]
        public void GetCorrectNumberOfEntriesFromValuesPerDataQuarters()
        {
            List<ValuesPerDateCummulatedModel> values = _sut.GetValuesPerDateCummulatedForQuarters();
            values.Count.Should().Be(5);
        }

        [TestMethod]
        public void GetCorrectNumberOfEntriesFromValuesPerDataYears()
        {
            List<ValuesPerDateCummulatedModel> values = _sut.GetValuesPerDateCummulatedForYears();
            values.Count.Should().Be(1);
        }
    }
}
