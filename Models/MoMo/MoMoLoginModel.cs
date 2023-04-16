using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.MoMo
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class MomoMsg
    {
        public string userId { get; set; }
        public int agentId { get; set; }
        public bool isReged { get; set; }
        public bool isActived { get; set; }
        public bool isNamed { get; set; }
        public string identify { get; set; }
        public int capset { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public long lastLogin { get; set; }
        public string phoneOs { get; set; }
        public int appVer { get; set; }
        public string appCode { get; set; }
        public string langCode { get; set; }
        public string groupId { get; set; }
        public string createDate { get; set; }
        public string deviceName { get; set; }
        public bool isEu { get; set; }
        public bool untouchedClaimCode { get; set; }
        public bool startTrackerTrans { get; set; }
        public string nameKyc { get; set; }
        public string personalIdKyc { get; set; }
        public string passportKyc { get; set; }
        public string kycDob { get; set; }
        public string kycGender { get; set; }
        public string kycIssueDate { get; set; }
        public string kycIssuePlace { get; set; }
        public string kycExpiredDate { get; set; }
        public string kycNationality { get; set; }
        public string verifyInfo { get; set; }
        public string bankVerifyName { get; set; }
        public string bankVerifyPersonalid { get; set; }
        public string bankVerifyDob { get; set; }
        public string walletStatus { get; set; }
        public int faceMatching { get; set; }
        public int userType { get; set; }
        public bool fastLogin { get; set; }
        public string lastImei { get; set; }
        public long lastSessionTime { get; set; }
        public string bankName { get; set; }
        public string bankCode { get; set; }
        public string firmware { get; set; }
        public string validateEmail { get; set; }
        public bool isAccepted { get; set; }
        public List<string> beGroups { get; set; }
        public string _class { get; set; }
    }

    public class Extra
    {
        public string ONESIGNAL_TOKEN { get; set; }
        public string AUTH_TOKEN { get; set; }
        public string originalPhone { get; set; }
        public string SESSION_KEY { get; set; }
        public string REQUEST_ENCRYPT_KEY { get; set; }
        public string GOLDEN_PIG { get; set; }
        public string REFRESH_TOKEN { get; set; }
        public string PRE_LOGIN { get; set; }
        public string EMAIL { get; set; }
        public string BALANCE { get; set; }
        public string FIRST_TIME_LOGIN { get; set; }
        public string pHash { get; set; }
        public string VISA_NEW_FLOW { get; set; }
        public string FULL_NAME { get; set; }
        public string checkSum { get; set; }
        public string SIMULATOR { get; set; }
    }

    public class Ext
    {
        public string rkey { get; set; }
        public string onesignal { get; set; }
        public string ohash { get; set; }
        public string otp { get; set; }
        public string setupkey { get; set; }
        public string imei { get; set; }
        public string requestkey { get; set; }
        public string auth_token { get; set; }
    }

    public class MoMoLoginModel
    {
        public MomoMsg momoMsg { get; set; }
        public long time { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public string cmdId { get; set; }
        public string lang { get; set; }
        public string msgType { get; set; }
        public bool result { get; set; }
        public int errorCode { get; set; }
        public string errorDesc { get; set; }
        public string appCode { get; set; }
        public int appVer { get; set; }
        public string channel { get; set; }
        public string deviceOS { get; set; }
        public string session { get; set; }
        public Extra extra { get; set; }
        public string resultType { get; set; }
        public string path { get; set; }
        public Ext ext { get; set; }
    }
}