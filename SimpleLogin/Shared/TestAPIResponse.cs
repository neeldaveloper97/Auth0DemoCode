
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLogin.Shared
{
    public class TestAPIResponse
    {
        public string Message { get; set; }

    }

    public class APIResponse
    {
        public List<int> MyArray { get; set; }
    }

    public class EmailAPIRequest
    {
        public string email { get; set; }
    }
}
