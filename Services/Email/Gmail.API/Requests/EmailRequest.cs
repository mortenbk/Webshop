using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gmail.API.Requests
{
    public class EmailRequest
    {
        public string Receiver { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
