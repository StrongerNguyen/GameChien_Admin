using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Vietinbank.Login
{
    public class VietinbankLoginModel
    {
        public string requestId { get; set; }
        public string sessionId { get; set; }
        public bool error { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string systemDate { get; set; }
        public string status { get; set; }
        public string customerNumber { get; set; }
        public string ipayId { get; set; }
        public List<object> unreadMessages { get; set; }
        public string addField3 { get; set; }
        public string tokenId { get; set; }
    }
}