using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Vietinbank
{
    public class Account
    {
        public string number { get; set; }
        public string bsb { get; set; }
        public string type { get; set; }
        public string currencyCode { get; set; }
        public string status { get; set; }
        public string openDate { get; set; }
        public AccountState accountState { get; set; }
        public string relationshipType { get; set; }
        public string relationshipTypeName { get; set; }
        public string title { get; set; }
        public string entityNumber { get; set; }
        public string productId { get; set; }
        public string branchName { get; set; }
        public string holdAmt { get; set; }
        public string restrictionCode { get; set; }
    }

    public class AccountState
    {
        public List<ServiceLimit> serviceLimits { get; set; }
        public int availableBalance { get; set; }
        public int balance { get; set; }
    }

    public class Entity
    {
        public string title { get; set; }
        public bool isDefault { get; set; }
        public string emailAddress { get; set; }
        public string number { get; set; }
        public string mobileNumber { get; set; }
        public string type { get; set; }
        public string defaultAccount { get; set; }
        public string segmentId { get; set; }
        public string segmentName { get; set; }
    }

    public class VietinbankEntitiesAndAccountModel
    {
        public string requestId { get; set; }
        public string sessionId { get; set; }
        public bool error { get; set; }
        public string customerType { get; set; }
        public string customerNumber { get; set; }
        public int totalAmount { get; set; }
        public List<Account> accounts { get; set; }
        public List<Entity> entities { get; set; }
    }

    public class ServiceLimit
    {
        public string service { get; set; }
    }
}