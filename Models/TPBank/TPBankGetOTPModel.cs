using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.TPBank
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Messages
    {
        public string en { get; set; }
        public string vn { get; set; }
    }

    public class TitlesOTP
    {
        public string en { get; set; }
        public string vn { get; set; }
    }

    public class ErrorMessage
    {
        public string errorCode { get; set; }
        public object errorDesc { get; set; }
        public Messages messages { get; set; }
        public TitlesOTP titles { get; set; }
    }

    public class TPBankGetOTPModel
    {
        public int responseCode { get; set; }
        public int requestCode { get; set; }
        public object description { get; set; }
        public ErrorMessage errorMessage { get; set; }
        public string id { get; set; }
        public string authMethod { get; set; }
        public string feeAmount { get; set; }
        public string feeCurrency { get; set; }
        public string feeDesc { get; set; }
    }
}