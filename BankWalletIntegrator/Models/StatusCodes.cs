namespace BankWalletIntegrator.Models
{
    public class StatusCodes
    {
        public static string success { get; set; } = "00";
        public static string validationError { get; set; } = "01";
        public static string technicalError { get; set; } = "02";
        public static string finnacleError { get; set; } = "03";
    }
}
