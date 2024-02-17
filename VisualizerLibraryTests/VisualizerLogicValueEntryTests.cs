using FluentAssertions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            string[] connectionFileData = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NavVisualizerTestConnectionData.txt");

            ServerFromFile = connectionFileData[0];
            DatabaseFromFile = connectionFileData[1];
            CompanyFromFile = connectionFileData[2];
        }

        [TestMethod]
        public void GetAllValueEntriesAndNumberIsCorrect()
        {
            int expectedNumber = 389;
            List<ValueEntryModel> ValueEntries = VisualizerLogic.GetValueEntries(ServerFromFile, DatabaseFromFile, CompanyFromFile);
            ValueEntries.Count.Should().Be(expectedNumber);
        }

    }
}
