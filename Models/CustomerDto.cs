using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models
{
    public class CustomerDto
    {
        public System.Guid Id { get; set; }
        public string GameId { get; set; }
        public string GameAccountName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankFullName { get; set; }
        public string Note { get; set; }
        public string UpdateBy { get; set; }
        public System.DateTime CreatedTime { get; set; }
        public bool isActive { get; set; }
        public bool isReset { get; set; }
        public long TotalDeposit { get; set; }
        public string VipName { get; set; }

    }
}