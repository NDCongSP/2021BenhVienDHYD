using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATWebLogger.Models
{
    public class AlarmModel
    {
        public bool EnableEmail { get; set; }
        public bool EnableSMS { get; set; }
        public string Email { get; set; }
        public string SMS { get; set; }
        public string EmailKDNT { get; set; }
        public string SMSKDNT { get; set; }
    }
}