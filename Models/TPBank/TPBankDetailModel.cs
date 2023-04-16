using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.TPBank
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class AdditionInformation
    {
        public string AC_STAT_BLOCK { get; set; }
        public string CUSTOMER_NAME1 { get; set; }
        public string THAUCHI_STK_UDF { get; set; }
        public string ACCOUNT_DESCRIPTION { get; set; }
        public string ACCOUNT_UDF { get; set; }
        public string TOD_LIMIT { get; set; }
        public string NGAY_BAT_DAU_HMTC { get; set; }
        public string AUTH_STAT { get; set; }
        public string RATE { get; set; }
        public string CUSTOMER_TYPE { get; set; }
        public string BLOCK_AMOUNT { get; set; }
        public string AC_STAT_STOP_PAY { get; set; }
        public string MONEY_UN_AUTHORIZE { get; set; }
        public DateTime AC_OPEN_DATE { get; set; }
        public string AC_STAT_NO_DR { get; set; }
        public string NGAY_KET_THUC_HMTC { get; set; }
        public string AC_STAT_NO_CR { get; set; }
        public string CHK_EFFECT_OVERDRAFT { get; set; }
        public string AC_STAT_FROZEN { get; set; }
        public string JOINT_ACC { get; set; }
        public string AC_CLASS_TYPE { get; set; }
        public string RECORD_STAT { get; set; }
    }
    public class MessagesDetail
    {
        public string en { get; set; }
        public string vn { get; set; }
    }
    public class ErrorMessageDetail
    {
        public string errorCode { get; set; }
        public object errorDesc { get; set; }
        public MessagesDetail messages { get; set; }
    }
    public class TPBankDetailModel
    {
        public string bookedBalance { get; set; }
        public string availableBalance { get; set; }
        public string BBAN { get; set; }
        public string currency { get; set; }
        public string bankBranchCode { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string accountClass { get; set; }
        public bool canDoFundTopup { get; set; }
        public string description { get; set; }
        public string branchName { get; set; }
        public AdditionInformation additionInformation { get; set; }
        public int responseCode { get; set; }
        public int requestCode { get; set; }
        public ErrorMessageDetail errorMessage { get; set; }
    }
}