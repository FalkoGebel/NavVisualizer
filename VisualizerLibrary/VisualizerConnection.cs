using System.Collections.Generic;
using VisualizerLibrary.Models;

namespace VisualizerLibrary
{
    public class VisualizerConnection
    {
        public string NavServer { get; set; }
        public string NavDatabase { get; set; }
        public string Company { get; set; }
        public string VisualizerServer { get; set; }
        public string VisualizerDatabase { get; set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        public VisualizerConnection()
        {
            NavServer = "";
            NavDatabase = "";
            Company = "";
            VisualizerServer = "";
            VisualizerDatabase = "";
        }

        public VisualizerConnection(string navServer, string navDatabase, string company, string visualizerServer, string visualizerDatabase)
        {
            NavServer = navServer;
            NavDatabase = navDatabase;
            Company = company;
            VisualizerServer = visualizerServer;
            VisualizerDatabase = visualizerDatabase;
        }


        public void UpdateVisualizerDatabaseFromNavDatabase()
        {
            // Drop Tables
            VisualizerDatabaseLogic.DropVisualizerDatabaseTables(VisualizerServer, VisualizerDatabase);

            // Create Tables
            VisualizerDatabaseLogic.CreateVisualizerDatabaseTables(VisualizerServer, VisualizerDatabase);
            
            // Fill "Values Per Date" Table
            List<ValueEntryModel> valueEntries = NavDatabaseLogic.GetValueEntries(NavServer, NavDatabase, Company);
            if (valueEntries.Count > 0 )
            {
                List<DateTime> dates = [.. valueEntries.Select(ve => ve.PostingDate).Distinct().OrderBy(d => d)];
                DateTime currentDate = dates[0];
                DateTime endDate = dates[^1];
                List<ValuesPerDateModel> valuesPerDates = [];
                ValuesPerDateModel lastModel = new();
                while (currentDate <= endDate)
                {
                    bool endOfMonth = currentDate.Day == DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                    ValuesPerDateModel vpdm = new()
                    {
                        Date = currentDate,
                        EndOfWeek = ((int)currentDate.DayOfWeek) == 0,
                        EndOfMonth = endOfMonth,
                        EndOfQuarter = endOfMonth && currentDate.Month % 3 == 0,
                        EndOfYear = endOfMonth && currentDate.Month == 12
                    };

                    if (dates.Contains(currentDate))
                    {
                        vpdm.CostAmountActual = valueEntries.Where(ve => ve.PostingDate <= currentDate).Sum(ve => ve.CostAmountActual);
                        vpdm.CostAmountExpected = valueEntries.Where(ve => ve.PostingDate <= currentDate).Sum(ve => ve.CostAmountExpected);
                    }
                    else
                    {
                        vpdm.CostAmountActual = lastModel.CostAmountActual;
                        vpdm.CostAmountExpected = lastModel.CostAmountExpected;
                    }

                    valuesPerDates.Add(vpdm);
                    lastModel = vpdm;
                    currentDate = currentDate.AddDays(1);
                }
                VisualizerDatabaseLogic.FillValuesPerDateTable(VisualizerServer, VisualizerDatabase, valuesPerDates);
            }
        }

        public void SetDateFilter(DateTime? start, DateTime? end)
        {
            StartDate = start;
            EndDate = end;
            
            if (StartDate == null && EndDate == null)
                return;

            if (EndDate != null && EndDate < StartDate)
                throw new ArgumentException(Properties.Resources.EXP_END_BEFORE_START_DATE);
        }

        public List<ValuesPerDateModel> GetValuesPerDateForDates()
        {
            List<ValuesPerDateModel> output = VisualizerDatabaseLogic.GetValuesPerDateEntries(VisualizerServer, VisualizerDatabase);

            if (StartDate != null)
                output = output.Where(e => e.Date >= StartDate).ToList();

            if (EndDate != null)
                output = output.Where(e => e.Date <= EndDate).ToList();

            return output;
        }

        public List<ValuesPerDateModel> GetValuesPerDateForWeeks()
        {
            return GetValuesPerDateForDates().Where(e => e.EndOfWeek).ToList();
        }

        public List<ValuesPerDateModel> GetValuesPerDateForMonths()
        {
            return GetValuesPerDateForDates().Where(e => e.EndOfMonth).ToList();
        }

        public List<ValuesPerDateModel> GetValuesPerDateForQuarters()
        {
            return GetValuesPerDateForDates().Where(e => e.EndOfQuarter).ToList();
        }

        public List<ValuesPerDateModel> GetValuesPerDateForYears()
        {
            return GetValuesPerDateForDates().Where(e => e.EndOfYear).ToList();
        }
    }
}
