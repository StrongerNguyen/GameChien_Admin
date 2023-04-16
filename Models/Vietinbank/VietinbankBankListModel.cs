using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Vietinbank
{
    public class CodeMapping
    {
        public string context { get; set; }
        public string lang { get; set; }
        public string code { get; set; }
        public string value { get; set; }
        public string value2 { get; set; }
        public string valueType { get; set; }
        public string order { get; set; }
    }

    public class VietinbankBankListModel
    {
        public string requestId { get; set; }
        public string sessionId { get; set; }
        public bool error { get; set; }
        public List<CodeMapping> codeMapping { get; set; }
    }
}