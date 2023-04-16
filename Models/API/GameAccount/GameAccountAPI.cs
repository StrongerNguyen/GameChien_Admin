using FT_Admin.Models.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace FT_Admin.Models.API.GameAccount
{
    public static class GameAccountAPI
    {
        private static Dictionary<string, string> apis = new Dictionary<string, string>()
        {
            {"createAccount",ConfigurationManager.AppSettings["GameAccountAPIServer"]+"/createAccount?num={0}" },
            {"addPointAccount",ConfigurationManager.AppSettings["GameAccountAPIServer"]+"/depositAccount" },
            {"subPointAccount",ConfigurationManager.AppSettings["GameAccountAPIServer"]+"/withdrawAccount?userID={0}&value={1}&note={2}" },
            {"resetInfoAccount",ConfigurationManager.AppSettings["GameAccountAPIServer"]+"/resetInfo?userID={0}" },
            {"resetAccount",ConfigurationManager.AppSettings["GameAccountAPIServer"]+"/resetAccount?userID={0}" },
            {"resetPasswordAccount",ConfigurationManager.AppSettings["GameAccountAPIServer"]+"/resetPassword?userID={0}" },
            {"getAdminBalance",ConfigurationManager.AppSettings["GameAccountAPIServer"]+"/getAdminBalance" },
            {"getBalance",ConfigurationManager.AppSettings["GameAccountAPIServer"]+"/getBalance?userID={0}" },
            {"checkLock",ConfigurationManager.AppSettings["GameAccountAPIServer"]+"/checkLock?userID={0}" }
        };
        public static async Task<CreateAccountAPIModel> CreateAccountAsync(int number)
        {
            CreateAccountAPIModel accounts = null;
            string result = string.Empty;
            try
            {
                HttpClient client = new HttpClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = await client.GetAsync(string.Format(apis["createAccount"], number));
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    accounts = new JavaScriptSerializer().Deserialize<CreateAccountAPIModel>(result);
                }
                else
                {
                    await Logging.LogToDBAsync("GameAccountAPI/CreateAccountAsync", "StatusCode:" + response.StatusCode + "\n Reason: " + response.ReasonPhrase, $"number={number}");
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("GameAccountAPI/CreateAccount", ex + "\n" + result, $"number={number}");
            }
            return accounts;
        }
        public static async Task<AddPointAccountModel> AddPointAccountAsync(string gameId, double point, decimal moneyOfPoint, string note, string callby)
        {
            AddPointAccountModel addPointAccount = null;
            string result = string.Empty;
            tblLogGameAPI logGameAPI = new tblLogGameAPI()
            {
                Id = Guid.NewGuid(),
                Name = "addPointAccount",
                CreatedBy = callby,
                CreatedTime = DateTime.Now,
                DataRequest = JsonConvert.SerializeObject(new { userID = gameId, value = point, name = moneyOfPoint.ToString("N0"), note = note })
            };
            try
            {
                HttpClient client = new HttpClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = await client.PostAsJsonAsync(apis["addPointAccount"], new { userID = gameId, value = point, name = moneyOfPoint.ToString("N0"), note = note });

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();

                    logGameAPI.isSuccess = true;
                    logGameAPI.DataResult = result;
                    logGameAPI.ResultTime = DateTime.Now;

                    addPointAccount = new JavaScriptSerializer().Deserialize<AddPointAccountModel>(result);
                }
                else
                {
                    logGameAPI.isSuccess = false;
                    logGameAPI.ResultTime = DateTime.Now;

                    await Logging.LogToDBAsync("GameAccountAPI/AddPointAccount", "StatusCode:" + response.StatusCode + "\n Reason: " + response.ReasonPhrase, $"gameId={gameId}&point={point}");
                }
                using (var db = new BankAPIEntities())
                {
                    db.tblLogGameAPIs.Add(logGameAPI);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("GameAccountAPI/AddPointAccount", ex + "\n" + result, $"gameId={gameId}&point={point}");
            }
            return addPointAccount;
        }
        public static async Task<SubPointAccountModel> SubPointAccountAsync(string gameId, double point, string note, string callby)
        {
            SubPointAccountModel subPointAccount = null;
            string result = string.Empty;
            tblLogGameAPI logGameAPI = new tblLogGameAPI()
            {
                Id = Guid.NewGuid(),
                Name = "subPointAccount",
                CreatedBy = callby,
                CreatedTime = DateTime.Now,
                DataRequest = JsonConvert.SerializeObject(new { gameId = gameId, point = point, note = note })
            };
            try
            {
                HttpClient client = new HttpClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = await client.GetAsync(string.Format(apis["subPointAccount"], gameId, point, note));
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();

                    logGameAPI.isSuccess = true;
                    logGameAPI.DataResult = result;
                    logGameAPI.ResultTime = DateTime.Now;

                    subPointAccount = new JavaScriptSerializer().Deserialize<SubPointAccountModel>(result);
                }
                else
                {
                    logGameAPI.isSuccess = false;
                    logGameAPI.ResultTime = DateTime.Now;

                    await Logging.LogToDBAsync("GameAccountAPI/SubPointAccountAsync", "StatusCode:" + response.StatusCode + "\n Reason: " + response.ReasonPhrase, $"gameId={gameId}&point={point}");
                }
                using (var db = new BankAPIEntities())
                {
                    db.tblLogGameAPIs.Add(logGameAPI);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("GameAccountAPI/SubPointAccountAsync", ex + "\n" + result, $"gameId={gameId}&point={point}");
            }
            return subPointAccount;
        }
        public static async Task<ResetAccountInfoModel> ResetAccountInfoAsync(string gameId, string callby)
        {
            ResetAccountInfoModel resetAccountInfo = null;
            string result = string.Empty;
            tblLogGameAPI logGameAPI = new tblLogGameAPI()
            {
                Id = Guid.NewGuid(),
                Name = "resetInfoAccount",
                CreatedBy = callby,
                CreatedTime = DateTime.Now,
                DataRequest = JsonConvert.SerializeObject(new { gameId = gameId })
            };
            try
            {
                HttpClient client = new HttpClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = await client.GetAsync(string.Format(apis["resetInfoAccount"], gameId));
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();

                    logGameAPI.isSuccess = true;
                    logGameAPI.DataResult = result;
                    logGameAPI.ResultTime = DateTime.Now;

                    resetAccountInfo = new JavaScriptSerializer().Deserialize<ResetAccountInfoModel>(result);
                }
                else
                {
                    logGameAPI.isSuccess = false;
                    logGameAPI.ResultTime = DateTime.Now;

                    await Logging.LogToDBAsync("GameAccountAPI/resetInfoAccount", "StatusCode:" + response.StatusCode + "\n Reason: " + response.ReasonPhrase, $"gameId={gameId}");
                }
                using (var db = new BankAPIEntities())
                {
                    db.tblLogGameAPIs.Add(logGameAPI);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("GameAccountAPI/resetInfoAccount", ex + "\n" + result, $"gameId={gameId}");
            }
            return resetAccountInfo;
        }
        public static async Task<ResetAccountModel> ResetAccountAsync(string gameId, string callby)
        {
            ResetAccountModel resetAccount = null;
            string result = string.Empty;
            tblLogGameAPI logGameAPI = new tblLogGameAPI()
            {
                Id = Guid.NewGuid(),
                Name = "resetAccount",
                CreatedBy = callby,
                CreatedTime = DateTime.Now,
                DataRequest = JsonConvert.SerializeObject(new { gameId = gameId })
            };
            try
            {
                HttpClient client = new HttpClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = await client.GetAsync(string.Format(apis["resetAccount"], gameId));
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();

                    logGameAPI.isSuccess = true;
                    logGameAPI.DataResult = result;
                    logGameAPI.ResultTime = DateTime.Now;

                    resetAccount = new JavaScriptSerializer().Deserialize<ResetAccountModel>(result);
                }
                else
                {
                    logGameAPI.isSuccess = false;
                    logGameAPI.ResultTime = DateTime.Now;

                    await Logging.LogToDBAsync("GameAccountAPI/ResetAccountAsync", "StatusCode:" + response.StatusCode + "\n Reason: " + response.ReasonPhrase, $"gameId={gameId}");
                }
                using (var db = new BankAPIEntities())
                {
                    db.tblLogGameAPIs.Add(logGameAPI);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("GameAccountAPI/ResetAccountAsync", ex + "\n" + result, $"gameId={gameId}");
            }
            return resetAccount;
        }
        public static async Task<ResetPasswordModel> ResetPasswordAsync(string gameId, string callby)
        {
            ResetPasswordModel resetPassword = null;
            string result = string.Empty;
            tblLogGameAPI logGameAPI = new tblLogGameAPI()
            {
                Id = Guid.NewGuid(),
                Name = "resetPasswordAccount",
                CreatedBy = callby,
                CreatedTime = DateTime.Now,
                DataRequest = JsonConvert.SerializeObject(new { gameId = gameId })
            };
            try
            {
                HttpClient client = new HttpClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = await client.GetAsync(string.Format(apis["resetPasswordAccount"], gameId));
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();

                    logGameAPI.isSuccess = true;
                    logGameAPI.DataResult = result;
                    logGameAPI.ResultTime = DateTime.Now;

                    resetPassword = new JavaScriptSerializer().Deserialize<ResetPasswordModel>(result);
                }
                else
                {
                    logGameAPI.isSuccess = false;
                    logGameAPI.ResultTime = DateTime.Now;

                    await Logging.LogToDBAsync("GameAccountAPI/ResetPasswordAsync", "StatusCode:" + response.StatusCode + "\n Reason: " + response.ReasonPhrase, $"gameId={gameId}");
                }
                using (var db = new BankAPIEntities())
                {
                    db.tblLogGameAPIs.Add(logGameAPI);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("GameAccountAPI/resetPasswordAccount", ex + "\n" + result, $"gameId={gameId}");
            }
            return resetPassword;
        }
        public static async Task<AdminBalanceModel> GetAdminBalance()
        {
            AdminBalanceModel adminBalance = null;
            string result = string.Empty;
            try
            {
                HttpClient client = new HttpClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = await client.GetAsync(apis["getAdminBalance"]);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    adminBalance = new JavaScriptSerializer().Deserialize<AdminBalanceModel>(result);
                }
                else
                {
                    await Logging.LogToDBAsync("GameAccountAPI/GetAdminBalance", "StatusCode:" + response.StatusCode + "\n Reason: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("GameAccountAPI/GetAdminBalance", ex + "\n" + result);
            }
            return adminBalance;
        }
        public static async Task<UserBalanceModel> GetBalance(string gameId)
        {
            UserBalanceModel userBalance = null;
            string result = string.Empty;
            try
            {
                HttpClient client = new HttpClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = await client.GetAsync(string.Format(apis["getBalance"], gameId));
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    userBalance = new JavaScriptSerializer().Deserialize<UserBalanceModel>(result);
                }
                else
                {
                    await Logging.LogToDBAsync("GameAccountAPI/GetBalance", "StatusCode:" + response.StatusCode + "\n Reason: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("GameAccountAPI/GetBalance", ex + "\n" + result);
            }
            return userBalance;
        }
        public static async Task<CheckLockedModel> CheckLocked(string gameId)
        {
            CheckLockedModel checkLocked = null;
            string result = string.Empty;
            try
            {
                HttpClient client = new HttpClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = await client.GetAsync(string.Format(apis["checkLock"], gameId));
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    checkLocked = new JavaScriptSerializer().Deserialize<CheckLockedModel>(result);
                }
                else
                {
                    await Logging.LogToDBAsync("GameAccountAPI/CheckLocked", "StatusCode:" + response.StatusCode + "\n Reason: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("GameAccountAPI/CheckLocked", ex + "\n" + result);
            }
            return checkLocked;
        }
    }
}