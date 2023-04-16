using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.MoMo.ConfirmOTP
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class MomoMsg
    {
        public string _class { get; set; }
    }

    public class Extra
    {
        public string CUSTOM_OUT_MSG { get; set; }
        public string requiredCall { get; set; }
    }

    public class Ext
    {
        public string rkey { get; set; }
        public string onesignal { get; set; }
        public string imei { get; set; }
    }

    public class MoMoGetOTPModel
    {
        public MomoMsg momoMsg { get; set; }
        public long time { get; set; }
        public string user { get; set; }
        public string cmdId { get; set; }
        public string lang { get; set; }
        public string msgType { get; set; }
        public bool result { get; set; }
        public int errorCode { get; set; }
        public string errorDesc { get; set; }
        public string appCode { get; set; }
        public int appVer { get; set; }
        public string channel { get; set; }
        public string session { get; set; }
        public Extra extra { get; set; }
        public string path { get; set; }
        public Ext ext { get; set; }
    }


}