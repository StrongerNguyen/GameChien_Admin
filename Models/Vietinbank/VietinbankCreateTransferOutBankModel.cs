using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Vietinbank
{
    public class Fees
    {
        public string feeType { get; set; }
        public int feeAmount { get; set; }
        public int taxAmount { get; set; }
        public bool earlyPayment { get; set; }
    }

    public class Method
    {
        public string id { get; set; }
        public string number { get; set; }
        public string type { get; set; }
    }

    public class VietinbankCreateTransferOutBankModel
    {
        public string requestId { get; set; }
        public string sessionId { get; set; }
        public bool error { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public Fees fees { get; set; }
        public string authenticationActionCode { get; set; }
        public string paymentId { get; set; }
        public string systemDate { get; set; }
        public List<Method> methods { get; set; }
        public string smsOtpTemplate { get; set; }
        public string beneficiaryName { get; set; }
        public string beneficiaryBank { get; set; }
    }
}