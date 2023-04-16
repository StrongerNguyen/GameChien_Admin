using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Vietcombank
{
    public class VietcombankBankListModel
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<VietcombankBankList> banks { get; set; }
    }
    public class VietcombankBankList
    {
        public string bankCode { get; set; }
        public string bankName { get; set; }
        public string bankNameEN { get; set; }
        public string level { get; set; }
        public string fastTrans { get; set; }
    }
}