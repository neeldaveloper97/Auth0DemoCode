using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleLogin.Server
{
    public class SMSService : ISender
    {
        public int Send(string email)
        {
            Debug.WriteLine("From SMS Service");
            return 1;
        }
    }
}
