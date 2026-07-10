using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife
{
    public class Logger : Script
    {
        public static void Print(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            // Console.Write("[+]");
            Console.Write($"[+] {DateTime.Now.ToString("HH':'mm':'ss")} ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(msg);
            WebhookSender.SendMessage("Console", "" + msg + " -> " + DateTime.Now.ToString("HH':'mm':'ss"), Webhooks.Console, "Logger Print");
        }

        [RemoteEvent("clientsidelog")]
        public void clientsidelog(Player player, string argument)
        {
            Print(argument);
            WebhookSender.SendMessage("Console", "" + argument + " -> " + DateTime.Now.ToString("HH':'mm':'ss"), Webhooks.Console, "clientsidelog?");
        }

    }
}

