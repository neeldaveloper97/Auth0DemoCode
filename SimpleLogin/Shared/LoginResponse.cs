using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLogin.Shared
{
    public class LoginResponse
    {
        public int Status { get; set; }
        public string token { get; set; }
        public int expirein { get; set; }
    }

    public class ErrorResponse
    {
        public string error { get; set; }
        public string error_description { get; set; }
    }

    public class SuccessResponse
    {
        public string access_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }

    }
}
