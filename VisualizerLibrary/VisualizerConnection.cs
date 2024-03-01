using VisualizerLibrary.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

            // Fill "Values Per Date" Tables
            List<ValueEntryModel> valueEntries = NavDatabaseLogic.GetValueEntries(NavServer, NavDatabase, Company);
            List<DateTime> dates = [.. valueEntries.Select(ve => ve.PostingDate).Distinct().OrderBy(d => d)];
            DateTime currentDate = dates[0];
            DateTime endDate = dates[^1];

            if (valueEntries.Count > 0 )
            {
                List<ValuesPerDateCummulatedModel> valuesPerDatesCummulated = [];
                ValuesPerDateCummulatedModel lastModel = new();
                List<ValuesPerDatePlainModel> valuesPerDatesPlain = [];
                ValuesPerDateCummulatedModel vpdcm;
                ValuesPerDatePlainModel vpdpm;
                bool endOfMonth;

                while (currentDate <= endDate)
                {
                    endOfMonth = currentDate.Day == DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                    // Fill "Values Per Date Cummulated" Table
                    vpdcm = new()
                    {
                        Date = currentDate,
                        EndOfWeek = ((int)currentDate.DayOfWeek) == 0,
                        EndOfMonth = endOfMonth,
                        EndOfQuarter = endOfMonth && currentDate.Month % 3 == 0,
                        EndOfYear = endOfMonth && currentDate.Month == 12
                    };

                    if (dates.Contains(currentDate))
                    {
                        vpdcm.CostAmountActual = valueEntries.Where(ve => ve.ItemLedgerEntryType != 7 && ve.PostingDate <= currentDate).Sum(ve => ve.CostAmountActual);
                        vpdcm.CostAmountExpected = valueEntries.Where(ve => ve.ItemLedgerEntryType != 7 && ve.PostingDate <= currentDate).Sum(ve => ve.CostAmountExpected);
                    }
                    else
                    {
                        vpdcm.CostAmountActual = lastModel.CostAmountActual;
                        vpdcm.CostAmountExpected = lastModel.CostAmountExpected;
                    }
                    valuesPerDatesCummulated.Add(vpdcm);
                    lastModel = vpdcm;

                    // Fill "Values Per Date Plain" Table

                    // Single date
                    vpdpm = new()
                    {
                        Date = currentDate,
                        SingleDate = true,
                        SalesAmountActual = valueEntries.Where(ve => ve.ItemLedgerEntryType == 1 && ve.PostingDate == currentDate).Sum(ve => ve.SalesAmountActual)
                    };
                    valuesPerDatesPlain.Add(vpdpm);

                    // End of week
                    if (currentDate.DayOfWeek == 0)
                    {
                        vpdpm = new()
                        {
                            Date = currentDate,
                            EndOfWeek = true,
                            SalesAmountActual = valueEntries.Where(ve => ve.ItemLedgerEntryType == 1 && ve.PostingDate > currentDate.AddDays(-7) && ve.PostingDate <= currentDate).Sum(ve => ve.SalesAmountActual)
                        };
                        valuesPerDatesPlain.Add(vpdpm);
                    }

                    // End of month
                    if (endOfMonth)
                    {
                        vpdpm = new()
                        {
                            Date = currentDate,
                            EndOfMonth = true,
                            SalesAmountActual = valueEntries.Where(ve => ve.ItemLedgerEntryType == 1 && ve.PostingDate > currentDate.AddMonths(-1) && ve.PostingDate <= currentDate).Sum(ve => ve.SalesAmountActual)
                        };
                        valuesPerDatesPlain.Add(vpdpm);
                    }

                    // End of quarter
                    if (endOfMonth && currentDate.Month % 3 == 0)
                    {
                        vpdpm = new()
                        {
                            Date = currentDate,
                            EndOfQuarter = true,
                            SalesAmountActual = valueEntries.Where(ve => ve.ItemLedgerEntryType == 1 && ve.PostingDate > currentDate.AddMonths(-3) && ve.PostingDate <= currentDate).Sum(ve => ve.SalesAmountActual)
                        };
                        valuesPerDatesPlain.Add(vpdpm);
                    }

                    // End of year
                    if (endOfMonth && currentDate.Month == 12)
                    {
                        vpdpm = new()
                        {
                            Date = currentDate,
                            EndOfYear = true,
                            SalesAmountActual = valueEntries.Where(ve => ve.ItemLedgerEntryType == 1 && ve.PostingDate > currentDate.AddMonths(-12) && ve.PostingDate <= currentDate).Sum(ve => ve.SalesAmountActual)
                        };
                        valuesPerDatesPlain.Add(vpdpm);
                    }

                    // TODO - think about the saving of the periods

                    currentDate = currentDate.AddDays(1);
                }
                VisualizerDatabaseLogic.FillValuesPerDateCummulatedTable(VisualizerServer, VisualizerDatabase, valuesPerDatesCummulated);

                // Add end of week for end date
                if (endDate.DayOfWeek != 0)
                {
                    DateTime endOfWeek = endDate.AddDays(7 - (int)endDate.DayOfWeek);
                    vpdpm = new()
                    {
                        Date = endOfWeek,
                        EndOfWeek = true,
                        SalesAmountActual = valueEntries.Where(ve => ve.ItemLedgerEntryType == 1 && ve.PostingDate > endOfWeek.AddDays(-7) && ve.PostingDate <= endOfWeek).Sum(ve => ve.SalesAmountActual)
                    };
                    valuesPerDatesPlain.Add(vpdpm);
                }
                
                endOfMonth = endDate.Day == DateTime.DaysInMonth(endDate.Year, endDate.Month);

                // Add end of month for end date
                if (!endOfMonth)
                {
                    DateTime endOfThisMonth = new(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month));
                    vpdpm = new()
                    {
                        Date = endOfThisMonth,
                        EndOfMonth = true,
                        SalesAmountActual = valueEntries.Where(ve => ve.ItemLedgerEntryType == 1 && ve.PostingDate > endOfThisMonth.AddMonths(-1) && ve.PostingDate <= endOfThisMonth).Sum(ve => ve.SalesAmountActual)
                    };
                    valuesPerDatesPlain.Add(vpdpm);
                }

                // Add end of quarter for end date
                if (!(endOfMonth && endDate.Month % 3 == 0))
                {
                    DateTime endOfQuarter = new(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month));
                    
                    while (endOfQuarter.Month % 3 != 0)
                        endOfQuarter.AddMonths(1);

                    vpdpm = new()
                    {
                        Date = endOfQuarter,
                        EndOfQuarter = true,
                        SalesAmountActual = valueEntries.Where(ve => ve.ItemLedgerEntryType == 1 && ve.PostingDate > endOfQuarter.AddMonths(-3) && ve.PostingDate <= endOfQuarter).Sum(ve => ve.SalesAmountActual)
                    };
                    valuesPerDatesPlain.Add(vpdpm);
                }

                // Add end of year for end date
                if (!(endOfMonth && endDate.Month == 12))
                {
                    DateTime endOfYear = new(endDate.Year, 12, 31);
                    vpdpm = new()
                    {
                        Date = endOfYear,
                        EndOfYear = true,
                        SalesAmountActual = valueEntries.Where(ve => ve.ItemLedgerEntryType == 1 && ve.PostingDate > endOfYear.AddMonths(-12) && ve.PostingDate <= endOfYear).Sum(ve => ve.SalesAmountActual)
                    };
                    valuesPerDatesPlain.Add(vpdpm);
                }

                VisualizerDatabaseLogic.FillValuesPerDatePlainTable(VisualizerServer, VisualizerDatabase, valuesPerDatesPlain);
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

        public List<ValuesPerDateCummulatedModel> GetValuesPerDateCummulatedForDates()
        {
            List<ValuesPerDateCummulatedModel> output = VisualizerDatabaseLogic.GetValuesPerDateCummulatedEntries(VisualizerServer, VisualizerDatabase);

            if (StartDate != null)
                output = output.Where(e => e.Date >= StartDate).ToList();

            if (EndDate != null)
                output = output.Where(e => e.Date <= EndDate).ToList();

            return output;
        }

        public List<ValuesPerDateCummulatedModel> GetValuesPerDateCummulatedForWeeks()
        {
            return GetValuesPerDateCummulatedForDates().Where(e => e.EndOfWeek).ToList();
        }

        public List<ValuesPerDateCummulatedModel> GetValuesPerDateCummulatedForMonths()
        {
            return GetValuesPerDateCummulatedForDates().Where(e => e.EndOfMonth).ToList();
        }

        public List<ValuesPerDateCummulatedModel> GetValuesPerDateCummulatedForQuarters()
        {
            return GetValuesPerDateCummulatedForDates().Where(e => e.EndOfQuarter).ToList();
        }

        public List<ValuesPerDateCummulatedModel> GetValuesPerDateCummulatedForYears()
        {
            return GetValuesPerDateCummulatedForDates().Where(e => e.EndOfYear).ToList();
        }

        public List<ValuesPerDatePlainModel> GetValuesPerDatePlain()
        {
            List<ValuesPerDatePlainModel> output = VisualizerDatabaseLogic.GetValuesPerDatePlainEntries(VisualizerServer, VisualizerDatabase);

            if (StartDate != null)
                output = output.Where(e => e.Date >= StartDate).ToList();

            if (EndDate != null)
                output = output.Where(e => e.Date <= EndDate).ToList();

            return output;
        }

        public List<ValuesPerDatePlainModel> GetValuesPerDatePlainForDates()
        {
            return GetValuesPerDatePlain().Where(e => e.SingleDate).ToList();
        }

        public List<ValuesPerDatePlainModel> GetValuesPerDatePlainForWeeks()
        {
            return GetValuesPerDatePlain().Where(e => e.EndOfWeek).ToList();
        }

        public List<ValuesPerDatePlainModel> GetValuesPerDatePlainForMonths()
        {
            return GetValuesPerDatePlain().Where(e => e.EndOfMonth).ToList();
        }

        public List<ValuesPerDatePlainModel> GetValuesPerDatePlainForQuarters()
        {
            return GetValuesPerDatePlain().Where(e => e.EndOfQuarter).ToList();
        }

        public List<ValuesPerDatePlainModel> GetValuesPerDatePlainForYears()
        {
            return GetValuesPerDatePlain().Where(e => e.EndOfYear).ToList();
        }
    }
}
