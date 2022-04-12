using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATWebLogger.Models
{
    public class WriteValueModel
    {
        public int DeviceId { get; set; }
        public string DataType { get; set; }
        public int Address { get; set; }
        public double Value { get; set; }
    }
}