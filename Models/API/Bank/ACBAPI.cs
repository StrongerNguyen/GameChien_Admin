using FT_Admin.Models.ACB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace FT_Admin.Models.API
{
    public static class ACBAPI
    {
        private readonly static HttpClient client = new HttpClient();
        private readonly static string server = ConfigurationManager.AppSettings["ACBServer"];
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="accountnumber"></param>
        /// <param name="fromTime">01/03/2022 00:00:00</param>
        /// <param name="toTime">01/03/2022 00:00:00</param>
        /// <returns>
        /// {
        ///     "success": true,
        ///     "message": "Success",
        ///     "took": 18,
        ///     "transactions": [],
        ///     "total": 0,
        ///     "page": 1,
        ///     "size": 100
        /// }
        /// </returns>
        public static async Task<ACBTransactionsModel> GetHistoryTransactions(string username, string password, string accountnumber, string fromTime, string toTime)
        {
            ACBTransactionsModel aCBTransactions = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/acb/transactions", new
                {
                    username = username,
                    password = password,
                    accountNumber = accountnumber,
                    begin = fromTime,
                    end = toTime
                });
                content = await request.Content.ReadAsStringAsync();
                aCBTransactions = new JavaScriptSerializer().Deserialize<ACBTransactionsModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("ACBAPI/GetHistoryTransactions", ex, content);
            }
            return aCBTransactions;
        }
        public static async Task<ACBBankCodeModel> GetBankCode(string username, string password)
        {
            ACBBankCodeModel aCBBankCode = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/acb/getBankCode", new
                {
                    username = username,
                    password = password
                });
                content = await request.Content.ReadAsStringAsync();
                aCBBankCode = new JavaScriptSerializer().Deserialize<ACBBankCodeModel>(content);

            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("ACBAPI/GetBankCode", ex, content);
            }

            return aCBBankCode;
        }
        public static async Task<ACBTransferModel> CreateTransferInBank(string username, string password, string accountNumber, string tranfer_to, int amount, string message, string otp_type = "SMS")
        {
            ACBTransferModel aCBTransfer = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/acb/tranfer_local", new
                {
                    username = username,
                    password = password,
                    accountNumber = accountNumber,
                    tranfer_to = tranfer_to,
                    amount = amount,
                    message = message,
                    otp_type = otp_type
                });
                content = await request.Content.ReadAsStringAsync();
                aCBTransfer = new JavaScriptSerializer().Deserialize<ACBTransferModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("ACBAPI/CreateTransferInBank", ex, content);
            }
            return aCBTransfer;
        }
        public static async Task<ACBTransferModel> CreateTransfer247(string username, string password, string accountNumber, string napasBankCode, string tranfer_to, int amount, string message, string otp_type = "SMS")
        {
            ACBTransferModel aCBTransfer = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/acb/tranfer_247", new
                {
                    username = username,
                    password = password,
                    accountNumber = accountNumber,
                    napasBankCode = napasBankCode,
                    tranfer_to = tranfer_to,
                    amount = amount,
                    message = message,
                    otp_type = otp_type
                });
                content = await request.Content.ReadAsStringAsync();
                aCBTransfer = new JavaScriptSerializer().Deserialize<ACBTransferModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("ACBAPI/CreateTransfer247", ex, content);
            }
            return aCBTransfer;
        }
        public static async Task<ACBConfirmOTPModel> ConfirmTransfer(string username, string password, string accountNumber, string uuid, string code)
        {
            ACBConfirmOTPModel aCBConfirmOTP = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/acb/confirm_tranfer", new
                {
                    username = username,
                    password = password,
                    accountNumber = accountNumber,
                    uuid = uuid,
                    code = code
                });
                content = await request.Content.ReadAsStringAsync();
                aCBConfirmOTP = new JavaScriptSerializer().Deserialize<ACBConfirmOTPModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("ACBAPI/ConfirmTransfer", ex, content);
            }
            return aCBConfirmOTP;
        }
    }
}