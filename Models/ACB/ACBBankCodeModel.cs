using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.ACB
{
    public class BankNapas
    {
        public string bank { get; set; }
        public string bankName { get; set; }
        public string napasBankCode { get; set; }
        public string thumbnail { get; set; }
        public bool fastTransferSupported { get; set; }
    }
    public class ACBBankCodeModel
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<BankNapas> data { get; set; }
    }
}