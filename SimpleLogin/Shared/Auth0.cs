using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLogin.Shared
{
    public class Auth0
    {
        public string Audiance { get; set; }
        public string ClientId { get; set; }
        public string Auth0TokenEndPoint { get; set; }
        public string ClientSecret { get; set; }
    }
}
