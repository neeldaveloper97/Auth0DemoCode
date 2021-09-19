using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleLogin.Server
{
    public interface ISender
    {
        /// <summary>
        /// Function to send email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        int Send (string subject, string emailbody, string[] receivers, string sendername, string email);
    }
}
