using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.ViewModel
{
    public class DepositViewModel
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public string PlayerAccountName { get; set; }
        public long Amount { get; set; }
        public string AttachFile { get; set; }
        public Nullable<Guid> TransactionId { get; set; }
        public DateTime CreatedTime { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<Guid> UpdatedBy { get; set; }
        public string UpdatedByUserName { get; set; }
        public Nullable<DateTime> UpdatedTime { get; set; }

    }
}