using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Vietcombank
{
    public class VietcombankOTPModel
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string tranfer_type { get; set; }
        public Transaction transaction { get; set; }
        public AcceptResposne accept_resposne { get; set; }
    }
    public class Fee
    {
        public string methodOTP { get; set; }
        public string methodFee { get; set; }
        public string exchangeMethodFee { get; set; }
        public string feeAmount { get; set; }
        public string totalFee { get; set; }
        public string totalFeeAll { get; set; }
        public string feeVat { get; set; }
        public string exchangeFeeAmount { get; set; }
        public string exchangeFeeVat { get; set; }
        public string exchangeTotalFee { get; set; }
        public string exchangeTotalFeeAll { get; set; }
    }

    public class Transaction
    {
        public string cusName { get; set; }
        public string tranId { get; set; }
        public string tranType { get; set; }
        public string debitAccountNo { get; set; }
        public string debitNewAccountNo { get; set; }
        public string debitAccountName { get; set; }
        public string debitAccountType { get; set; }
        public string debitAccountCcy { get; set; }
        public string debitBranchCode { get; set; }
        public string creditAccountNo { get; set; }
        public string creditAccountName { get; set; }
        public string creditAccountType { get; set; }
        public string amount { get; set; }
        public string content { get; set; }
        public string createTime { get; set; }
        public string exchangeRate { get; set; }
        public string feeType { get; set; }
        public string otpMethod { get; set; }
        public string backupOtpMethod { get; set; }
        public string remark { get; set; }
        public string exchangeAmount { get; set; }
        public List<Fee> fees { get; set; }
        public string creditBankCode { get; set; }
        public string creditBankName { get; set; }
        public string methodFee { get; set; }
        public string feeAmount { get; set; }
        public string totalFee { get; set; }
        public string feeVat { get; set; }
        public string exchangeFeeAmount { get; set; }
        public string exchangeFeeVat { get; set; }
        public string exchangeTotalFee { get; set; }
        public string exchangeMethodFee { get; set; }
        public int DebitSeq { get; set; }
        public int dIssueFee { get; set; }
        public int dTransportFee { get; set; }
        public List<string> listMethods { get; set; }
        public string fastTranTrace { get; set; }
        public int tsolRequestType { get; set; }
        public string groupTsol { get; set; }
        public bool isUpdateSoft { get; set; }
    }

    public class AcceptResposne
    {
        public string otpPhoneNumber { get; set; }
        public string mid { get; set; }
        public string code { get; set; }
        public string des { get; set; }
    }
}