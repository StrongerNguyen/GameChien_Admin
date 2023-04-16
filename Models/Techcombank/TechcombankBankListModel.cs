using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models
{
    public class BankList
    {
        public string Code { get; set; }
        public int BankID { get; set; }
        public string BankName { get; set; }
    }
    public class TechcombankBankListModel
    {
        public bool success { get; set; }
        public List<BankList> BankList { get; set; }
    }
}