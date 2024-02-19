using FluentAssertions;
using VisualizerLibrary;

namespace VisualizerLibraryTests
{
    [TestClass]
    public class VisualizerConnectionTests
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
