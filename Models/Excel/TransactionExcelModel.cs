using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Excel
{
    public class TransactionExcelModel
    {
        public string From { get; set; }
        public string BankName { get; set; }
        public string TransactionDate { get; set; }
        public string PCTime { get; set; }
        public string DateTime { get; set; }
        public string CD { get; set; }
        public Nullable<int> Amount { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string UpdateBy { get; set; }
    }
}