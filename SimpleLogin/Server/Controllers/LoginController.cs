using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RestSharp;
using SimpleLogin.Shared;
using System;
using System.Text;
using static SimpleLogin.Server.Startup;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleLogin.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    { 
        public readonly ISender Sender;
        private readonly Auth0 _Config;

        public LoginController(IOptions<Auth0> config, ISender sender, ServiceResolver serviceResolver)
        {
            _Config = config.Value;
            Sender = serviceResolver(_Config.ServiceKey);
        }

        // POST api/<Login>
        [HttpPost]
        public IActionResult Post([FromBody] Login login)
        {
            try
            {
                //This entire code here can be used from service.
                var APIURL = _Config.Auth0TokenEndPoint;
                var audiance = _Config.Audiance; 
                var clientId = _Config.ClientId;
                var clientSecret = _Config.ClientSecret;

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
            string message;
            if (emailAPIRequest != null && (!string.IsNullOrEmpty(emailAPIRequest.email)))
            {
                //added service call
                int nRet = Sender.Send("","",null, "", emailAPIRequest.email);
                if (nRet > 0)
                {
                    message = "Message queued to send verification email.";
                }
                else
                {
                    message = "Something went wrong.";
                }
            }
            else
            {
                message = "Missing parameters";
            }
            return Ok(new
            {
                Message = message
            });

        }
    }
}
