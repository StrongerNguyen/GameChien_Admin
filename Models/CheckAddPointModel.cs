using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models
{
    public class CheckAddPointModel
    {
        public int TransactionId { get; set; }
        public Guid CustomerRequestId { get; set; }
    }
}