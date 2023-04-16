using FT_Admin.Models.Data;
using FT_Admin.Models.TPBank;
using FT_Admin.Models.TPBankBankList;
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
    public static class TPBankAPI
    {
        private readonly static HttpClient client = new HttpClient();
        private readonly static string server = ConfigurationManager.AppSettings["TPBankServer"];
        public static async Task<TPBankLoginModel> Login(string userName, string passWord)
        {
            TPBankLoginModel tPBankLogin = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/login.php", new { username = userName, password = passWord });
                content = await request.Content.ReadAsStringAsync();
                tPBankLogin = new JavaScriptSerializer().Deserialize<TPBankLoginModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TPBankAPI/Login", ex, content);
            }
            return tPBankLogin;

        }
        public static async Task<TPBankDetailModel> getDetail(string token, string accountNumber)
        {
            TPBankDetailModel tPBankDetail = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/getDetails.php", new { token = token, accountnumber = accountNumber });
                content = await request.Content.ReadAsStringAsync();
                tPBankDetail = new JavaScriptSerializer().Deserialize<TPBankDetailModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TPBankAPI/getDetail", ex, content);
            }
            return tPBankDetail;
        }
        public static async Task<TPBankTransactionModel> getHistoryTransactions(string token, string accountNumber, DateTime fromDate, DateTime toDate)
        {
            TPBankTransactionModel tPBankTransaction = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/getHistoryTransactions.php", new { token = token, accountnumber = accountNumber, fromdate = fromDate.ToString("yyyyMMdd"), todate = toDate.ToString("yyyyMMdd") });
                content = await request.Content.ReadAsStringAsync();
                tPBankTransaction = new JavaScriptSerializer().Deserialize<TPBankTransactionModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TPBankAPI/getHistoryTransactions", ex, content);
            }
            return tPBankTransaction;
        }
        public static async Task<TPBankBankListModel> getBankList(string token)
        {
            TPBankBankListModel tPBankBankList = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/getListBank.php", new { token = token });
                content = await request.Content.ReadAsStringAsync();
                tPBankBankList = new JavaScriptSerializer().Deserialize<TPBankBankListModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TPBankAPI/getBankList", ex, content);
            }
            return tPBankBankList;
        }
        public static async Task<TPBankAccountBankInfoModel> getAccountBankInfo(string token, string accountNumber, string stkNhan, string bankId)
        {
            TPBankAccountBankInfoModel tPBankAccountBankInfo = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/getAccountBankInfo.php", new { token = token, accountnumber = accountNumber, accountto = stkNhan, bankid = bankId });
                content = await request.Content.ReadAsStringAsync();
                tPBankAccountBankInfo = new JavaScriptSerializer().Deserialize<TPBankAccountBankInfoModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TPBankAPI/getAccountBankInfo", ex, content);
            }
            return tPBankAccountBankInfo;
        }
        public static async Task<TPBankGetOTPModel> getOTP(string token, string accountNumber, string stkNhan, string bankId, string money, string note, string creditorInfo)
        {
            TPBankGetOTPModel tPBankGetOTP = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/getOTP.php", new { token = token, accountnumber = accountNumber, accountto = stkNhan, bankid = bankId, amount = money, note = note, creditorinfo = creditorInfo });
                content = await request.Content.ReadAsStringAsync();
                tPBankGetOTP = new JavaScriptSerializer().Deserialize<TPBankGetOTPModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TPBankAPI/getOTP", ex, content);
            }
            return tPBankGetOTP;
        }
        public static async Task<TPBankConfirmOTPModel> confirmOTP(string token, string accountNumber, string id, string otp)
        {
            TPBankConfirmOTPModel tPBankConfirmOTP = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/confirmOTP.php", new { token = token, accountnumber = accountNumber, id = id, otp = otp });
                content = await request.Content.ReadAsStringAsync();
                tPBankConfirmOTP = new JavaScriptSerializer().Deserialize<TPBankConfirmOTPModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TPBankAPI/confirmOTP", ex, content);
            }
            return tPBankConfirmOTP;
        }
    }
}