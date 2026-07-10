using Crimelife;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crimelife.Module.Command
{
    public class CommandCallback2 : Script
    {
        [RemoteEvent("announce")]
        public void announce(Player c, string text)
        {
            DbPlayer dbPlayer = c.GetPlayer();
            Adminrank adminranks = dbPlayer.Adminrank;
            //  if (adminranks.Permission >= 11) return;

            Player client = dbPlayer.player;

            Notification.SendGlobalNotification(String.Join(adminranks.Name + " " + dbPlayer.player.Name + " ", text).Replace("announce ", ""), 10000, "white", Notification.icon.warn);


        }



        [RemoteEvent("car")]
        public static void car(Player c, string modelname)
        {
            DbPlayer dbPlayer = c.GetPlayer();
            Adminrank adminranks = dbPlayer.Adminrank;
            //  if (adminranks.Permission >= 11) return;

            Vehicle vehicle = NAPI.Vehicle.CreateVehicle(NAPI.Util.GetHashKey(modelname), c.Position, 0.0f, 0, 0, "", 255, false, true, c.Dimension);
            Logger.Print("CreateVeh " + modelname + " " + NAPI.Util.GetHashKey(modelname));
            c.SetIntoVehicle(vehicle, 0);
            vehicle.CustomPrimaryColor = dbPlayer.Adminrank.RGB;
            vehicle.CustomSecondaryColor = dbPlayer.Adminrank.RGB;
            vehicle.NumberPlate = ("" + dbPlayer.Name);
            dbPlayer.SendNotification("Du hast das Fahrzeug " + modelname + " erfolgreich gespawnt.", 5000, "red", "ADMIN");
            WebhookSender.SendMessage("Spieler spawnt Auto", "Der Spieler " + dbPlayer.Name + " hat das auto " + modelname + " ohne registrierung gespawnt", Webhooks.adminlogs, "Auto");


        }

        [RemoteEvent("banplayer")]
        public void bannplayer(Player c, string name)
        {
            DbPlayer dbPlayer = c.GetPlayer();
            Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(name));
            DbPlayer dbPlayer2 = target.GetPlayer();

            Adminrank adminrank = dbPlayer.Adminrank;
            Adminrank adminranks = dbPlayer2.Adminrank;

            if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
            {
                dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "ADMIN");
                return;
            }
            if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                if (adminrank.Permission <= adminranks.Permission)
                {
                    dbPlayer.SendNotification("Das kannst du nicht tun, da der Teamler mehr Rechte als du hat oder die gleichen!", 5000, "red", "ADMIN");
                    return;
                }
                else
                {
                    Player client = dbPlayer2.player;

                    BanExternal.BanPlayerU(dbPlayer2, dbPlayer.Name, dbPlayer2.Name);
                    dbPlayer2.player.Kick();
                    dbPlayer.SendNotification("Spieler gebannt!", 5000, "red", "ADMIN");
                    //Notification.SendGlobalNotification(dbPlayer.Adminrank.Name + " " + dbPlayer.Name + " hat " + dbPlayer2.Name + " vom Server gekickt!", 10000, "red", Notification.icon.warn);
                    // String.Join(" ", args).Replace("announce ", "")


                }

        }
    }
}