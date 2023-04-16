using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.MoMo.MoMoTransaction
{
    public class EnableOptions
    {
        public bool voucher { get; set; }
        public bool discount { get; set; }
        public bool prepaid { get; set; }
        public string desc { get; set; }
    }

    public class TranList
    {
        public string ID { get; set; }
        public string user { get; set; }
        public string commandInd { get; set; }
        public object tranId { get; set; }
        public object clientTime { get; set; }
        public object ackTime { get; set; }
        public object finishTime { get; set; }
        public int tranType { get; set; }
        public int io { get; set; }
        public string partnerCode { get; set; }
        public int amount { get; set; }
        public int status { get; set; }
        public string ownerNumber { get; set; }
        public int moneySource { get; set; }
        public string desc { get; set; }
        public string rowCardId { get; set; }
        public int originalAmount { get; set; }
        public string serviceId { get; set; }
        public int quantity { get; set; }
        public object lastUpdate { get; set; }
        public string share { get; set; }
        public int receiverType { get; set; }
        public string extras { get; set; }
        public string channel { get; set; }
        public string otpType { get; set; }
        public string ipAddress { get; set; }
        public EnableOptions enableOptions { get; set; }
        public string _class { get; set; }
        public string partnerName { get; set; }
        public int? category { get; set; }
        public string partnerId { get; set; }
        public string billId { get; set; }
        public int? parentTranType { get; set; }
        public string customerNumber { get; set; }
        public string serviceName { get; set; }
        public int? pageNumber { get; set; }
        public string tranData { get; set; }
        public string prepaidIds { get; set; }
        public object comment { get; set; }
        public string ownerName { get; set; }
        public int? partnerError { get; set; }
    }

    public class MomoMsg
    {
        public long begin { get; set; }
        public long end { get; set; }
        public List<TranList> tranList { get; set; }
        public string _class { get; set; }
    }

    public class Extra
    {
        public string originalClass { get; set; }
        public string originalPhone { get; set; }
        public string checkSum { get; set; }
    }

    public class MoMoTransactionsModel
    {
        public MomoMsg momoMsg { get; set; }
        public long time { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public string cmdId { get; set; }
        public string lang { get; set; }
        public string msgType { get; set; }
        public bool result { get; set; }
        public int errorCode { get; set; }
        public string errorDesc { get; set; }
        public string appCode { get; set; }
        public int appVer { get; set; }
        public string channel { get; set; }
        public string deviceOS { get; set; }
        public string path { get; set; }
        public string session { get; set; }
        public Extra extra { get; set; }
    }
}