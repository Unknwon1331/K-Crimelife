using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife
{
    public class Ringtone
    {
        [JsonProperty("id")]
        public int Id
        {
            get;
            set;
        }
        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }
        [JsonProperty("file")]
        public string File
        {
            get;
            set;
        }

        public Ringtone(int id, string name, string file)
        {
            this.Id = id;
            this.Name = name;
            this.File = file;
            Console.WriteLine(Id + " | " + Name);
        }
    }
}
   
