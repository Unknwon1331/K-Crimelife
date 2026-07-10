using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife
{
    public class EmailRegister
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("readed")]
        public bool readed { get; set; }
        [JsonProperty("subject")]
        public string subject { get; set; }
        [JsonProperty("body")]
        public string body { get; set; }

        public EmailRegister() { }
    }
}
