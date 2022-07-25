using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication.Model
{
    public class AuthResponse
    {
        public string access_token { get; set; }

        public string refresh_token { get; set; }

        public string id_token { get; set; }

        public string token_type { get; set; }
    }
}