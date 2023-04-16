using FT_Admin.Models.Techcombank;
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
    public static class TechcombankAPI
    {
        private readonly static HttpClient client = new HttpClient();
        private readonly static string server = ConfigurationManager.AppSettings["TechcombankServer"];
        public static async Task<TechcombankLoginModel> Login(string userName, string passWord)
        {
            TechcombankLoginModel techcombankLogin = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/login.php", new { username = userName, password = passWord });
                content = await request.Content.ReadAsStringAsync();
                techcombankLogin = new JavaScriptSerializer().Deserialize<TechcombankLoginModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TechcombankAPI/Login", ex, content);
            }
            return techcombankLogin;
        }

        public static async Task<TechcombankWalletsModel> getWallet(string userName)
        {
            TechcombankWalletsModel techcombankWallets = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/getWallet.php", new { username = userName });
                content = await request.Content.ReadAsStringAsync();
                techcombankWallets = new JavaScriptSerializer().Deserialize<TechcombankWalletsModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TechcombankAPI/getWallet", ex, content);
            }
            return techcombankWallets;
        }
        public static async Task<TechcombankTransactionModel> getHistoryTransactions(string userName, string accountNumber, string fromDate, string toDate)
        {
            TechcombankTransactionModel techcombankTransaction = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/getTransactionHistory.php", new { username = userName, isMobile = "0", accountNumber = accountNumber, fromDate = fromDate, toDate = toDate });
                content = await request.Content.ReadAsStringAsync();
                techcombankTransaction = new JavaScriptSerializer().Deserialize<TechcombankTransactionModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TechcombankAPI/getHistoryTransactions", ex, content);
            }
            return techcombankTransaction;
        }
        public static async Task<TechcombankBankListModel> getBankList(string userName)
        {
            TechcombankBankListModel techcombankBankList = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/getBankList.php", new { username = userName });
                content = await request.Content.ReadAsStringAsync();
                techcombankBankList = new JavaScriptSerializer().Deserialize<TechcombankBankListModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TechcombankAPI/getBankList", ex, content);
            }
            return techcombankBankList;
        }
        public static async Task<TechcombankOTPModel> getOTP(string userName, string accountNumber, string bankId, string stkNhan, int money, string note)
        {
            TechcombankOTPModel techcombankOTP = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/getOTP.php", new { username = userName, isMobile = "0", accountNumber = accountNumber, bankId = bankId, stkNhan = stkNhan, money = money, note = note });
                content = await request.Content.ReadAsStringAsync();
                techcombankOTP = new JavaScriptSerializer().Deserialize<TechcombankOTPModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TechcombankAPI/getOTP", ex, content);
            }
            return techcombankOTP;
        }
        public static async Task<TeckcombankConfirmOTPModel> confirmOTP(string userName, string otp, string systemid)
        {
            TeckcombankConfirmOTPModel teckcombankConfirmOTP = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/confirmOTP.php", new { username = userName, otp = otp, systemid = systemid });
                content = await request.Content.ReadAsStringAsync();
                teckcombankConfirmOTP = new JavaScriptSerializer().Deserialize<TeckcombankConfirmOTPModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("TechcombankAPI/confirmOTP", ex, content);
            }
            return teckcombankConfirmOTP;
        }
    }
}