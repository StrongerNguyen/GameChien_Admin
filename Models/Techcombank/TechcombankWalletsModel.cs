using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Techcombank
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class SpareFields
    {
        public object spareString1 { get; set; }
        public object spareString2 { get; set; }
        public string spareString3 { get; set; }
        public object spareString4 { get; set; }
        public object spareString5 { get; set; }
        public object spareString6 { get; set; }
        public object spareString7 { get; set; }
        public object spareString8 { get; set; }
        public object spareString9 { get; set; }
        public object spareString10 { get; set; }
        public object spareDate1 { get; set; }
        public object spareDate2 { get; set; }
        public object spareDate3 { get; set; }
        public object spareDate4 { get; set; }
        public object spareDate5 { get; set; }
        public int spareInteger1 { get; set; }
        public int spareInteger2 { get; set; }
        public int spareInteger3 { get; set; }
        public int spareInteger4 { get; set; }
        public int spareInteger5 { get; set; }
        public object spareBoolean1 { get; set; }
        public object spareBoolean2 { get; set; }
        public object spareBoolean3 { get; set; }
        public object spareBoolean4 { get; set; }
        public object spareBoolean5 { get; set; }
        public object spareCLOB1 { get; set; }
    }

    public class BankAccount
    {
        public string accountHolderName { get; set; }
        public string accountNumber { get; set; }
        public string displayNumber { get; set; }
        public string bankCode { get; set; }
        public string branchCode { get; set; }
        public object bankName { get; set; }
        public string bankCountry { get; set; }
        public object bankCity { get; set; }
        public object lastUpdate { get; set; }
        public long id { get; set; }
        public long customerId { get; set; }
        public bool active { get; set; }
        public int status { get; set; }
        public string currency { get; set; }
        public bool multiCurrency { get; set; }
        public object issuerId { get; set; }
        public object issuerName { get; set; }
        public int type { get; set; }
        public object limitSetId { get; set; }
        public object hash { get; set; }
        public SpareFields spareFields { get; set; }
    }

    public class Sva
    {
        public int creditBalance { get; set; }
        public int creditReserved { get; set; }
        public int debitBalance { get; set; }
        public int debitReserved { get; set; }
        public object lastBookingDate { get; set; }
        public object lastSvaBookingId { get; set; }
        public object maxBalance { get; set; }
        public int minBalance { get; set; }
        public object refundInTransaction { get; set; }
        public object refundAbsoluteAmount { get; set; }
        public object refundPaymentInstrumentId { get; set; }
        public object refundTargetAmount { get; set; }
        public object refundThreshold { get; set; }
        public long id { get; set; }
        public long customerId { get; set; }
        public bool active { get; set; }
        public int status { get; set; }
        public string currency { get; set; }
        public bool multiCurrency { get; set; }
        public object issuerId { get; set; }
        public object issuerName { get; set; }
        public int type { get; set; }
        public object limitSetId { get; set; }
        public object hash { get; set; }
        public object spareFields { get; set; }
    }

    public class WalletEntry
    {
        public object id { get; set; }
        public object customerId { get; set; }
        public object paymentInstrumentId { get; set; }
        public int? debitPriority { get; set; }
        public int? creditPriority { get; set; }
        public object limitSetId { get; set; }
        public string alias { get; set; }
        public BankAccount bankAccount { get; set; }
        public object creditCard { get; set; }
        public object externalAccount { get; set; }
        public Sva sva { get; set; }
        public object offlineSva { get; set; }
    }

    public class Status
    {
        public object value { get; set; }
        public int code { get; set; }
    }

    public class TechcombankWalletsModel
    {
        public object conversationId { get; set; }
        public List<WalletEntry> walletEntries { get; set; }
        public Status Status { get; set; }
    }
}