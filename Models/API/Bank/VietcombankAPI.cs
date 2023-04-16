using FT_Admin.Models.Vietcombank;
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
    public static class VietcombankAPI
    {
        private readonly static HttpClient client = new HttpClient();
        private readonly static string server = ConfigurationManager.AppSettings["VCBServer"];

        public static async Task<VCBTransactionResultModel> getHistoryTransactions(string userName, string passWord, DateTime fromDate, DateTime toDate)
        {
            VCBTransactionResultModel vCBTransactionResult = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/vcb/transactions", new { username = userName, password = passWord, accountNumber = "", begin = toDate.ToString("dd/MM/yyyy"), end = fromDate.ToString("dd/MM/yyyy") });
                content = await request.Content.ReadAsStringAsync();
                vCBTransactionResult = new JavaScriptSerializer().Deserialize<VCBTransactionResultModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietcombankAPI/getHistoryTransactions", ex, content);
            }
            return vCBTransactionResult;
        }
        public static async Task<VietcombankBankListModel> getBankList()
        {
            VietcombankBankListModel vietcombankBankList = null;
            var content = "";
            try
            {
                var request = await client.GetAsync($"{server}/api/vcb/bankcode_tranfer247");
                content = await request.Content.ReadAsStringAsync();
                vietcombankBankList = new JavaScriptSerializer().Deserialize<VietcombankBankListModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietcombankAPI/getBankList", ex, content);
            }
            return vietcombankBankList;
        }
        public static async Task<VietcombankOTPModel> getOTP(string userName, string passWord, string bankCode, string stkNhan, string money, string note)
        {
            VietcombankOTPModel vietcombankOTP = null;
            var content = "";
            try
            {
                if (string.IsNullOrEmpty(bankCode))
                {
                    //trong VCB
                    var request = await client.PostAsJsonAsync($"{server}/api/vcb/tranfer_local", new { username = userName, password = passWord, tranfer_to = stkNhan, amount = money, content = note, feeType = 1, accountNumber = "" });
                    content = await request.Content.ReadAsStringAsync();
                    vietcombankOTP = new JavaScriptSerializer().Deserialize<VietcombankOTPModel>(content);
                }
                else
                {
                    //ngoài VCB
                    var request = await client.PostAsJsonAsync($"{server}/api/vcb/tranfer247", new { username = userName, password = passWord, tranfer_to = stkNhan, bank_code = bankCode, amount = money, content = note, feeType = 1, accountNumber = "" });
                    content = await request.Content.ReadAsStringAsync();
                    vietcombankOTP = new JavaScriptSerializer().Deserialize<VietcombankOTPModel>(content);
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietcombankAPI/getOTP", ex, content);
            }
            return vietcombankOTP;
        }
        public static async Task<VietcombankConfirmOTPModel> confirmOTP(string userName, string passWord, string tranId, string otp)
        {
            VietcombankConfirmOTPModel vietcombankConfirmOTP = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/vcb/confirm_tranfer", new { username = userName, password = passWord, tranId = tranId, otp = otp });
                content = await request.Content.ReadAsStringAsync();
                vietcombankConfirmOTP = new JavaScriptSerializer().Deserialize<VietcombankConfirmOTPModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("VietcombankAPI/confirmOTP", ex, content);
            }
            return vietcombankConfirmOTP;
        }
    }
}