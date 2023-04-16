using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models
{
    public static class WithdrawSuccessQueue
    {
        public static HashSet<Guid> Queues { get; set; }
    }
}