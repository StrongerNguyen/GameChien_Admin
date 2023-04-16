using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models
{
    public class Payer
    {
        public object piID { get; set; }
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public string bankName { get; set; }
        public object customerId { get; set; }
    }

    public class Payee
    {
        public object piID { get; set; }
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public string bankName { get; set; }
        public object customerId { get; set; }
    }

    public class Transaction
    {
        public Payer payer { get; set; }
        public Payee payee { get; set; }
        public object useCase { get; set; }
        public string txnRef { get; set; }
        public object mobiliserTxnId { get; set; }
        public string coreBankTxnId { get; set; }
        public int txnAmount { get; set; }
        public object effectiveDate { get; set; }
        public object feeType { get; set; }
        public string txnDesc { get; set; }
        public string debitCurrency { get; set; }
        public string creditCurrency { get; set; }
        public int balanceAfterTxn { get; set; }
        public object status { get; set; }
        public object fastalink { get; set; }
        public object mobSubUseCase { get; set; }
        public DateTime txnDate { get; set; }
        public object additionalInfos { get; set; }
        public object socialTxnType { get; set; }
        public object mobInvoiceId { get; set; }
    }

    public class Status
    {
        public object value { get; set; }
        public int code { get; set; }
    }
    public class TechcombankTransactionModel
    {
        public object conversationId { get; set; }
        public List<Transaction> transactions { get; set; }
        public Status Status { get; set; }
    }
}