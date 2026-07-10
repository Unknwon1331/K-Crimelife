using GTANetworkAPI;
using GVMP;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Crimelife
{
    public class Main : Script
    {
        public static int timeToRestart;
        public static List<Npc> ServerNpcs = new List<Npc>();

        public void InitGameMode()
        {

            NAPI.Server.SetAutoRespawnAfterDeath(false);
            NAPI.Server.SetCommandErrorMessage(" ");
            NAPI.Server.SetGlobalServerChat(false);
            NAPI.Server.SetAutoSpawnOnConnect(false);

            Modules.Instance.LoadAll();

            Logger.Print("");
            Logger.Print("");
            Logger.Print("     N E M E S I S    C R I M E L I F E       S T A R T E D    ");
            Logger.Print("");
            Logger.Print("");

            MySqlHandler.ExecuteSync(new MySqlQuery("UPDATE vehicles SET Parked = 1"));
            WebhookSender.SendMessage("-> " + NAPI.Server.GetServerName(), $"Server Load {NAPI.Server.GetServerName()} - {NAPI.Server.GetServerPort()} - {NAPI.Server.GetGamemodeName()} - {NAPI.Server.GetMaxPlayers()} ", Webhooks.Serverstatus, "Server Started");
            WebhookSender.SendMessage("Nemesis-Crimelife", "Der Server wird in Version 1.1 Gestartet", Webhooks.Serverstatus, "Server Started");
            Logger.Print("Parked all vehicles.");
        }

        public bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStartHandler()
        {
            InitGameMode();
            timeToRestart = 15;
            SyncThread.Init();
            SyncThread.Instance.Start();
            WebhookSender.SendMessage("Console", "" + timeToRestart + " -> " + DateTime.Now.ToString("HH':'mm':'ss"), Webhooks.Console, "OnResourceStartHandler");
        }

public static void OnHourHandler()
{
    try
    {
        foreach (DbPlayer dbPlayer in PlayerHandler.GetPlayers())
        {
            if (dbPlayer == null ||
                !dbPlayer.IsValid(true) ||
                dbPlayer.player == null)
            {
                continue;
            }

            int currentXp = dbPlayer.GetAttributeInt("XP") + 1;

            dbPlayer.SetAttribute("XP", currentXp);
            dbPlayer.XP = currentXp;

            if (currentXp >= dbPlayer.Level * 4)
            {
                dbPlayer.Level++;

                dbPlayer.SetAttribute("Level", dbPlayer.Level);

                dbPlayer.SendNotification(
                    $"Glückwunsch, Sie haben nun Level {dbPlayer.Level} erreicht!",
                    5000,
                    "yellow",
                    "Level aufgestiegen!"
                );

                dbPlayer.SendNotification(
                    $"Durch Ihr Level-up haben Sie Level {dbPlayer.Level} erreicht!",
                    5000,
                    "#2f2f30"
                );
            }

            House house = HouseModule.houses.FirstOrDefault(
                currentHouse =>
                    currentHouse.TenantsIds != null &&
                    currentHouse.TenantsIds.Contains(dbPlayer.Id)
            );

            if (house != null)
            {
                int rentPrice = 0;

                if (house.TenantPrices != null &&
                    house.TenantPrices.ContainsKey(dbPlayer.Id))
                {
                    rentPrice = Math.Max(
                        0,
                        house.TenantPrices[dbPlayer.Id]
                    );
                }

                if (rentPrice > 0)
                {
                    dbPlayer.removeMoney(rentPrice);

                    dbPlayer.SendNotification(
                        $"Dir wurde dein Mietpreis abgezogen! -{rentPrice.ToDots()}$"
                    );
                }
            }

            const int normalPayday = 250000;

            dbPlayer.addMoney(normalPayday);

            dbPlayer.SendNotification(
                $"Du hast deinen Payday erhalten +{normalPayday.ToDots()}$",
                5000,
                "darkgreen",
                "Kontoüberweisung"
            );

            int adminPermission =
                dbPlayer.Adminrank?.Permission ?? 0;

            if (adminPermission > 0 && adminPermission <= 91)
            {
                const int teamPayday = 350000;

                dbPlayer.addMoney(teamPayday);

                dbPlayer.SendNotification(
                    $"Teammitglied-Payday +{teamPayday.ToDots()}$",
                    5000,
                    "red",
                    "Kontoüberweisung"
                );
            }

            dbPlayer.RefreshData(dbPlayer);
        }
    }
    catch (Exception ex)
    {
        Logger.Print(
            "[EXCEPTION OnHourHandler] " + ex.Message
        );

        Logger.Print(
            "[EXCEPTION OnHourHandler] " + ex.StackTrace
        );
    }
}
