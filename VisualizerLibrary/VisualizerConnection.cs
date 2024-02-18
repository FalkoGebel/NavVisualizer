using VisualizerLibrary.Models;

namespace VisualizerLibrary
{
    public class VisualizerConnection(string server, string database, string company)
    {
        public string Server { get; set; } = server;
        public string Database { get; set; } = database;
        public string Company { get; set; } = company;
        public string DateFilter { get; private set; } = "";

        public List<ValueEntryModel> GetValueEntries()
        {
            return VisualizerLogic.GetValueEntries(Server, Database, Company, DateFilter);
        }

        public List<ValueEntryModel> GetValueEntriesCalcSumsPerExistingDate()
        {
            List<ValueEntryModel> valueEntries = GetValueEntries();
            List<DateTime> dates = [.. valueEntries.Select(ve => ve.PostingDate).Distinct().OrderBy(d => d)];

            List<ValueEntryModel> output = [];

            foreach(DateTime d in dates)
            {
                ValueEntryModel ve = new()
                {
                    PostingDate = d,
                    CostAmountActual = valueEntries.Where(entry => entry.PostingDate <= d).Select(entry => entry.CostAmountActual).Sum()
                };
                output.Add(ve);
            }

            return output;
        }

        public void SetDateFilter(DateTime start, DateTime end)
        {
            DateFilter = "";
            
            if (start == DateTime.MinValue && end == DateTime.MinValue)
                return;

            if (end != DateTime.MinValue && end < start)
                throw new ArgumentException(Properties.Resources.EXP_END_BEFORE_START_DATE);
            
            if (start != DateTime.MinValue)
                DateFilter = start.ToString("yyyy-MM-dd");
            
            DateFilter += "..";

            if (end != DateTime.MinValue)
                DateFilter += end.ToString("yyyy-MM-dd");
        }
    }
}
