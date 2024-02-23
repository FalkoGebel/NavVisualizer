using FluentAssertions;
using VisualizerLibrary;

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
            string expected = "2019-02-01..";

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
    }
}
