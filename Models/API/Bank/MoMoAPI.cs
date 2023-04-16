using FT_Admin.Models.Data;
using FT_Admin.Models.MoMo;
using FT_Admin.Models.MoMo.BankList;
using FT_Admin.Models.MoMo.ConfirmOTP;
using FT_Admin.Models.MoMo.ConfirmTransferToMoMo;
using FT_Admin.Models.MoMo.CreateTransferToBank;
using FT_Admin.Models.MoMo.MoMoConfirmTransferToBank;
using FT_Admin.Models.MoMo.MoMoTransaction;
using FT_Admin.Models.MoMo.TransferToMoMo;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace FT_Admin.Models.API
{
    public static class MoMoAPI
    {
        private readonly static HttpClient client = new HttpClient();
        private readonly static string server = ConfigurationManager.AppSettings["MoMoServer"];
        public static async Task<MoMoGetOTPModel> getOTP(string username, string password)
        {
            MoMoGetOTPModel moMoGetOTP = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/getOTP.php", new { username = username, password = password });
                content = await request.Content.ReadAsStringAsync();
                moMoGetOTP = new JavaScriptSerializer().Deserialize<MoMoGetOTPModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("MoMoAPI/getOTP", ex, content);
            }
            return moMoGetOTP;
        }
        public static async Task<MoMoLoginModel> ConfirmOTPasLogin(string username, string password, string otp, string rkey, string onesignal, string imei)
        {
            MoMoLoginModel moMoLogin = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/confirmOTPasLogin.php", new
                {
                    username = username,
                    password = password,
                    otp = otp,
                    rkey = rkey,
                    onesignal = onesignal,
                    imei = imei
                });
                content = await request.Content.ReadAsStringAsync();
                moMoLogin = new JavaScriptSerializer().Deserialize<MoMoLoginModel>(content);
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("MoMoAPI/confirmOTPasLogin", ex, content);
            }
            return moMoLogin;
        }
        public static async Task<bool> reTryLogin(tblBankAccount bankAccount)
        {

            MoMoLoginModel moMoLogin = null;
            var content = "";
            try
            {
                var request = await client.PostAsJsonAsync($"{server}/api/reLogin.php", new DataMoMo
                {
                    username = bankAccount.UserName,
                    password = bankAccount.Password,
                    requestkey = bankAccount.requestkey,
                    auth_token = bankAccount.auth_token,
                    imei = bankAccount.imei,
                    ohash = bankAccount.ohash,
                    onesignal = bankAccount.onesignal,
                    otp = bankAccount.OTP,
                    rkey = bankAccount.rkey,
                    setupkey = bankAccount.setupkey
                });
                content = await request.Content.ReadAsStringAsync();
                moMoLogin = new JavaScriptSerializer().Deserialize<MoMoLoginModel>(content);
                if (moMoLogin.result)
                {
                    using (var db = new BankAPIEntities())
                    {
                        var momo = db.tblBankAccounts.FirstOrDefault(t => t.Id == bankAccount.Id);
                        if (momo != null)
                        {
                            //update authen
                            momo.rkey = moMoLogin.ext.rkey;
                            momo.onesignal = moMoLogin.ext.onesignal;
                            momo.imei = moMoLogin.ext.imei;
                            momo.ohash = moMoLogin.ext.ohash;
                            momo.setupkey = moMoLogin.ext.setupkey;
                            momo.requestkey = moMoLogin.ext.requestkey;
                            momo.auth_token = moMoLogin.ext.auth_token;
                            db.SaveChanges();
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("MoMoAPI/reLogin", ex, content);
            }
            return false;
        }

        public static async Task<MoMoTransactionsModel> getHistoryTransactions(tblBankAccount bankAccount, int hours)
        {
            MoMoTransactionsModel moMoTransactions = null;
            var content = "";
            try
            {
                //recall:
                var request = await client.PostAsJsonAsync($"{server}/api/getHistoryTransactions.php", new
                {
                    bankAccount = new DataMoMo
                    {
                        username = bankAccount.UserName,
                        password = bankAccount.Password,
                        requestkey = bankAccount.requestkey,
                        auth_token = bankAccount.auth_token,
                        imei = bankAccount.imei,
                        ohash = bankAccount.ohash,
                        onesignal = bankAccount.onesignal,
                        otp = bankAccount.OTP,
                        rkey = bankAccount.rkey,
                        setupkey = bankAccount.setupkey
                    },
                    hours = hours
                });
                content = await request.Content.ReadAsStringAsync();
                moMoTransactions = new JavaScriptSerializer().Deserialize<MoMoTransactionsModel>(content);
                //if (content.Equals("401") || content.Equals("0"))
                //{
                //    //relogin
                //    await reTryLogin(bankAccount);
                //}
                //else
                //{

                //    if (moMoTransactions.msgType.Equals("USER_LOGIN_MSG"))
                //    {
                //        //Hết session -> cần đăng nhập lại
                //    }
                //    else
                //    {

                //    }
                //    //if (!moMoTransactions.result && (moMoTransactions.errorCode == -83 || moMoTransactions.errorCode == -314))
                //    //{
                //    //    //relogin
                //    //    //if (await reLogin(bankAccount)) goto recall;
                //    //}
                //}
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("MoMoAPI/getHistoryTransactions", ex, content);
            }
            return moMoTransactions;
        }
        public static async Task<TransferToMoMoModel> createTransferToMoMo(tblBankAccount bankAccount, string phone, int amount, string note)
        {
            TransferToMoMoModel transferToMoMo = null;
            var content = "";
            try
            {
                //recall:
                var request = await client.PostAsJsonAsync($"{server}/api/createTransferToMoMo.php", new
                {
                    bankAccount = new DataMoMo
                    {
                        username = bankAccount.UserName,
                        password = bankAccount.Password,
                        requestkey = bankAccount.requestkey,
                        auth_token = bankAccount.auth_token,
                        imei = bankAccount.imei,
                        ohash = bankAccount.ohash,
                        onesignal = bankAccount.onesignal,
                        otp = bankAccount.OTP,
                        rkey = bankAccount.rkey,
                        setupkey = bankAccount.setupkey
                    },
                    phone = phone,
                    amount = amount,
                    note = note
                });
                content = await request.Content.ReadAsStringAsync();


                if (content.Equals("401") || content.Equals("0"))
                {
                    //relogin
                    //bool rsReLogin = await reLogin(bankAccount);
                    //if (rsReLogin) goto recall;
                }
                else
                {
                    transferToMoMo = new JavaScriptSerializer().Deserialize<TransferToMoMoModel>(content);
                    if (!transferToMoMo.result && (transferToMoMo.errorCode == -83 || transferToMoMo.errorCode == -314))
                    {
                        //relogin
                        //if (await reLogin(bankAccount)) goto recall;
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("MoMoAPI/createTransferToMoMo", ex, content);
            }
            return transferToMoMo;
        }
        public static async Task<ConfirmTransferToMoMoModel> confirmTransferToMoMo(tblBankAccount bankAccount, string id)
        {
            ConfirmTransferToMoMoModel confirmTransferToMoMo = null;
            var content = "";
            try
            {
                //recall:
                var request = await client.PostAsJsonAsync($"{server}/api/confirmTransferToMoMo.php", new
                {
                    bankAccount = new DataMoMo
                    {
                        username = bankAccount.UserName,
                        password = bankAccount.Password,
                        requestkey = bankAccount.requestkey,
                        auth_token = bankAccount.auth_token,
                        imei = bankAccount.imei,
                        ohash = bankAccount.ohash,
                        onesignal = bankAccount.onesignal,
                        otp = bankAccount.OTP,
                        rkey = bankAccount.rkey,
                        setupkey = bankAccount.setupkey
                    },
                    id = id
                });
                content = await request.Content.ReadAsStringAsync();

                if (content.Equals("401") || content.Equals("0"))
                {
                    //relogin
                    //if (await reLogin(bankAccount)) goto recall;
                }
                else
                {
                    confirmTransferToMoMo = new JavaScriptSerializer().Deserialize<ConfirmTransferToMoMoModel>(content);
                    if (!confirmTransferToMoMo.result && (confirmTransferToMoMo.errorCode == -83 || confirmTransferToMoMo.errorCode == -314))
                    {
                        //relogin
                        //if (await reLogin(bankAccount)) goto recall;
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("MoMoAPI/confirmTransferToMoMo", ex, content);
            }
            return confirmTransferToMoMo;
        }
        public static async Task<MoMoBankListModel> getBankList(tblBankAccount bankAccount)
        {
            MoMoBankListModel moMoBankList = null;
            var content = "";
            try
            {
                //recall:
                var request = await client.PostAsJsonAsync($"{server}/api/getBankList.php", new DataMoMo
                {
                    username = bankAccount.UserName,
                    password = bankAccount.Password,
                    requestkey = bankAccount.requestkey,
                    auth_token = bankAccount.auth_token,
                    imei = bankAccount.imei,
                    ohash = bankAccount.ohash,
                    onesignal = bankAccount.onesignal,
                    otp = bankAccount.OTP,
                    rkey = bankAccount.rkey,
                    setupkey = bankAccount.setupkey
                });
                content = await request.Content.ReadAsStringAsync();

                if (content.Equals("401") || content.Equals("0"))
                {
                    //relogin
                    //if (await reLogin(bankAccount)) goto recall;
                }
                else
                {
                    moMoBankList = new JavaScriptSerializer().Deserialize<MoMoBankListModel>(content);
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("MoMoAPI/getBankList", ex, content);
            }
            return moMoBankList;
        }

        public static async Task<MoMoCreateTransferToBank> createTransferToBank(tblBankAccount bankAccount, string bankObject, string cardNumber, string cardName, int amount, string partnerRef, string note)
        {
            MoMoCreateTransferToBank moMoCreateTransferToBank = null;
            var content = "";
            try
            {
                //recall:
                var request = await client.PostAsJsonAsync($"{server}/api/createTransferToBank.php", new
                {
                    bankAccount = new DataMoMo
                    {
                        username = bankAccount.UserName,
                        password = bankAccount.Password,
                        requestkey = bankAccount.requestkey,
                        auth_token = bankAccount.auth_token,
                        imei = bankAccount.imei,
                        ohash = bankAccount.ohash,
                        onesignal = bankAccount.onesignal,
                        otp = bankAccount.OTP,
                        rkey = bankAccount.rkey,
                        setupkey = bankAccount.setupkey
                    },
                    bankObject = new JavaScriptSerializer().Deserialize<NapasBank>(bankObject),
                    cardNumber = cardNumber,
                    cardName = cardName,
                    amount = amount,
                    partnerRef = partnerRef,
                    comment = note
                });
                content = await request.Content.ReadAsStringAsync();

                if (content.Equals("401") || content.Equals("0"))
                {
                    //relogin
                    //if (await reLogin(bankAccount)) goto recall;
                }
                else
                {
                    moMoCreateTransferToBank = new JavaScriptSerializer().Deserialize<MoMoCreateTransferToBank>(content);
                    if (!moMoCreateTransferToBank.result && (moMoCreateTransferToBank.errorCode == -83 || moMoCreateTransferToBank.errorCode == -314))
                    {
                        //relogin
                        //if (await reLogin(bankAccount)) goto recall;
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("MoMoAPI/createTransferToBank", ex, content);
            }
            return moMoCreateTransferToBank;
        }
        public static async Task<MoMoConfirmTransferToBankModel> confirmTransferToBank(tblBankAccount bankAccount, string id, string bankObject, string cardNumber, string cardName, int amount, string partnerRef, string note, int fee, string extras)
        {
            MoMoConfirmTransferToBankModel moMoConfirmTransferToBank = null;
            var content = "";
            try
            {
                //recall:
                var request = await client.PostAsJsonAsync($"{server}/api/confirmTransferToBank.php", new
                {
                    bankAccount = new DataMoMo
                    {
                        username = bankAccount.UserName,
                        password = bankAccount.Password,
                        requestkey = bankAccount.requestkey,
                        auth_token = bankAccount.auth_token,
                        imei = bankAccount.imei,
                        ohash = bankAccount.ohash,
                        onesignal = bankAccount.onesignal,
                        otp = bankAccount.OTP,
                        rkey = bankAccount.rkey,
                        setupkey = bankAccount.setupkey
                    },
                    id = id,
                    bankObject = new JavaScriptSerializer().Deserialize<NapasBank>(bankObject),
                    cardNumber = cardNumber,
                    cardName = cardName,
                    amount = amount,
                    partnerRef = partnerRef,
                    comment = note,
                    fee = fee,
                    extras = extras
                });
                content = await request.Content.ReadAsStringAsync();

                if (content.Equals("401") || content.Equals("0"))
                {
                    //relogin
                    //if (await reLogin(bankAccount)) goto recall;
                }
                else
                {
                    moMoConfirmTransferToBank = new JavaScriptSerializer().Deserialize<MoMoConfirmTransferToBankModel>(content);
                    if (!moMoConfirmTransferToBank.result && (moMoConfirmTransferToBank.errorCode == -83 || moMoConfirmTransferToBank.errorCode == -314))
                    {
                        //relogin
                        //if (await reLogin(bankAccount)) goto recall;
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("MoMoAPI/confirmTransferToBank", ex, content);
            }
            return moMoConfirmTransferToBank;
        }
    }
}