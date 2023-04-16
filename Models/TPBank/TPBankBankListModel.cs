using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.TPBankBankList
{
    public class TPBankBankListModel
    {
        public List<Banksnapas> banksnapas { get; set; }
    }
    public class Banksnapas
    {
        public string en_name { get; set; }
        public string vn_name { get; set; }
        public string bankId { get; set; }
        public string bicCode { get; set; }
        public string atmBin { get; set; }
        public int cardLength { get; set; }
        public string shortName { get; set; }
        public string bankCode { get; set; }
        public string type { get; set; }
        public bool napasSupported { get; set; }
        public string status { get; set; }
        public string channel { get; set; }
    }
}