using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.MoMo.TransferToMoMo
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class EnableOptions
    {
        public bool voucher { get; set; }
        public bool discount { get; set; }
        public bool prepaid { get; set; }
        public string desc { get; set; }
    }

    public class TranHisMsg
    {
        public string otpType { get; set; }
        public string ipAddress { get; set; }
        public EnableOptions enableOptions { get; set; }
        public string _class { get; set; }
        public string ID { get; set; }
        public string user { get; set; }
        public string commandInd { get; set; }
        public long tranId { get; set; }
        public long clientTime { get; set; }
        public long ackTime { get; set; }
        public int tranType { get; set; }
        public int io { get; set; }
        public string partnerId { get; set; }
        public string partnerCode { get; set; }
        public string partnerName { get; set; }
        public int amount { get; set; }
        public string comment { get; set; }
        public int status { get; set; }
        public string ownerNumber { get; set; }
        public string ownerName { get; set; }
        public int moneySource { get; set; }
        public string desc { get; set; }
        public string serviceMode { get; set; }
        public int originalAmount { get; set; }
        public string serviceId { get; set; }
        public int quantity { get; set; }
        public long lastUpdate { get; set; }
        public string share { get; set; }
        public int receiverType { get; set; }
        public string extras { get; set; }
        public string channel { get; set; }
    }

    public class BankInReply
    {
        public TranHisMsg tranHisMsg { get; set; }
        public string _class { get; set; }
    }

    public class ReplyMsg
    {
        public string ID { get; set; }
        public long transId { get; set; }
        public bool isSucess { get; set; }
        public TranHisMsg tranHisMsg { get; set; }
        public string _class { get; set; }
    }

    public class MomoMsg
    {
        public BankInReply bankInReply { get; set; }
        public List<ReplyMsg> replyMsgs { get; set; }
        public string _class { get; set; }
    }

    public class Extra
    {
        public string totalAmount { get; set; }
        public string originalAmount { get; set; }
        public string originalClass { get; set; }
        public string originalPhone { get; set; }
        public string totalFee { get; set; }
        public string checkSum { get; set; }
        public string GetUserInfoTaskRequest { get; set; }
    }

    public class TransferToMoMoModel
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