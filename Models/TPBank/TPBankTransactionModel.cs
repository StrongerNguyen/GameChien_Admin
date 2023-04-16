using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.TPBank
{
    public class TransactionInfo
    {
        public string id { get; set; }
        public string arrangementId { get; set; }
        public string reference { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public string bookingDate { get; set; }
        public string valueDate { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string creditDebitIndicator { get; set; }
        public string runningBalance { get; set; }
        public string ofsAcctNo { get; set; }
        public string ofsAcctName { get; set; }
        public string creditorBankNameVn { get; set; }
        public string creditorBankNameEn { get; set; }
    }
    public class TPBankTransactionModel
    {
        public List<TransactionInfo> transactionInfos { get; set; }
    }
}