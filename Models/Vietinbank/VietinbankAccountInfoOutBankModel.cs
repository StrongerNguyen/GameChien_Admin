using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Vietinbank
{
    public class VietinbankAccountInfoOutBankModel
    {
        public string requestId { get; set; }
        public string sessionId { get; set; }
        public bool error { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string systemDate { get; set; }
        public string beneficiaryName { get; set; }
        public string beneficiaryBank { get; set; }
    }
}