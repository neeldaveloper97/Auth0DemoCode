using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace WebAPIApplication.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        /// <summary>
        /// API which can be called without JWT 
        /// </summary>
        /// <returns></returns>
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok(new
            {
                Message = "Hello from a public endpoint! You don't need to be authenticated to see this."
            });
        }

        [HttpGet("private")]
        [Authorize]
        public IActionResult Private()
        {
            var result = GenerateRandomArray();
            string[] stringarr = Array.ConvertAll(result, x => x.ToString());

            return Ok(new
            {
                Message = String.Concat(stringarr)
            });
           
        }


       


        /// <summary>
        /// Function to generate array with random length and random value at each index.
        /// </summary>
        /// <returns></returns>
        [NonAction]

        public int[] GenerateRandomArray()
        {
            string ret = string.Empty;
            Random _random = new Random();
            int arrLength = _random.Next(5, 20);
            int[] array = new int[arrLength];

            for (int i = 0; i < arrLength; i++)
            {
                Random _randomNum = new Random();
                int arrValue = _randomNum.Next(0, 2);
                array[i] = arrValue;
            }
            return array;
        }

    }
}
