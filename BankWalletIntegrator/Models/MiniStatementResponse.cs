using System.Collections.Generic;

namespace BankWalletIntegrator.Models
{
    public class MiniStatementResponse:Response
    {
        public List<Statement> LastNTransactions { get; set; }
    }

    public class Statement
    {
        public string TranId { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string Currency { get; set; }        
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
    }
}
