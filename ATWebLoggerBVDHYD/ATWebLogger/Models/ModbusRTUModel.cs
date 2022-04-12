using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATWebLogger.Models
{
    public class ModbusRTUModel
    {
        public string Port { get; set; }
        public int Baudrate { get; set; }
        public int DataBits { get; set; }
        public int Parity { get; set; }
        public int Stopbits { get; set; }
        public int Timeout { get;set; }
    }
}