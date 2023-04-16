using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Techcombank
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class AuthMethodPayer
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isInlineAuthentication { get; set; }
        public bool isDynamicCredential { get; set; }
    }

    public class AuthenticationMethods
    {
        public object authMethodPayee { get; set; }
        public AuthMethodPayer authMethodPayer { get; set; }
    }

    public class StatusOTP
    {
        public string value { get; set; }
        public int code { get; set; }
    }

    public class Transaction
    {
        public object value { get; set; }
        public long systemId { get; set; }
        public object type { get; set; }
    }

    public class TeckcombankConfirmOTPModel
    {
        public object conversationId { get; set; }
        public AuthenticationMethods authenticationMethods { get; set; }
        public StatusOTP Status { get; set; }
        public Transaction Transaction { get; set; }
    }
}