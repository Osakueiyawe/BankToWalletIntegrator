namespace BankWalletIntegrator.Models
{
    public class MultipleTransactions
    {
        public string AccountNumber { get; set; }
        public string PartTranType { get; set; }
        public decimal Amount { get; set; }
        public string Narration { get; set; }
        public string Narration2 { get; set; }
        public string Currency { get; set; }
    }
}
