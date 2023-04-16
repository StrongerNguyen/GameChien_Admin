using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Excel
{
    public class DepositExcelModel
    {
        public string GameId { get; set; }
        public string GameAccountName { get; set; }
        public string PhoneNumber { get; set; }
        public double Point { get; set; }
        public Nullable<int> MoneyOfPoint { get; set; }
        public Nullable<double> Total { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<bool> isCallAPIError { get; set; }
        public System.DateTime CreatedTime { get; set; }
        public string AttachFile { get; set; }
        public string ReportErrorMessage { get; set; }
    }
}