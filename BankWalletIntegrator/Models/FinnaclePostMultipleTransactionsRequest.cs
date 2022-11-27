using System.Collections.Generic;

namespace BankWalletIntegrator.Models
{
    public class FinnaclePostMultipleTransactionsRequest:BaseRequest
    {
        public List<MultipleTransactions> Transactions { get; set; }
        public string ClientReferenceId { get; set; }
    }
}
