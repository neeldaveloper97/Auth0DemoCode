using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLogin.Server
{
    public class EmailSender : ISender
    {
        public int Send(string subject, string emailbody, string[] receivers, string sendername, string email)
        {
            int nRet = 0;
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "task_queue",
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                        //created a json message so that different type of messages can be handled at the receiving end.
                        string message = "{\"email\":\"" + email + "\",\"action\":\"verificationemail\"}";
                        var body = Encoding.UTF8.GetBytes(message);
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        channel.BasicPublish(exchange: "",
                         routingKey: "task_queue",
                         basicProperties: properties,
                         body: body);
                        nRet = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                nRet = -1;
            }
            return nRet;
        }
    }
}
