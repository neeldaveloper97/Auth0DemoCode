using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace Rabbitmq_Receive
{
    class EmailService : ServiceBase
    {
        string smtpAddress = "smtp.gmail.com";
        int portNumber = 587;
        bool enableSSL = true;
        string emailFromAddress = "neel.demoacc@gmail.com"; //Sender Email Address  
        string password = "abcd@1234"; //Sender Password  
        //string emailToAddress = "neeldeveloper97@gmail.com"; //Receiver Email Address  
        string subject = "User verification email";
        //In real time, the email body should have a verification link (magic link) to verify email.
        string body = "Hello, <br><br> This is verification mock email.";
        private const string _logFileLocation = @"E:\temp\servicelog.txt";


        private static ConnectionFactory factory;
        private static IConnection connection;
        private static IModel channel;
        private static EventingBasicConsumer consumer;



        /// <summary>
        /// Function to log
        /// </summary>
        /// <param name="logMessage"></param>
        private void Log(string logMessage)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_logFileLocation));
            File.AppendAllText(_logFileLocation, DateTime.UtcNow.ToString() + " : " + logMessage + Environment.NewLine);
        }

        /// <summary>
        /// Function to send the email.
        /// </summary>
        public void SendEmail(string emailToAddress)
        {
            Log("SendEmail");
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFromAddress);
                    mail.To.Add(emailToAddress);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Log("SendEmail : " + ex.Message);
            }
        }

        /// <summary>
        /// Function responsible to execute message.
        /// </summary>
        /// <param name="message"></param>
        public void ProcessMessage(string message)
        {
            Log("ProcessMessage");
            try
            {
                //json can be made more complex and generic based of the different tasks or email types.
                var messageobj = JsonConvert.DeserializeObject<EmailTask>(message);
                if (messageobj.action == "verificationemail")
                {
                    SendEmail(messageobj.email);
                }
            }
            catch (Exception ex)
            {
                Log("ProcessMessage : " + ex.Message);
            }
        }

        /// <summary>
        /// Function to initialize
        /// </summary>
        public void Initialize()
        {

            Log("Initialize");

            try
            {
                factory = new ConnectionFactory() { HostName = "localhost" };
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                channel.QueueDeclare(queue: "task_queue",
                      durable: true,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    //this funtion will be called everytime a new message will be received.
                    ProcessMessage(message);
                    int dots = message.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "task_queue",
                                     autoAck: false,
                                     consumer: consumer);
                //}
            }
            catch (Exception ex)
            {
                Log("Initialize : " + ex.Message);
            }
        }

        protected override void OnStart(string[] args)
        {
            Initialize();
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            Log("stop");
            channel.Close();
            connection.Close();
            channel.Dispose();
            connection.Dispose();
            base.OnStop();
        }

        protected override void OnPause()
        {
            Log("pause");
            base.OnPause();
        }
    }
}
