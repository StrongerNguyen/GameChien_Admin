using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.TPBank
{
    public class Error
    {
        public string error_code { get; set; }
        public string error_message { get; set; }
        public string remain_try_number { get; set; }
        public string limit_try_number { get; set; }
        public bool require_captcha { get; set; }
        public bool require_update_pass { get; set; }
    }
    public class TPBankLoginModel
    {
        public Error error { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}