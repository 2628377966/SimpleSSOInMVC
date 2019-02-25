using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSO.Models
{
    public class User
    {
        public string UserName { get; set; }
    }

    public class TokenUser
    {
        public string Token { get; set; }
        public User User { get; set; }
    }

    public class ResultEx
    {
        public bool Flag { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
    }
}