using FT_Admin.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models
{
    public class BankAccountModel
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AccountNumber { get; set; }
        public string Token { get; set; }
        public bool isActive { get; set; }
        public int CountOfError { get; set; }
        public Nullable<System.DateTime> LoginTime { get; set; }
        public string Error { get; set; }
        public Nullable<System.DateTime> LasttimeRunJob { get; set; }
        public Nullable<int> BankConfigId { get; set; }
        public string OTP { get; set; }

        public BankConfigModel tblBankConfig { get; set; }
    }
}