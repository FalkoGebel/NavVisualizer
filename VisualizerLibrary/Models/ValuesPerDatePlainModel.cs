using System.Globalization;

namespace VisualizerLibrary.Models
{
    public class ValuesPerDatePlainModel
    {
        public DateTime Date { get; set; }
        public bool SingleDate { get; set; }
        public bool EndOfWeek { get; set; }
        public bool EndOfMonth { get; set; }
        public bool EndOfQuarter { get; set; }
        public bool EndOfYear { get; set; }
        public decimal SalesAmountActual { get; set; }

        public string LongChartLabel
        {
            get
            {
                string output = "";

                if (SingleDate)
                    return $"{Date.Year}-{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Date.Month)}-{Date.Day}";

                if (EndOfWeek)
                {
                    Calendar cal = CultureInfo.CurrentCulture.Calendar;
                    int week = cal.GetWeekOfYear(Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    return $"{Date.Year}/W{week}";
                }

                if (EndOfMonth)
                    return $"{Date.Year}/{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Date.Month)}";

                if (EndOfQuarter)
                {
                    int quarter = (int)Math.Ceiling(Date.Month / 3d);
                    output += $"Q{quarter}";
                    return output;
                }

                if (EndOfYear)
                    return Date.Year.ToString();

                return string.Empty;
            }
        }

        public string ChartLabel
        {
            get
            {
                if (SingleDate)
                {
                    if (Date.Day == 1 && Date.Month == 1)
                        return LongChartLabel;

                    if (Date.Day == 1)
                        return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Date.Month)}-{Date.Day}";

                    return $"D{Date.Day}";
                }

                if (EndOfWeek)
                {
                    Calendar cal = CultureInfo.CurrentCulture.Calendar;
                    int week = cal.GetWeekOfYear(Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    if (week == 1)
                        return LongChartLabel;
                    return $"W{week}";
                }

                if (EndOfMonth)
                {
                    if (Date.Month == 1)
                        return LongChartLabel;
                    return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Date.Month)}";
                }

                if (EndOfQuarter)
                {
                    int quarter = (int)Math.Ceiling(Date.Month / 3d);
                    if (quarter == 1)
                        return LongChartLabel;
                    return $"Q{quarter}";
                }

                if (EndOfYear)
                    return Date.Year.ToString();
                
                return string.Empty;
            }
        }
    }
}
