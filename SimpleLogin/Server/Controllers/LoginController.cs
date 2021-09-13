using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RestSharp;
using SimpleLogin.Shared;
using System;
using System.Text;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleLogin.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        public IConfiguration Configuration { get; }

        public LoginController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // POST api/<Login>
        [HttpPost]
        public IActionResult Post([FromBody] Login login)
        {
            try
            {
                //This entire code here can be used from service.

                var APIURL = Configuration["Auth0:Auth0TokenEndPoint"];
                var audiance = Configuration["Auth0:Audiance"];
                var clientId = Configuration["Auth0:ClientId"];
                var clientSecret = Configuration["Auth0:ClientSecret"];

                var client = new RestClient(APIURL);
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");

                request.AddParameter("application/x-www-form-urlencoded", "grant_type=password&username=" + login.Username + "&password=" + login.Password + "&audience=" + audiance + "&client_id=" + clientId + "&client_secret=" + clientSecret, ParameterType.RequestBody);
                IRestResponse APIresponse = client.Execute(request);
                string ResponseString = APIresponse.Content;
                LoginResponse loginResponse = new LoginResponse();
                if (APIresponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var JWT = JsonConvert.DeserializeObject<SuccessResponse>(ResponseString);
                    loginResponse.Status = 1;
                    loginResponse.token = JWT.access_token;
                    loginResponse.expirein = JWT.expires_in;
                }
                else
                {
                    //in case of unauthorized (wrong emaail or password)
                    loginResponse.Status = -1;
                }
                return Ok(loginResponse);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("confirmemail")]
        [HttpPost]
        public IActionResult ConfirmEmail([FromBody] EmailAPIRequest emailAPIRequest)
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
                    string message = "{\"email\":\"" + emailAPIRequest.email + "\",\"action\":\"verificationemail\"}";
                    var body = Encoding.UTF8.GetBytes(message);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    channel.BasicPublish(exchange: "",
                     routingKey: "task_queue",
                     basicProperties: properties,
                     body: body);
                }
            }
            return Ok(new
            {
                Message = "Message queued to send verification email."
            });
        }

    }
}
