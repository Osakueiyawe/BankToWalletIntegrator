namespace BankWalletIntegrator.Models
{
    public class RequestTypes
    {
        public static string accountBalance { get; set; } = "GETACCBAL";
        public static string miniStatement { get; set; } = "GETMINI";
        public static string bankToWallet { get; set; } = "A2W";
        public static string walletToBank { get; set; } = "W2A";
        public static string transactionInquiry { get; set; } = "TRANINQ";
        public static string cancelTransaction { get; set; } = "CANCELTRAN";
    }
}
