namespace VisualizerLibrary.Models
{
    public class ValueEntryModel
    {
        public int EntryNo { get; set; }
        public int ItemLedgerEntryType { get; set; }
        public DateTime PostingDate { get; set; }
        public decimal CostAmountActual { get; set; }
        public decimal CostAmountExpected { get; set; }
        public decimal SalesAmountActual { get; set; }
        public string PostingDateAsShortString
        {
            get => PostingDate.ToShortDateString();
        }
        public decimal CostAmountTotal
        {
            get => CostAmountActual + CostAmountExpected;
        }
    }
}
