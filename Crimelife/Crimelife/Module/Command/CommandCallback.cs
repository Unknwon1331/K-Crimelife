using Crimelife;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crimelife.Module.Command
{
    class CommandCallback : Script
    {
        [RemoteEvent("revive")]
        public void revive(Player c, string name)
        {
            DbPlayer dbPlayer = c.GetPlayer();
            Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(name));
            DbPlayer dbPlayer2 = target.GetPlayer();

            if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
            {
                dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "[Admin]");
                return;
            }
            if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                return;

            Player client = dbPlayer2.player;


            dbPlayer2.SpawnPlayer(dbPlayer2.player.Position);
            dbPlayer2.disableAllPlayerActions(false);
            dbPlayer2.SetAttribute("Death", 0);
            dbPlayer2.StopAnimation();
            dbPlayer2.SetInvincible(false);
            dbPlayer2.DeathData = new DeathData
            {
                IsDead = false,
                DeathTime = new DateTime(0)
            };
            dbPlayer2.StopScreenEffect("DeathFailOut");
            dbPlayer.SendNotification("Du hast " + dbPlayer2.Name + " revived!", 5000, "red", "[Admin]");
            dbPlayer2.SendNotification("Du wurdest von " + dbPlayer.Adminrank.Name + " " + dbPlayer.Name + " revived!", 5000, "red", "[Admin]");
            WebhookSender.SendMessage("Spieler wird revived", "Der Spieler " + dbPlayer.Name + " hat den Spieler " + dbPlayer2.Name + " revived.", Webhooks.revivelogs, "Revive");


        }

        [RemoteEvent("kickplayer")]
        public void kickplayer(Player c, string name)
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
      /*          if (adminrank.Permission <= adminranks.Permission)
                {
                    dbPlayer.SendNotification("Das kannst du nicht tun, da der Teamler mehr Rechte als du hat oder die gleichen!", 5000, "red", "ADMIN");
                    return;
                }
                else*/
                {
                    Player client = dbPlayer2.player;

                    dbPlayer2.player.Kick();
                    dbPlayer.SendNotification("Spieler gekickt!", 5000, "red", "ADMIN");
                    Notification.SendGlobalNotification(dbPlayer.Adminrank.Name + " " + dbPlayer.Name + " hat " + dbPlayer2.Name + " vom Server gekickt!", 10000, "red", Notification.icon.warn);
                    // String.Join(" ", args).Replace("announce ", "")
        }
     
            }
        }
    }