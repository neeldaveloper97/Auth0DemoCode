using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleLogin.Server
{
    public class DebugOutputSender : ISender
    {
        public int Send(string subject, string emailbody, string[] receivers, string sendername, string email                                                                       )
        {
            Debug.WriteLine("From SMS Service");
            return 1;
        }
    }
}
