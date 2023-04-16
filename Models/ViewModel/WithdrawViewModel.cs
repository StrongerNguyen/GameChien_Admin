using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.ViewModel
{
    public class WithdrawViewModel
    {
        public Guid Id { get; set; }
        public long Amount { get; set; }
        public Nullable<bool> Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public string UpdatedByUserName { get; set; }
        public Nullable<DateTime> UpdatedTime { get; set; }
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public string BankName { get; set; }
        public string ShortName { get; set; }
        public Nullable<int> Bin { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankFullName { get; set; }
    }
}