using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.ViewModel
{
    public class TransactionViewModel
    {
        public Guid Id { get; set; }
        public string GetBy { get; set; }
        public string Device { get; set; }
        public string BankName { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public string CD { get; set; }
        public long Amount { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedTime { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<Guid> LastUpdateBy { get; set; }
        public string LastUpdateByUserName { get; set; }
        public Nullable<DateTime> LastUpdateTime { get; set; }
        public Nullable<Guid> DepositId { get; set; }
    }
}