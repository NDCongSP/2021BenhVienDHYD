using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATWebLogger.Models
{
    public class WriteValueATMSModel
    {
        public int DeviceId { get; set; }
        public double Deadband { get; set; }
        public double TempHigh { get; set; }
        public double TempLow { get; set; }
        public double Offset { get; set; }
    }
}