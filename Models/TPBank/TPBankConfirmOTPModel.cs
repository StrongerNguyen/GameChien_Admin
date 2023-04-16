using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.TPBank
{
    public class TPBankConfirmOTPModel
    {
        public long timestamp { get; set; }
        public int status { get; set; }
        public string error { get; set; }
        public string message { get; set; }
        public string path { get; set; }
        public int responseCode { get; set; }
        public int requestCode { get; set; }
        public object description { get; set; }
        public ErrorMessage errorMessage { get; set; }
    }
}