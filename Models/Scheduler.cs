using FT_Admin.Hubs;
using FT_Admin.Models.Data;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FT_Admin.Models
{
    public class Scheduler
    {
        private static int timerRunJob = int.Parse(ConfigurationManager.AppSettings["timerRunJob"] ?? "60");
        public void Start()
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            // get a scheduler
            IScheduler scheduler = factory.GetScheduler();
            scheduler.Start();

            #region JobCheckSql
            //JobCheckSql
            IJobDetail jobHandleNotification = JobBuilder.Create<JobHandleNotification>()
                .WithIdentity("jobHandleNotification", "group1")
                .Build();

            //TriggerCheckSql
            ITrigger triggerHandleNotification = TriggerBuilder.Create()
                .WithIdentity("triggerHandleNotification", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(1)
                    .RepeatForever())
            .Build();
            #endregion
            //#region jobAutoAddPoint
            ////JobCheckGameIdUnAvaliable
            //IJobDetail jobAutoAddPoint = JobBuilder.Create<JobAutoAddPoint>()
            //    .WithIdentity("jobAutoAddPoint", "group10")
            //    .Build();
            //ITrigger triggerAutoAddPoint = TriggerBuilder.Create()
            //    .WithIdentity("triggerAutoAddPoint", "group10")
            //    .StartNow()
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInSeconds(2)
            //        .RepeatForever())
            //.Build();
            //#endregion

            scheduler.ScheduleJob(jobHandleNotification, triggerHandleNotification);
            //scheduler.ScheduleJob(jobAutoAddPoint, triggerAutoAddPoint);
        }

        [DisallowConcurrentExecution]
        public class JobHandleNotification : IJob
        {
            public async void Execute(IJobExecutionContext context)
            {
                using (var db = new GameChienEntities())
                {
                    var notify = db.tblNotifications.Where(t => !t.Executed).OrderBy(t => t.CreatedTime).FirstOrDefault();
                    if (notify != null)
                    {
                        notify.Executed = true;
                        db.SaveChanges();

                        var configTransactionByMobileApp = db.tblConfigTransactionByMobileApps.FirstOrDefault(t => t.isActive == true && t.From.Equals(notify.From));
                        if (configTransactionByMobileApp != null) await Handing(notify, configTransactionByMobileApp);
                    }
                }
            }
        }
        private static async Task Handing(tblNotification notify, tblConfigTransactionByMobileApp appConfig)
        {
            try
            {
                using (var db = new GameChienEntities())
                {
                    var transaction = new tblTransaction()
                    {
                        Id = Guid.NewGuid(),
                        BankName = appConfig.Name,
                        CreatedTime = notify.CreatedTime,
                        GetBy = notify.Type,
                        Status = null,
                        Device = notify.DeviceName
                    };

                    if (transaction.BankName.Equals("MoMo") || transaction.BankName.Equals("Techcombank")) transaction.Description = notify.Data2;
                    else transaction.Description = notify.Data;
                    string contentCheck = transaction.Description;
                    string contentCheck2 = transaction.Description;
                    //Lấy ngày
                    Regex regexDate = new Regex(@"\ \d{2}-\d{2}-\d{4}\ ");
                    Match matchDate = regexDate.Match(contentCheck);
                    if (matchDate.Success)
                    {
                        transaction.TransactionDate = matchDate.Value.Trim().Replace("-", "/");
                    }
                    else
                    {
                        transaction.TransactionDate = notify.CreatedTime.ToString("dd/MM/yyyy");
                    }
                    if (!DateTime.Now.ToString("dd/MM/yyyy").Equals(transaction.TransactionDate) && !DateTime.Now.ToString("HH").Equals("00"))
                    {
                        return;
                    }
                    //lấy giờ
                    Regex regexTime = new Regex(@"\ \d{2}:\d{2}:\d{2}\.");
                    Match matchTime = regexTime.Match(contentCheck);
                    if (matchTime.Success)
                    {
                        transaction.TransactionTime = matchTime.Value.Trim().Replace(".", "");
                    }
                    else
                    {
                        transaction.TransactionTime = notify.CreatedTime.ToString("HH:mm:ss");
                    }
                    if (!contentCheck.Contains(" OTP "))
                    {
                        contentCheck = contentCheck.Replace("VND", "").Replace(" ", "");
                        Regex regexMoney = new Regex(@"(-|\+)?\d{1,3}((,|\.)\d{3})+");
                        Match matchMoney = regexMoney.Match(contentCheck);
                        if (matchMoney.Success)
                        {
                            //Lấy được tiền
                            var money = matchMoney.Value;
                            if (money[0] == '-' || money[0] == '+')
                            {
                                transaction.CD = money[0].ToString();
                            }
                            money = money.Replace("-", "").Replace("+", "").Replace(".", "").Replace(",", "");
                            int.TryParse(money, out int amount);
                            if (amount > 0)
                            {
                                transaction.Amount = amount;
                            }
                        }
                    }
                    //if (transaction.Amount == null)
                    //{
                    //    //Không lấy được tiền thì lấy số
                    //    Regex regexNumber = new Regex(@"-?\d+");
                    //    Match matchNumber = regexNumber.Match(contentCheck);
                    //    if (matchNumber.Success)
                    //    {
                    //        int.TryParse(matchNumber.Value, out int amount);
                    //        if (amount > 0)
                    //        {
                    //            transaction.Amount = amount;
                    //        }
                    //    }
                    //}

                    int indexOfRef = contentCheck.IndexOf("Ref");
                    if (indexOfRef > -1)
                    {
                        transaction.Description = contentCheck.Substring(indexOfRef, contentCheck.Length - indexOfRef);
                    }
                    if (transaction.BankName.Equals("TPBank"))
                    {
                        string keycheck = "ND: ";
                        int indexOfND = contentCheck2.IndexOf(keycheck);
                        if (indexOfND > -1)
                        {
                            transaction.Description = contentCheck2.Substring(indexOfND + keycheck.Length, contentCheck2.Length - indexOfND - keycheck.Length).Trim();
                        }
                    }
                    if (transaction.BankName.Equals("Techcombank"))
                    {
                        string keycheck = ",000 ";
                        int indexOfTechcombank = contentCheck2.IndexOf(keycheck);
                        if (indexOfTechcombank > -1)
                        {
                            transaction.Description = contentCheck2.Substring(indexOfTechcombank + keycheck.Length, contentCheck2.Length - indexOfTechcombank - keycheck.Length).Trim();
                        }
                    }
                    if (transaction.BankName.Equals("BIDV"))
                    {
                        string keycheck = "Nội dung giao dịch: ";
                        int indexOfBIDV = contentCheck2.IndexOf(keycheck);
                        if (indexOfBIDV > -1)
                        {
                            transaction.Description = contentCheck2.Substring(indexOfBIDV + keycheck.Length, contentCheck2.Length - indexOfBIDV - keycheck.Length).Trim();
                        }
                    }
                    //if (transaction.BankName.Contains("MoMo"))
                    //{
                    //    Regex regexMoney = new Regex(@"([.\d,]+)");
                    //    Match matchMoney = regexMoney.Match(content);
                    //    if (matchMoney.Success)
                    //    {
                    //        transaction.Amount = matchMoney.Value;
                    //    }
                    //}
                    //else if (transaction.BankName.Contains("TPBank"))
                    //{
                    //    Regex regexMoney = new Regex(@"(?<=\+\s)[\d.]+|(?<=\+)[\d.]+");
                    //    Match matchMoney = regexMoney.Match(content);
                    //    if (matchMoney.Success)
                    //    {
                    //        transaction.Amount = matchMoney.Value;
                    //        transaction.CD = "+";
                    //    }
                    //    else
                    //    {
                    //        Regex regexMoneySub = new Regex(@"(?<=\-\s)[\d.]+|(?<=\-)[\d.]+");
                    //        Match matchMoneySub = regexMoneySub.Match(content);
                    //        if (matchMoneySub.Success)
                    //        {
                    //            transaction.Amount = matchMoneySub.Value;
                    //            transaction.CD = "-";
                    //        }
                    //    }
                    //}
                    //else if (transaction.BankName.Contains("Techcombank"))
                    //{
                    //    content = content.Replace(" VND ", "");
                    //    Regex regexMoney = new Regex(@"(?<=\+\s)[\d,]+|(?<=\+)[\d,]+");
                    //    Match matchMoney = regexMoney.Match(content);
                    //    if (matchMoney.Success)
                    //    {
                    //        transaction.Amount = matchMoney.Value;
                    //        transaction.CD = "+";
                    //    }
                    //    else
                    //    {
                    //        Regex regexMoneySub = new Regex(@"(?<=\-\s)[\d,]+|(?<=\-)[\d,]+");
                    //        Match matchMoneySub = regexMoneySub.Match(content);
                    //        if (matchMoneySub.Success)
                    //        {
                    //            transaction.Amount = matchMoneySub.Value;
                    //            transaction.CD = "-";
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    Regex regexMoney = new Regex(@"(?<=\+\s)[\d,]+|(?<=\+)[\d,]+");
                    //    Match matchMoney = regexMoney.Match(content);
                    //    if (matchMoney.Success)
                    //    {
                    //        transaction.Amount = matchMoney.Value;
                    //        transaction.CD = "+";
                    //    }
                    //    else
                    //    {
                    //        Regex regexMoneySub = new Regex(@"(?<=\-\s)[\d,]+|(?<=\-)[\d,]+");
                    //        Match matchMoneySub = regexMoneySub.Match(content);
                    //        if (matchMoneySub.Success)
                    //        {
                    //            transaction.Amount = matchMoneySub.Value;
                    //            transaction.CD = "-";
                    //        }
                    //    }
                    //}
                    //if (string.IsNullOrEmpty(transaction.Amount))
                    //{
                    //    Regex regexNumber = new Regex(@"-?\d+");
                    //    Match matchNumber = regexNumber.Match(content);
                    //    if (matchNumber.Success)
                    //    {
                    //        transaction.Amount = matchNumber.Value;
                    //    }
                    //}

                    //Auto Add Point
                    //Regex regexPhone = new Regex(@"(0\d{9})");
                    //var matchPhone = regexPhone.Matches(transaction.Description);

                    //Lấy sdt
                    Regex regexPhone = new Regex(@"((\ |\.|\,|\:|\;|\-|IBFT)0\d{9}(\ |$|\.|\,|\:|\;|\-|FT))", RegexOptions.RightToLeft);
                    if (transaction.BankName.Equals("TPBank"))
                    {
                        regexPhone = new Regex(@"((\ |\.|\,|\:|\;|\-|IBFT|)0\d{9}(\ |$|\.|\,|\:|\;|\-|FT|))");
                    }
                    transaction.Description = transaction.Description.Replace("0925410862", "").Replace("0326597652", "");
                    var matchPhone = regexPhone.Match(transaction.Description);
                    if (matchPhone.Success)
                    {
                        transaction.PhoneNumber = matchPhone.Value.Trim().Replace(" ", "").Replace(".", "").Replace(",", "").Replace(":", "").Replace(";", "").Replace("-", "").Replace("IBFT", "").Replace("FT", "");
                    }
                    else
                    {
                        string phone = "";
                        regexPhone = new Regex(@"(\ |[a-zA-Z])0\d{9}($|[a-zA-Z])", RegexOptions.RightToLeft);
                        matchPhone = regexPhone.Match(transaction.Description);
                        if (matchPhone.Success)
                        {
                            phone = matchPhone.Value.Trim();
                            regexPhone = new Regex(@"0\d{9}");
                            matchPhone = regexPhone.Match(phone);
                            if (matchPhone.Success)
                            {
                                transaction.PhoneNumber = matchPhone.Value;
                            }
                        }
                    }
                    db.tblTransactions.Add(transaction);
                    await db.SaveChangesAsync();
                    new RealtimeHub().updateTransaction(transaction.Id);
                    //Check xem transaction này có sdt không? có đang cộng điểm không? số tiền có trống không?
                    if (!string.IsNullOrEmpty(transaction.PhoneNumber) && transaction.CD.Equals("+"))
                    {
                        //Check xem có player nào trùng sdt không
                        tblPlayer playerFromDB = await db.tblPlayers.FirstOrDefaultAsync(t => t.PhoneNumber.Equals(transaction.PhoneNumber));
                        if (playerFromDB != null)
                        {
                            //check xem có lệnh nạp nào của player này chưa xử lý không
                            tblDeposit deposit = db.tblDeposits.FirstOrDefault(t => t.PlayerId == playerFromDB.Id && t.Status == null && t.Amount == transaction.Amount);
                            if (deposit == null)
                            {
                                deposit = new tblDeposit()
                                {
                                    Id = Guid.NewGuid(),
                                    Amount = transaction.Amount,
                                    CreatedTime = DateTime.Now,
                                    PlayerId = playerFromDB.Id
                                };
                                db.tblDeposits.Add(deposit);
                                db.SaveChanges();
                            }
                            deposit.Status = true;
                            deposit.TransactionId = transaction.Id;
                            deposit.UpdatedTime = DateTime.Now;
                            playerFromDB.Credit += deposit.Amount;
                            db.SaveChanges();
                            new RealtimeHub().updateTransaction(transaction.Id);
                            new RealtimeHub().updatePlayer(playerFromDB.Id);
                            //Call api player
                            PlayerAPI.pushNotifyToPlayer(new RquestToPlayer.NotifyToPlayerModel(playerFromDB.AccountName, "success", $"Bạn đã nạp thành công {deposit.Amount.ToString("N0")}VND"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Logging.LogToDBAsync("Scheduler/JobHandleNotification", ex);
            }
        }

        //#region Check & Cộng điểm
        //[DisallowConcurrentExecution]
        //public class JobAutoAddPoint : IJob
        //{
        //    public async void Execute(IJobExecutionContext context)
        //    {
        //        try
        //        {
        //            string checkBy = ConfigurationManager.AppSettings["CheckAddPointBy"]?.ToString() ?? string.Empty;
        //            if (!string.IsNullOrEmpty(checkBy))
        //            {
        //                using (var db = new BankAPIEntities())
        //                {
        //                    //Lấy danh sách chuyển khoản chưa được xử lý & có số điện thoại nằm trong danh sách các lệnh nạp

        //                    var dateCheck = DateTime.Now.Date;

        //                    List<string> listPhoneCheck = db.tblCustomerRequests.Where(t => t.Type && !t.isProcessing && t.isActive && string.IsNullOrEmpty(t.Status) && t.isCallAPIError == null).GroupBy(t => t.tblCustomer.PhoneNumber).Select(t => t.Key).ToList();
        //                    List<tblTransaction> transactions = db.tblTransactions.Where(t =>
        //                    DbFunctions.TruncateTime(t.DateTime) == dateCheck
        //                    && t.CD.Equals("+")
        //                    && t.Status.Equals("null")
        //                    && !string.IsNullOrEmpty(t.PhoneNumber)
        //                    && t.From.Contains(checkBy)
        //                    && t.Amount != null
        //                    && listPhoneCheck.Contains(t.PhoneNumber)).OrderBy(t => t.DateTime).ToList();

        //                    foreach (var tran in transactions)
        //                    {
        //                        //Lấy khách hàng
        //                        tblTransaction transactionFromDB = db.tblTransactions.FirstOrDefault(t => t.Id == tran.Id);
        //                        tblCustomer customerFromDB = db.tblCustomers.OrderByDescending(t => t.CreatedTime).FirstOrDefault(t => t.PhoneNumber == transactionFromDB.PhoneNumber);

        //                        if (customerFromDB != null)
        //                        {
        //                            tblCustomerRequest customerRequestFromDB = db.tblCustomerRequests.Where(t => t.CustomerId == customerFromDB.Id
        //                            && DbFunctions.TruncateTime(t.CreatedTime) == dateCheck
        //                            && t.Type
        //                            && !t.isProcessing
        //                            && t.isActive
        //                            && string.IsNullOrEmpty(t.Status)
        //                            && string.IsNullOrEmpty(t.ReportErrorMessage)
        //                            && t.Total != null
        //                            && t.Total == transactionFromDB.Amount
        //                            && t.isCallAPIError == null).ToList().OrderBy(t => t.CreatedTime).FirstOrDefault();

        //                            if (customerRequestFromDB != null)
        //                            {
        //                                //Update đang xử lý -> push cho user biết
        //                                customerRequestFromDB.UpdatingBy = "Job";
        //                                db.SaveChanges();
        //                                new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                                //transaction.isProcessing = true;
        //                                //customerRequest.Processingg = true

        //                                bool isActiveAccount = false;
        //                                //Check xem lệnh này phải lệnh active không
        //                                if (string.IsNullOrEmpty(customerFromDB.GameId) || !customerFromDB.isActive)
        //                                {
        //                                    isActiveAccount = true;
        //                                    //Đây là lệnh kích hoạt tài khoản

        //                                    //Lấy gameID có sẵn để gán
        //                                    List<tblCustomer> listCustomerAvaliable = db.tblCustomers.Where(t =>
        //                                    !string.IsNullOrEmpty(t.GameId)
        //                                    && string.IsNullOrEmpty(t.PhoneNumber)
        //                                    && string.IsNullOrEmpty(t.GameAccountName)).OrderBy(t => t.CreatedTime).ToList();

        //                                    if (listCustomerAvaliable != null && listCustomerAvaliable.Count > 0)
        //                                    {
        //                                        //lấy ngẫu nhiên account
        //                                        Random rd = new Random();
        //                                        tblCustomer customerAvaliableGameId;
        //                                        if (listCustomerAvaliable.Count > 10)
        //                                        {
        //                                            customerAvaliableGameId = listCustomerAvaliable[rd.Next(10)];
        //                                        }
        //                                        else
        //                                        {
        //                                            customerAvaliableGameId = listCustomerAvaliable[rd.Next(listCustomerAvaliable.Count)];
        //                                        }
        //                                        //reset lại 1 lần nữa
        //                                        ResetAccountModel resetAccount = GameAccountAPI.ResetAccountAsync(customerAvaliableGameId.GameId, "JOB").Result;
        //                                        if (resetAccount != null && resetAccount.status)
        //                                        {
        //                                            customerFromDB.GameId = customerAvaliableGameId.GameId;
        //                                            customerFromDB.Password = ConfigurationManager.AppSettings["passWord"];
        //                                            customerFromDB.isActive = true;

        //                                            customerAvaliableGameId.GameId += "_" + customerAvaliableGameId.CreatedTime.ToString("ddMMyyyyHHmmss");
        //                                            customerAvaliableGameId.GameAccountName = customerAvaliableGameId.GameId + "_" + customerAvaliableGameId.CreatedTime.ToString("ddMMyyyyHHmmss");
        //                                            customerAvaliableGameId.isActive = false;
        //                                        }
        //                                    }
        //                                    //else
        //                                    //{
        //                                    //    //đang không có sẵn => call api để tạo
        //                                    //    CreateAccountAPIModel createAccountAPI = await GameAccountAPI.CreateAccountAsync(1);
        //                                    //    if (createAccountAPI != null && createAccountAPI.status)
        //                                    //    {
        //                                    //        customerFromDB.GameId = createAccountAPI.data.FirstOrDefault();
        //                                    //        customerFromDB.Password = ConfigurationManager.AppSettings["passWord"];
        //                                    //        customerFromDB.isActive = true;
        //                                    //    }
        //                                    //    else
        //                                    //    {
        //                                    //        customerRequestFromDB.UpdatingBy = null;
        //                                    //        db.SaveChanges();
        //                                    //        new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                                    //    }
        //                                    //}
        //                                }
        //                                if (!string.IsNullOrEmpty(customerFromDB.GameId) && customerFromDB.isActive)
        //                                {
        //                                    //Add điểm
        //                                    string note = $"{transactionFromDB.TransactionDate.Substring(0, 5)} {transactionFromDB.PCTime} {(transactionFromDB.Amount?.ToString("n0") ?? "")}";
        //                                    AddPointAccountModel addPointAccount = GameAccountAPI.AddPointAccountAsync(customerFromDB.GameId, customerRequestFromDB.Point, customerRequestFromDB.MoneyOfPoint.Value / 1000, note, "JOB").Result;
        //                                    if (addPointAccount != null && addPointAccount.status)
        //                                    {
        //                                        //Add điểm thành công
        //                                        //Cập nhật request
        //                                        customerRequestFromDB.Status = "yes";
        //                                        customerRequestFromDB.Note = note;
        //                                        customerRequestFromDB.UpdatingBy = null;
        //                                        customerRequestFromDB.UpdateBy = "Job";

        //                                        //cập nhật transaction
        //                                        transactionFromDB.Status = "yes";
        //                                        transactionFromDB.UpdateBy = "Job";
        //                                        //Cập nhật customer
        //                                        customerFromDB.GameAccountName = addPointAccount.loginName;
        //                                        customerFromDB.MoneyOfPoint = customerRequestFromDB.MoneyOfPoint;
        //                                        db.SaveChanges();

        //                                        //tính tổng nạp
        //                                        double totalDeposit = db.Database.SqlQuery<double>($"select isnull(sum(Total),0) from tblCustomerRequest where [Type] = 1 and CustomerId = '{customerFromDB.Id}' and [Status] = 'yes'").FirstOrDefault();

        //                                        //Check tổng nạp để cập nhật vip
        //                                        if (totalDeposit > 0)
        //                                        {
        //                                            //cập nhật tổng nạp
        //                                            customerFromDB.TotalDeposit = (long)totalDeposit;
        //                                            tblVipConfig vipConfig = db.tblVipConfigs.Where(t => t.FromAmount <= totalDeposit).OrderByDescending(t => t.FromAmount).FirstOrDefault();
        //                                            if (vipConfig != null)
        //                                            {
        //                                                customerFromDB.VipConfigId = vipConfig.Id;
        //                                            }
        //                                            db.SaveChanges();
        //                                        }

        //                                        //Push Update transaction
        //                                        new RealtimeHub().updateTransaction(transactionFromDB.Id);
        //                                        //Push update request
        //                                        new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                                        //Push update customer
        //                                        new RealtimeHub().customerRequestCompleted(customerRequestFromDB.Id, customerRequestFromDB.Status, customerRequestFromDB.Type, isActiveAccount, null);
        //                                        //Push update totalSum
        //                                        var date = DateTime.Now.Date;
        //                                        var totalUpdateFromDB = db.tblTotalUpdates.Where(t => t.Date == date && t.Type).OrderByDescending(t => t.Id).FirstOrDefault();
        //                                        if (totalUpdateFromDB != null)
        //                                        {
        //                                            totalUpdateFromDB.Total += customerRequestFromDB.Total ?? 0;
        //                                        }
        //                                        db.SaveChanges();

        //                                        new RealtimeHub().updateTotalDeposit();
        //                                    }
        //                                    else
        //                                    {
        //                                        customerRequestFromDB.UpdatingBy = null;
        //                                        customerRequestFromDB.isCallAPIError = true;
        //                                        customerRequestFromDB.Note = addPointAccount?.message ?? "API AddPointAccount return Null";
        //                                        db.SaveChanges();
        //                                        new RealtimeHub().updateCustomerRequest(customerRequestFromDB.Id);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            await Logging.LogToDBAsync("Scheduler/JobAutoAddPoint", ex);
        //        }
        //    }
        //}
        //#endregion
    }
}