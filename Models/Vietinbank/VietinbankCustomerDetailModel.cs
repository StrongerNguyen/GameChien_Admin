using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Vietinbank
{
    public class CustomerInfo
    {
        public string address { get; set; }
        public string address1 { get; set; }
        public object address2 { get; set; }
        public object address3 { get; set; }
        public object address4 { get; set; }
        public string birthPlace { get; set; }
        public string branchId { get; set; }
        public string branchName { get; set; }
        public string customerNumber { get; set; }
        public string customerSegment { get; set; }
        public string customerSegmentName { get; set; }
        public string dateOfBirth { get; set; }
        public string district { get; set; }
        public string email { get; set; }
        public string extraDetails { get; set; }
        public string fax { get; set; }
        public string firstName { get; set; }
        public string fullname { get; set; }
        public string gender { get; set; }
        public string idNation { get; set; }
        public string idNumber { get; set; }
        public string idNumberNonMasking { get; set; }
        public string idType { get; set; }
        public string idTypeDesc { get; set; }
        public string issueDate { get; set; }
        public string issueLocation { get; set; }
        public string jobTitle { get; set; }
        public string lastName { get; set; }
        public string middleName { get; set; }
        public string nationId { get; set; }
        public string nation { get; set; }
        public string ocupation { get; set; }
        public string phone { get; set; }
        public string province { get; set; }
        public string subType { get; set; }
        public string subTypeName { get; set; }
        public string taxId { get; set; }
        public string title { get; set; }
        public string cifno { get; set; }
        public string identificationNo { get; set; }
        public string custType { get; set; }
        public string name { get; set; }
        public string serviceType { get; set; }
        public string evnCode { get; set; }
        public string idExpDate { get; set; }
        public string feeAcctNo { get; set; }
        public object errorCode { get; set; }
        public object errorMessage { get; set; }
        public bool blackList { get; set; }
        public string customerEkyc { get; set; }
    }

    public class VietinbankCustomerDetailModel
    {
        public string requestId { get; set; }
        public string sessionId { get; set; }
        public bool error { get; set; }
        public CustomerInfo customerInfo { get; set; }
        public string userName { get; set; }
    }
}