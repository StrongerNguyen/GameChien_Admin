using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Vietinbank
{
    public class Transaction
    {
        public string currency { get; set; }
        public string remark { get; set; }
        public string amount { get; set; }
        public string balance { get; set; }
        public string trxId { get; set; }
        public string processDate { get; set; }
        public string dorC { get; set; }
        public string refType { get; set; }
        public string refId { get; set; }
        public string tellerId { get; set; }
        public string corresponsiveAccount { get; set; }
        public string corresponsiveName { get; set; }
        public string channel { get; set; }
        public string serviceBranchId { get; set; }
        public string serviceBranchName { get; set; }
        public string pmtType { get; set; }
        public string sendingBankId { get; set; }
        public string sendingBranchId { get; set; }
        public string sendingBranchName { get; set; }
        public string receivingBankId { get; set; }
        public string receivingBranchId { get; set; }
        public string receivingBranchName { get; set; }
    }

    public class VietinbankTransactionModel
    {
        public string requestId { get; set; }
        public string sessionId { get; set; }
        public bool error { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string accountNo { get; set; }
        public int currentPage { get; set; }
        public int nextPage { get; set; }
        public int pageSize { get; set; }
        public int totalRecords { get; set; }
        public string warningMsg { get; set; }
        public List<Transaction> transactions { get; set; }
    }
}