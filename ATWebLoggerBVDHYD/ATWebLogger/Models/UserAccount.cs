using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATWebLogger
{
    public class UserAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}