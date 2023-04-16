using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Vietcombank
{
    public class VCBTransactionModel
    {
        public string TransactionDate { get; set; }
        public string Reference { get; set; }
        public string CD { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string PCTime { get; set; }
        public DateTime DateTime
        {
            get
            {
                if (PCTime.Length == 1) PCTime = "00000" + PCTime;
                if (PCTime.Length == 2) PCTime = "0000" + PCTime;
                if (PCTime.Length == 3) PCTime = "000" + PCTime;
                if (PCTime.Length == 4) PCTime = "00" + PCTime;
                if (PCTime.Length == 5) PCTime = "0" + PCTime;
                var dt = DateTime.ParseExact(TransactionDate + " " + PCTime.Substring(0, 4), "dd/MM/yyyy HHmm", CultureInfo.InvariantCulture);
                if (dt > DateTime.Now) dt = dt.AddDays(-1);
                return dt;
            }
            set { }
        }
    }
    public class VCBTransactionResultModel
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<VCBTransactionModel> transactions { get; set; }
        public string nextIndex { get; set; }
    }

}