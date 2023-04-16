using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models
{
    public class BankConfigModel
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string Logo { get; set; }
        public bool isUseAccountNumberInLogin { get; set; }
        public bool isUseOTPInLogin { get; set; }
        public bool isActive { get; set; }
    }
}