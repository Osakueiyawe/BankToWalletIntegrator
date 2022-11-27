using BankWalletIntegrator.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Repository
{
    public class BankToWalletMerchants:IBankToWalletMerchants
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public BankToWalletMerchants(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<DataTable> GetMerchantDetails(long merchantId)
        {
            DataTable dt = new DataTable();
            try
            {
                _logger.Information($"About to get merchant details for merchant id: {merchantId}");
                using (SqlConnection sqlConn = new SqlConnection(_configuration.GetSection("BankToWalletConnection").Value))
                {
                    if (sqlConn.State != ConnectionState.Open)
                    {
                        sqlConn.Open();
                    }
                    using (SqlCommand cmd = new SqlCommand("sp_GetMerchantDetails", sqlConn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@merchantId", SqlDbType.BigInt).Value = merchantId;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                        _logger.Information($"Number of rows retrieved for merchant details is: {dt.Rows.Count}");
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Error getting merchant details for merchant id: {merchantId}");
            }
            return dt;
        }

        public async Task<DataTable> GetMerchantAccountBalanceFees(long merchantId)
        {
            DataTable dt = new DataTable();
            try
            {
                _logger.Information($"About to get merchant account balance fees for merchant id: {merchantId}");
                using (SqlConnection sqlConn = new SqlConnection(_configuration.GetSection("BankToWalletConnection").Value))
                {
                    if(sqlConn.State != ConnectionState.Open) sqlConn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_GetAccountBalanceFees", sqlConn))
                    {
                        cmd.CommandType= CommandType.StoredProcedure;
                        cmd.Parameters.Add("@merchantId", SqlDbType.BigInt).Value = merchantId;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                        _logger.Information($"Number of rows retrieved for merchant account balance fees is: {dt.Rows.Count}");
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Error getting merchant account balance fees for merchant id: {merchantId}");
            }
            return dt;
        }

        public async Task<DataTable> GetMerchantMiniStatementFees(long merchantId)
        {
            DataTable dt = new DataTable();
            try
            {
                _logger.Information($"About to get merchant ministatement fees for merchant id: {merchantId}");
                using (SqlConnection sqlConn = new SqlConnection(_configuration.GetSection("BankToWalletConnection").Value))
                {
                    if (sqlConn.State != ConnectionState.Open) sqlConn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_GetMiniStatementFees", sqlConn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@merchantId", SqlDbType.BigInt).Value = merchantId;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                        _logger.Information($"Number of rows retrieved for merchant ministatement fees is: {dt.Rows.Count}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting merchant mini statement fees for merchant id: {merchantId}");
            }
            return dt;
        }

        public async Task<DataTable> GetMerchantBankToWalletFees(long merchantId)
        {
            DataTable dt = new DataTable();
            try
            {
                _logger.Information($"About to get merchant bank to wallet fees for merchant id: {merchantId}");
                using (SqlConnection sqlConn = new SqlConnection(_configuration.GetSection("BankToWalletConnection").Value))
                {
                    if(sqlConn.State != ConnectionState.Open)
                    {
                        sqlConn.Open();
                    }
                    using(SqlCommand cmd = new SqlCommand("sp_GetBankToWalletFees", sqlConn))
                    {
                        cmd.CommandType= CommandType.StoredProcedure;
                        cmd.Parameters.Add("@merchantId", SqlDbType.BigInt).Value = merchantId;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                        _logger.Information($"Number of rows retrieved for merchant bank to wallet fees is: {dt.Rows.Count}");
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Error getting merchant bank to wallet fees for merchant id: {merchantId}");
            }
            return dt;
        }

        public async Task<DataTable> GetMerchantWalletToBankFees(long merchantId)
        {
            DataTable dt = new DataTable();
            try
            {
                _logger.Information($"About to get merchant wallet to bank fees for merchant id: {merchantId}");
                using (SqlConnection sqlConn = new SqlConnection(_configuration.GetSection("BankToWalletConnection").Value))
                {
                    if (sqlConn.State != ConnectionState.Open)
                    {
                        sqlConn.Open();
                    }
                    using (SqlCommand cmd = new SqlCommand("sp_GetWalletToBankFees", sqlConn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@merchantId", SqlDbType.BigInt).Value = merchantId;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                        _logger.Information($"Number of rows retrieved for merchant bank to wallet fees is: {dt.Rows.Count}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting merchant wallet to bank fees for merchant id: {merchantId}");
            }
            return dt;
        }
    }
}
