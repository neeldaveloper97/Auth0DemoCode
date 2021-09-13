using System.ComponentModel.DataAnnotations;

namespace SimpleLogin.Shared
{
    public class Login
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}