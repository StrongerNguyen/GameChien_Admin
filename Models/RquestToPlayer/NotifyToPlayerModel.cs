using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.RquestToPlayer
{
    public class NotifyToPlayerModel
    {
        public string AccountName { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public NotifyToPlayerModel(string accountName, string type, string message)
        {
            AccountName = accountName;
            Type = type;
            Message = message;
        }
    }
}