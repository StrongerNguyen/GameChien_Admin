using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.ACB
{
    public class Transaction
    {
        public int amount { get; set; }
        public string accountName { get; set; }
        public object receiverName { get; set; }
        public int transactionNumber { get; set; }
        public string description { get; set; }
        public object bankName { get; set; }
        public bool isOnline { get; set; }
        public object postingDate { get; set; }
        public object accountOwner { get; set; }
        public string type { get; set; }
        public string receiverAccountNumber { get; set; }
        public string currency { get; set; }
        public int account { get; set; }
        public object activeDatetime { get; set; }
        public object effectiveDate { get; set; }
    }

    public class ACBTransactionsModel
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int took { get; set; }
        public List<Transaction> transactions { get; set; }
        public int total { get; set; }
        public int page { get; set; }
        public int size { get; set; }
    }

}