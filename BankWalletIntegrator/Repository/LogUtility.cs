using BankWalletIntegrator.Interfaces;
using BankWalletIntegrator.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Repository
{
    public class LogUtility:ILogUtility
    {
        private readonly IConfiguration _config;
        public LogUtility(IConfiguration config)
        {
            _config = config;
        }
        public async Task LogBankToWallet(BankToWalletLog logDetails)
        {            
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetSection("BankToWalletConnection").Value))
                {
                    if(conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    using(SqlCommand cmd = new SqlCommand("sp_InsertIntoBankToWalletLogs", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@requestId", SqlDbType.VarChar, 50).Value = logDetails.requestId;
                        cmd.Parameters.Add("@countryId", SqlDbType.VarChar, 10).Value = logDetails.countryId;
                        cmd.Parameters.Add("@externalRefNo", SqlDbType.VarChar, 50).Value = logDetails.externalRefNo;
                        cmd.Parameters.Add("@accountNo", SqlDbType.VarChar, 50).Value = logDetails.accountNo;
                        cmd.Parameters.Add("@finnacleTransRef", SqlDbType.VarChar, int.MaxValue).Value = logDetails.finnacleTransRef;
                        cmd.Parameters.Add("@responseCode", SqlDbType.VarChar, 10).Value = logDetails.responseCode;
                        cmd.Parameters.Add("@responseMessage", SqlDbType.VarChar, 200).Value = logDetails.responseMessage;
                        cmd.Parameters.Add("@transactionTime", SqlDbType.DateTime).Value = logDetails.transactionTime;
                        cmd.ExecuteNonQuery();                        
                    }
                }
            }
            catch(Exception ex)
            {

            }            
        }

        public async Task LogWalletToBank(WalletToBankLog logDetails)
        {            
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetSection("BankToWalletConnection").Value))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    using (SqlCommand cmd = new SqlCommand("sp_InsertIntoWalletToBankLogs", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@requestId", SqlDbType.VarChar, 50).Value = logDetails.requestId;
                        cmd.Parameters.Add("@countryId", SqlDbType.VarChar, 10).Value = logDetails.countryId;
                        cmd.Parameters.Add("@externalRefNo", SqlDbType.VarChar, 50).Value = logDetails.externalRefNo;
                        cmd.Parameters.Add("@accountNo", SqlDbType.VarChar, 50).Value = logDetails.accountNo;
                        cmd.Parameters.Add("@finnacleTransRef", SqlDbType.VarChar, int.MaxValue).Value = logDetails.finnacleTransRef;
                        cmd.Parameters.Add("@responseCode", SqlDbType.VarChar, 10).Value = logDetails.responseCode;
                        cmd.Parameters.Add("@responseMessage", SqlDbType.VarChar, 200).Value = logDetails.responseMessage;
                        cmd.Parameters.Add("@transactionTime", SqlDbType.DateTime).Value = logDetails.transactionTime;
                        cmd.ExecuteNonQuery();                        
                    }
                }
            }
            catch (Exception ex)
            {

            }            
        }
    }
}
