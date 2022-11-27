using BankWalletIntegrator.Interfaces;
using BankWalletIntegrator.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Repository
{
    public class MiddlewareRepo:IMiddlewareRepo
    {
        private readonly IConfiguration _configuration;
        public MiddlewareRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<DataTable> GetAllowedIPAddresses(string appId, string appKey)
        {
            DataTable dt = new DataTable();
            try
            {
                using(SqlConnection sqlConn = new SqlConnection(_configuration.GetSection("BankToWalletConnection").Value))
                {
                    if(sqlConn.State != ConnectionState.Open)
                    {
                        sqlConn.Open();
                    }
                    using(SqlCommand cmd = new SqlCommand("sp_GetAllowedIpAddresses", sqlConn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@appId", SqlDbType.VarChar,20).Value = appId;
                        cmd.Parameters.Add("@appKey", SqlDbType.VarChar, 50).Value = appKey;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }
            }
            catch(Exception ex)
            {

            }            
            return dt;
        }

        public async Task<string> GetRequestIdStatus(string requestId)
        {
            string response = string.Empty;
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(_configuration.GetSection("BankToWalletConnection").Value))
                {
                    if (sqlConn.State != ConnectionState.Open)
                    {
                        sqlConn.Open();
                    }
                    SqlCommand sqlcmd = new SqlCommand("sp_GetRequestResponseEntries", sqlConn);
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Add("@requestId", SqlDbType.VarChar).Value = requestId;
                    sqlcmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
                    sqlcmd.ExecuteNonQuery();
                    response = sqlcmd.Parameters["@output"].Value.ToString();                    
                }
                
            }
            catch (Exception ex)
            {
                
            }            
            return response;       
            
        }

        public async Task InsertRequestResponseEntries(RequestResponseEntry entry)
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(_configuration.GetSection("BankToWalletConnection").Value))
                {
                    if (sqlConn.State != ConnectionState.Open)
                    {
                        sqlConn.Open();
                    }
                    using (SqlCommand cmd = new SqlCommand("sp_insertRequestResponse", sqlConn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@requestId", SqlDbType.VarChar).Value = entry.requestId;
                        cmd.Parameters.Add("@requestContentType", SqlDbType.VarChar).Value = entry.requestContentType;
                        cmd.Parameters.Add("@requestIpAddress", SqlDbType.VarChar).Value = entry.requestIpAddress;
                        cmd.Parameters.Add("@requestMethod", SqlDbType.VarChar).Value = entry.requestMethod;
                        cmd.Parameters.Add("@requestHeaders", SqlDbType.VarChar).Value = entry.requestHeaders;
                        cmd.Parameters.Add("@requestTimestamp", SqlDbType.DateTime).Value = entry.requestTimestamp;
                        cmd.Parameters.Add("@requestUri", SqlDbType.VarChar).Value = entry.requestUri;
                        cmd.Parameters.Add("@queryString", SqlDbType.VarChar).Value = entry.queryString;
                        cmd.Parameters.Add("@responseContentBody", SqlDbType.VarChar).Value = entry.responseContentBody;
                        cmd.Parameters.Add("@responseContentType", SqlDbType.VarChar).Value = entry.responseContentType;
                        cmd.Parameters.Add("@responseHeaders", SqlDbType.VarChar).Value = entry.responseHeaders;
                        cmd.Parameters.Add("@responseTimestamp", SqlDbType.DateTime).Value = entry.responseTimestamp;
                        cmd.Parameters.Add("@responseStatusCode", SqlDbType.VarChar).Value = entry.responseStatusCode;
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
