using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.ACB
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Data
    {
        public string edtOtp { get; set; }
        public bool isRequiredOTP { get; set; }
        public string uuid { get; set; }
    }

    public class ACBTransferModel
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string tranfer_type { get; set; }
        public DateTime time { get; set; }
        public int codeStatus { get; set; }
        public string messageStatus { get; set; }
        public string description { get; set; }
        public int took { get; set; }
        public Data data { get; set; }
        public int redisTook { get; set; }
    }
}