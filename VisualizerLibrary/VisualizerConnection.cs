using VisualizerLibrary.Models;

namespace VisualizerLibrary
{
    public class VisualizerConnection(string navServer, string navDatabase, string company, string visualizerServer, string visualizerDatabase)
    {
        public string NavServer { get; set; } = navServer;
        public string NavDatabase { get; set; } = navDatabase;
        public string Company { get; set; } = company;
        public string VisualizerServer { get; set; } = visualizerServer;
        public string VisualizerDatabase { get; set; } = visualizerDatabase;
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

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
                    }
                    else
                    {
                        vpdm.CostAmountActual = lastModel.CostAmountActual;
                    }

                    valuesPerDates.Add(vpdm);
                    lastModel = vpdm;
                    currentDate = currentDate.AddDays(1);
                }
                VisualizerDatabaseLogic.FillValuesPerDateTable(VisualizerServer, VisualizerDatabase, valuesPerDates);
            }
        }

        [Obsolete("Maybe not longer needed - check")]
        public List<ValueEntryModel> GetValueEntries()
        {
            return NavDatabaseLogic.GetValueEntries(NavServer, NavDatabase, Company);
        }

        [Obsolete("Maybe not longer needed - check")]
        public List<ValueEntryModel> GetValueEntriesCalcSumsPerExistingDate()
        {
            List<ValueEntryModel> valueEntries = GetValueEntries();
            List<DateTime> dates = [.. valueEntries.Select(ve => ve.PostingDate).Distinct().OrderBy(d => d)];

            if (StartDate != null)
                dates = [..dates.Where(d => d >= StartDate)];

            List<ValueEntryModel> output = [];

            foreach(DateTime d in dates)
            {
                ValueEntryModel ve = new()
                {
                    PostingDate = d,
                    CostAmountActual = valueEntries.Where(entry => entry.PostingDate <= d).Select(entry => entry.CostAmountActual).Sum(),
                    CostAmountExpected = valueEntries.Where(entry => entry.PostingDate <= d).Select(entry => entry.CostAmountExpected).Sum()
                };
                output.Add(ve);
            }

            return output;
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
    }
}
