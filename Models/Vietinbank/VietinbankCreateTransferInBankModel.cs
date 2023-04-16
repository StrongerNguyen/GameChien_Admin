using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Vietinbank.CreateTransferInBank
{
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
        public bool accumulatedCd { get; set; }
    }

    public class Fees
    {
        public string feeType { get; set; }
        public int feeAmount { get; set; }
        public int taxAmount { get; set; }
        public bool earlyPayment { get; set; }
    }

    public class Method
    {
        public string id { get; set; }
        public string number { get; set; }
        public string type { get; set; }
    }

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

    public class TransactionFee
    {
        public string feeCode { get; set; }
        public string feeAmount { get; set; }
        public string vatAmount { get; set; }
        public string feeGLAccount { get; set; }
        public string vatGLAccount { get; set; }
        public object errorMessage { get; set; }
        public object feeDesc { get; set; }
        public object transactionName { get; set; }
        public object feeTransactionName { get; set; }
        public object vatTransactionName { get; set; }
        public object vatPercentage { get; set; }
        public object glAccount { get; set; }
        public bool earlyPayment { get; set; }
        public object feeRate { get; set; }
        public object transactionAmount { get; set; }
    }
    public class VietinbankCreateTransferInBankModel
    {
        public string requestId { get; set; }
        public string sessionId { get; set; }
        public bool error { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string toAccountName { get; set; }
        public Fees fees { get; set; }
        public string authenticationActionCode { get; set; }
        public string paymentId { get; set; }
        public string systemDate { get; set; }
        public List<Method> methods { get; set; }
        public string smsOtpTemplate { get; set; }
        public TransactionFee transactionFee { get; set; }
        public string toBranchId { get; set; }
        public string serviceType { get; set; }
        public string orgAcctNo { get; set; }
        public AcctRecvObj acctRecvObj { get; set; }
    }
}