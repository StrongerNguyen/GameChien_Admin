using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Excel
{
    public class SummaryCashoutWithBankExcelModel
    {
        public string BankName { get; set; }
        public string SendTo_BankName { get; set; }
        public string SendTo_BankAccountNumber { get; set; }
        public string SendTo_BankFullName { get; set; }
        public int TotalMoney { get; set; }
        public string CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public bool isActive { get; set; }
    }
}