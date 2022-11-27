using BankWalletIntegrator.Models;

namespace BankWalletIntegrator.RequestModels
{
    public class OrangeSenegalRequest
    {
    }

    public class OrangeSuscriberAccountRequest : BaseRequest
    {
        public string AccountAlias { get; set; }
    }

    public class OrangeSuscriberAccountResponse : Response
    {
        public string AccountNumber { get; set; }
        public string MobileNumber { get; set; }
    }
    public class OrangeSenegalAccountBalanceRequest:Request
    {
        public string OperatorCode { get; set; }
        public string RequestToken { get; set; }
        public string RequestType { get; set; }
        public string AffiliateCode { get; set; }
        public string AccountNo { get; set; }
        public string AccountAlias { get; set; }
        public string Reason { get; set; }
    }

    public class OrangeSenegalMiniStatementRequest:Request
    {
        public string OperatorCode { get; set; }
        public string RequestToken { get; set; }
        public string RequestType { get; set; }
        public string AffiliateCode { get; set; }
        public string AccountNo { get; set; }
        public string AccountAlias { get; set; }
        public string Reason { get; set; }
    }
}
