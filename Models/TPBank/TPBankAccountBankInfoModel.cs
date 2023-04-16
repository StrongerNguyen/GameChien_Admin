namespace FT_Admin.Models.TPBank
{
    public class MessagesACC
    {
        public string en { get; set; }
        public string vn { get; set; }
    }

    public class Titles
    {
        public string en { get; set; }
        public string vn { get; set; }
    }

    public class ErrorMessageACC
    {
        public string errorCode { get; set; }
        public object errorDesc { get; set; }
        public MessagesACC messages { get; set; }
        public Titles titles { get; set; }
    }

    public class CreditorInfo
    {
        public string name { get; set; }
        public string accountNumber { get; set; }
        public string extBankId { get; set; }
        public string extBankCode { get; set; }
        public string extBankNameEn { get; set; }
        public string extBankNameVn { get; set; }
        public string currency { get; set; }
        public int guarantee { get; set; }
    }

    public class TPBankAccountBankInfoModel
    {
        public int responseCode { get; set; }
        public int requestCode { get; set; }
        public object description { get; set; }
        public ErrorMessageACC errorMessage { get; set; }
        public CreditorInfo creditorInfo { get; set; }
        public long timestamp { get; set; }
        public int status { get; set; }
        public string error { get; set; }
        public string message { get; set; }
        public string path { get; set; }
    }
}