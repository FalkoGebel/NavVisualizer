namespace VisualizerLibrary.Models
{
    public class ValuesPerDateModel
    {
        public DateTime Date { get; set; }
        public bool EndOfWeek { get; set; }
        public bool EndOfMonth { get; set; }
        public bool EndOfQuarter { get; set; }
        public bool EndOfYear { get; set; }
        public decimal CostAmountActual { get; set; }
        public decimal CostAmountExpected { get; set; }
    }
}
