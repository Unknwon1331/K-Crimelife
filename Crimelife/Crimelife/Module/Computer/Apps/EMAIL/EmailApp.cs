using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using GTANetworkAPI;

namespace Crimelife
{
    public class EmailApp : Script
    {
        public static List<EmailRegister> AllEmails = new List<EmailRegister>();
        [RemoteEvent("requestEmails")]
        public void OnRequestEmails(Player client)
        {
            if (client == null || !client.Exists) return;

            try
            {
                var emails = new EmailRegister
                {
                    id = 0,
                    readed = false,
                    subject = "test",
                    body = "test2"
                };
                AllEmails.Add(emails);
                //  Console.WriteLine($"{NAPI.Util.FromJson(GetEmailsByName(client.Name))}");

                client.TriggerEvent("componentServerEvent", new object[3] {
                "EmailApp",
                "responseEmails",
                NAPI.Util.ToJson(AllEmails)
            });

            }
            catch (Exception exception)
            {
                Console.WriteLine($"OnRequestEmails: {exception.StackTrace}");
                Console.WriteLine($"OnRequestEmails: {exception.Message}");
            }
        }


        public static string GetEmailsByName(string playerName)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(Configuration.connectionString);
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText =
                    "SELECT * FROM accounts WHERE Username=@name";
                command.Parameters.AddWithValue("@name", playerName);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        string jsonOutput = reader.GetString("emails");
                        return jsonOutput;
                    }
                }

                connection.Close();
            }
            catch (Exception exception)
            {
                NAPI.Util.ConsoleOutput($"GetJailcostsByName: {exception.StackTrace}");
                NAPI.Util.ConsoleOutput($"GetJailcostsByName: {exception.Message}");
            }

            return "";
        }
    }

    public class EmailAppModule
    {
        public int id { get; set; }
        public bool readed { get; set; }
        public string subject { get; set; }
        public string body { get; set; }

        public EmailAppModule(int id, bool readed, string subject, string body)
        {
            this.id = id;
            this.readed = readed;
            this.subject = subject;
            this.body = body;
        }
    }
}
