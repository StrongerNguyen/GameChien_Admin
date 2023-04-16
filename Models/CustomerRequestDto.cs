using FT_Admin.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models
{
    public class CustomerRequestDto
    {
        public System.Guid Id { get; set; }
        public System.Guid CustomerId { get; set; }
        public string GameId { get; set; }
        public string GameAccountName { get; set; }
        public string PhoneNumber { get; set; }
        public bool Type { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankFullName { get; set; }
        public string Point { get; set; }
        public string MoneyOfPoint { get; set; }
        public double? Total { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public string UpdateBy { get; set; }
        public string UpdatingBy { get; set; }
        public string SubtractPointBy { get; set; }
        public string SendFromBank { get; set; }
        public Nullable<bool> isCallAPIError { get; set; }
        public System.DateTime CreatedTime { get; set; }
        public bool isActive { get; set; }
        public string AttachFile { get; set; }
        public CustomerDto Customer { get; set; }
        public string ReportErrorMessage { get; set; }
        public string BankBin { get; set; }
    }
}