using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.ViewModel
{
    public class PlayerViewModel
    {
        public Guid Id { get; set; }
        public string AccountName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string GameAccount { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public Nullable<int> Gender { get; set; }
        public long Credit { get; set; }
        public int GameLevel { get; set; }
        public int PercentOfLevelUp { get; set; }
        public string LastIPAddress { get; set; }
        public DateTime CreatedTime { get; set; }
        public Nullable<DateTime> LastTimeModified { get; set; }
        public Nullable<int> Status { get; set; }
        public bool isVerifiedGameAccount { get; set; }
        public bool isBlock { get; set; }
    }
}