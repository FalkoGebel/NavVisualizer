using VisualizerLibrary.Models;

namespace VisualizerLibrary
{
    public class VisualizerConnection(string server, string database, string company)
    {
        public string Server { get; set; } = server;
        public string Database { get; set; } = database;
        public string Company { get; set; } = company;
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        public List<ValueEntryModel> GetValueEntries()
        {
            return VisualizerLogic.GetValueEntries(Server, Database, Company, EndDate);
        }

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
