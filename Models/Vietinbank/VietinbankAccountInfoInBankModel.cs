using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Vietinbank
{
    public class Result
    {
        public string result { get; set; }
        public string hostRefNum { get; set; }
        public string hostFeeRefNum { get; set; }
        public string postedDate { get; set; }
        public string desc { get; set; }
        public object currentBalance { get; set; }
        public object availableBalance { get; set; }
        public string mt103Status { get; set; }
        public string postedDateIncas { get; set; }
        public string mt103Desc { get; set; }
        public string logDesc { get; set; }
        public string hostRefNumP2 { get; set; }
        public string hostFeeRefNumP2 { get; set; }
        public string postedDateP2 { get; set; }
        public string statusCode { get; set; }
        public string serverStatusCode { get; set; }
        public string severity { get; set; }
    }

    public class AcctRecvObj
    {
        public string accountNo { get; set; }
        public string accountType { get; set; }
        public string branchId { get; set; }
        public string branchName { get; set; }
        public string customerName { get; set; }
        public string productId { get; set; }
        public string holdAmt { get; set; }
        public string restrictionCode { get; set; }
        public Result result { get; set; }
        public string maturityDate { get; set; }
        public string settlementAmount { get; set; }
        public string interestTerm { get; set; }
        public string interestTermCode { get; set; }
        public string originalAmount { get; set; }
        public string fineAmount { get; set; }
        public string spreadInterestRate { get; set; }
        public string pricipalCurAmt { get; set; }
        public string svcFeeAmt { get; set; }
        public string nxtDepAmt { get; set; }
        public string channelName { get; set; }
        public bool accumulatedCd { get; set; }
    }

    public class VietinbankAccountInfoInBankModel
    {
        public string requestId { get; set; }
        public string sessionId { get; set; }
        public bool error { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string toAccountName { get; set; }
        public string systemDate { get; set; }
        public string toBranchId { get; set; }
        public string orgAcctNo { get; set; }
        public AcctRecvObj acctRecvObj { get; set; }
    }
}