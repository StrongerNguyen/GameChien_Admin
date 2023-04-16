using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.MoMo.BankList
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class NapasBank
    {
        public string bankCode { get; set; }
        public string bankName { get; set; }
        public string shortBankName { get; set; }
        public List<string> whiteLists { get; set; }
        public bool available { get; set; }
    }

    public class MoMoBankListModel
    {
        public int resultCode { get; set; }
        public string description { get; set; }
        public List<NapasBank> napasBanks { get; set; }
    }
}