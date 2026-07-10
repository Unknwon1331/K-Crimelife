using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Crimelife
{
    class WebhookSender : Crimelife.Module.Module<WebhookSender>
    {
        public static async void SendMessage(string title, string msg, string webhook, string type)
        {
            try
            {
                DateTime now = DateTime.Now;
                string[] strArray = new string[20];
                strArray[0] = "{\"username\":\"Nemesis Crimelife\",\"avatar_url\":\"https://cdn.discordapp.com/attachments/827144560562929704/1136712941866061985/8fa027d12ec18ac6fcb4567523f64fe31.png\",\"content\":\"\",\"embeds\":[{\"author\":{\"name\":\"Nemesis Crimelife\",\"url\":\"https://discord.gg/kscripts\",\"icon_url\":\"https://cdn.discordapp.com/attachments/827144560562929704/1136712941866061985/8fa027d12ec18ac6fcb4567523f64fe31.png\"},\"title\":\"" + type + "\",\"thumbnail\":{\"url\":\"https://cdn.discordapp.com/attachments/827144560562929704/1136718787165565090/hud.png\"},\"url\":\"https://discord.gg/kscripts\",\"description\":\"Uhrzeit **";
                int num = now.Day;
                strArray[1] = num.ToString();
                strArray[2] = ".";
                num = now.Month;
                strArray[3] = num.ToString();
                strArray[4] = ".";
                num = now.Year;
                strArray[5] = num.ToString();
                strArray[6] = " | ";
                num = now.Hour;
                strArray[7] = num.ToString();
                strArray[8] = ":";
                num = now.Minute;
                strArray[9] = num.ToString();
                strArray[10] = "**\",\"color\":16098851,\"fields\":[{\"name\":\"";
                strArray[11] = title;
                strArray[12] = "\",\"value\":\"";
                strArray[13] = msg;
                strArray[14] = "\",\"inline\":true}],\"footer\":{\"text\":\" Bot by Nemesis-Crimelife\"}}]}";
                string stringPayload = string.Concat(strArray);
                StringContent httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(webhook, (HttpContent)httpContent);
                }
                stringPayload = (string)null;
                httpContent = (StringContent)null;
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION SendMessage] " + ex.Message);
                Logger.Print("[EXCEPTION SendMessage] " + ex.StackTrace);
            }
        }
    }
}
