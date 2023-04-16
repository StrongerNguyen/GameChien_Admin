using FT_Admin.Models.Data;
using FT_Admin.Models.Vietinbank;
using FT_Admin.Models.Vietinbank.CreateTransferInBank;
using FT_Admin.Models.Vietinbank.Login;
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
    public static class VietinbankAPI
    {
        private readonly static HttpClient client = new HttpClient();
        private readonly static string server = ConfigurationManager.AppSettings["VietinbankAPIServer"];
        private readonly static Dictionary<string, string> API = new Dictionary<string, string>
        {
            {"login",server+"/vtbapi/login" },
            {"getCustomerDetails",server+"/vtbapi/getCustomerDetails" },
            {"getEntitiesAndAccounts",server+"/vtbapi/getEntitiesAndAccounts" },
            {"getHistTransactions",server+"/vtbapi/getHistTransactions" },
            {"getBankList",server+"/vtbapi/getBankList" },
            {"getAccountDetailInBank",server+"/vtbapi/getAccountDetailInBank" },
            {"createTransferInBank",server+"/vtbapi/createTransferInBank" },
            {"getAccountDetailOutBank",server+"/vtbapi/getAccountDetailOutBank" },
            {"createTransferOutBank",server+"/vtbapi/createTransferOutBank" },
            {"confirmOTPTransferSoftOTP",server+"/vtbapi/confirmOTPTransferSoftOTP" },
            {"getResultTransferSoftOTP",server+"/vtbapi/getResultTransferSoftOTP" }
        };
        public static async Task<VietinbankLoginModel> Login(string userName, string passWord)
        {
            VietinbankLoginModel vietinbankLogin = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync(API["login"], new { userName = userName, passWord = passWord });
                content = await request.Content.ReadAsStringAsync();
                vietinbankLogin = new JavaScriptSerializer().Deserialize<VietinbankLoginModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietinbankAPI/Login", ex, content);
            }
            return vietinbankLogin;
        }
        public static async Task<VietinbankCustomerDetailModel> getCustomerDetails(string sessionId)
        {
            VietinbankCustomerDetailModel vietinbankCustomerDetail = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync(API["getCustomerDetails"], new { sessionId = sessionId });
                content = await request.Content.ReadAsStringAsync();
                vietinbankCustomerDetail = new JavaScriptSerializer().Deserialize<VietinbankCustomerDetailModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietinbankAPI/getCustomerDetails", ex, content);
            }
            return vietinbankCustomerDetail;
        }
        public static async Task<VietinbankEntitiesAndAccountModel> getEntitiesAndAccounts(string sessionId)
        {
            VietinbankEntitiesAndAccountModel vietinbankEntitiesAndAccount = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync(API["getEntitiesAndAccounts"], new { sessionId = sessionId });
                content = await request.Content.ReadAsStringAsync();
                vietinbankEntitiesAndAccount = new JavaScriptSerializer().Deserialize<VietinbankEntitiesAndAccountModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietinbankAPI/getEntitiesAndAccounts", ex, content);
            }
            return vietinbankEntitiesAndAccount;
        }
        public static async Task<VietinbankTransactionModel> getHistTransactions(string sessionId, string accountNumber, DateTime startDate, DateTime endDate, int pageNumber = 0)
        {
            VietinbankTransactionModel vietinbankTransaction = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync(API["getHistTransactions"], new { sessionId = sessionId, accountNumber = accountNumber, startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd"), pageNumber = pageNumber });
                content = await request.Content.ReadAsStringAsync();

                vietinbankTransaction = new JavaScriptSerializer().Deserialize<VietinbankTransactionModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietinbankAPI/getHistoryTransactions", ex, content);
            }
            return vietinbankTransaction;
        }
        public static async Task<VietinbankBankListModel> getBankList(string sessionId)
        {
            VietinbankBankListModel vietinbankBankList = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync(API["getBankList"], new { sessionId = sessionId });
                content = await request.Content.ReadAsStringAsync();
                vietinbankBankList = new JavaScriptSerializer().Deserialize<VietinbankBankListModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietinbankAPI/getCodeMapping", ex, content);
            }
            return vietinbankBankList;
        }
        public static async Task<VietinbankAccountInfoInBankModel> getAccountDetailInBank(string sessionId, string beneficiaryAccount)
        {
            VietinbankAccountInfoInBankModel vietinbankAccountInfoInBank = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync(API["getAccountDetailInBank"], new { sessionId = sessionId, beneficiaryAccount = beneficiaryAccount });
                content = await request.Content.ReadAsStringAsync();
                vietinbankAccountInfoInBank = new JavaScriptSerializer().Deserialize<VietinbankAccountInfoInBankModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietinbankAPI/getAccountInfoInBank", ex, content);
            }
            return vietinbankAccountInfoInBank;
        }
        public static async Task<VietinbankCreateTransferInBankModel> createTransferInBank(string sessionId, string accountNumber, string accountType, string bsb, string currencyCode, string toAccountNumber, int amount, string note)
        {
            VietinbankCreateTransferInBankModel vietinbankCreateTransferInBank = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync(API["createTransferInBank"], new { sessionId = sessionId, accountNumber = accountNumber, accountType = accountType, bsb = bsb, currencyCode = currencyCode, toAccountNumber = toAccountNumber, amount = amount, message = note });
                content = await request.Content.ReadAsStringAsync();
                vietinbankCreateTransferInBank = new JavaScriptSerializer().Deserialize<VietinbankCreateTransferInBankModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietinbankAPI/createTransferInBank", ex, content);
            }
            return vietinbankCreateTransferInBank;
        }
        public static async Task<VietinbankAccountInfoOutBankModel> getAccountDetailOutBank(string sessionId, string accountNumber, string accountnumberRecive, string bankcode)
        {
            VietinbankAccountInfoOutBankModel vietinbankAccountInfoOutBank = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync(API["getAccountDetailOutBank"], new { sessionId = sessionId, accountNumber = accountNumber, beneficiaryAccount = accountnumberRecive, beneficiaryBin = bankcode });
                content = await request.Content.ReadAsStringAsync();
                vietinbankAccountInfoOutBank = new JavaScriptSerializer().Deserialize<VietinbankAccountInfoOutBankModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietinbankAPI/getAccountDetailOutBank", ex, content);
            }
            return vietinbankAccountInfoOutBank;
        }
        public static async Task<VietinbankCreateTransferOutBankModel> createTransferOutBank(string sessionId, string accountNumber, int amount, string note)
        {
            VietinbankCreateTransferOutBankModel vietinbankCreateTransferOutBank = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync(API["createTransferOutBank"], new { sessionId = sessionId, accountNumber = accountNumber, amount = amount, message = note });
                content = await request.Content.ReadAsStringAsync();
                vietinbankCreateTransferOutBank = new JavaScriptSerializer().Deserialize<VietinbankCreateTransferOutBankModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietinbankAPI/createTransferOutBank", ex, content);
            }
            return vietinbankCreateTransferOutBank;
        }
        public static async Task<VietinbankConfirmSoftOTPTransferModel> confirmOTPTransferSoftOTP(string sessionId, string accountNumber, string authenticationActionCode)
        {
            VietinbankConfirmSoftOTPTransferModel vietinbankConfirmSoftOTPTransfer = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync(API["confirmOTPTransferSoftOTP"], new { sessionId = sessionId, accountNumber = accountNumber, authenticationActionCode = authenticationActionCode });
                content = await request.Content.ReadAsStringAsync();
                vietinbankConfirmSoftOTPTransfer = new JavaScriptSerializer().Deserialize<VietinbankConfirmSoftOTPTransferModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietinbankAPI/confirmOTPTransferSoftOTP", ex, content);
            }
            return vietinbankConfirmSoftOTPTransfer;
        }
        public static async Task<VietinbankResultTransferModel> getResultTransferSoftOTP(string sessionId, string accountNumber, string authenticationActionCode)
        {
            VietinbankResultTransferModel vietinbankResultTransfer = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync(API["getResultTransferSoftOTP"], new { sessionId = sessionId });
                content = await request.Content.ReadAsStringAsync();
                vietinbankResultTransfer = new JavaScriptSerializer().Deserialize<VietinbankResultTransferModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietinbankAPI/getResultTransferSoftOTP", ex, content);
            }
            return vietinbankResultTransfer;
        }
    }
}