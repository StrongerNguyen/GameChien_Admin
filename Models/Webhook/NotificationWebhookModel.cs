﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Webhook
{
    public class NotificationWebhookModel
    {
        public string Type { get; set; }
        public string DeviceName { get; set; }
        public string From { get; set; }
        public string Data { get; set; }
        public string Data2 { get; set; }
    }
}