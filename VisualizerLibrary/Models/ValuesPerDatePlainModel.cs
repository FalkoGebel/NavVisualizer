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
    }
}
