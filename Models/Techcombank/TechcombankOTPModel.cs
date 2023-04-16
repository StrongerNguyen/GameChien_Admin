using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models
{
    public class TechcombankOTPModel
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string SigningCode { get; set; }
        public string name { get; set; }
        public long systemId { get; set; }
    }
}