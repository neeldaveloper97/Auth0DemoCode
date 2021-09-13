using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Net.Mail;
using System.Net;
using System.ServiceProcess;
using Rabbitmq_Receive;
using System.IO;

namespace Receive
{
    public class Receive
    {
       
        public static void Main(string[] args)
        {
            //start the service
            //Log("here");
            ServiceBase.Run(new EmailService());
        }
               
    }
}
