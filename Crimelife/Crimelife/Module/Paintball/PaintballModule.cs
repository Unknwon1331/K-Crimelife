using GTANetworkAPI;
using GTANetworkMethods;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using ColShape = GTANetworkAPI.ColShape;
using Player = GTANetworkAPI.Player;
using TextLabel = GTANetworkAPI.TextLabel;

namespace Crimelife
{
    class PaintballModule : Crimelife.Module.Module<PaintballModule>
    {

        public static string newkills { get; private set; }

        protected override bool OnLoad()
        {

            NAPI.Blip.CreateBlip(432, new Vector3(437.18, -622.83, 28.71), 1.0f, 0, "Paintball", 255, 0, true, 0, 0);
            NAPI.Ped.CreatePed(NAPI.Util.GetHashKey("s_m_y_blackops_01"), new Vector3(437.18, -622.83, 28.71), 94, false, true, true, true, 0);
            TextLabel textLabel = NAPI.TextLabel.CreateTextLabel("~w~Paintball", new Vector3(437.18, -622.83, 28.71), 10.0f, 0.5f, 7, new Color(255, 255, 255));

            ColShape cb = NAPI.ColShape.CreateCylinderColShape(new Vector3(437.18, -622.83, 28.2), 1.4f, 1.4f, 0);
            cb.SetData("FUNCTION_MODEL", new FunctionModel("Paintball-Menu"));
            cb.SetData("MESSAGE", new Message("Benutze E um Painball zu spielen.", "[Paintball]", "orange", 3000));

            return true;
        }

        [RemoteEvent("Paintball-Menu")]
        public void PaintballMenu(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null) return;


                dbPlayer.TriggerEvent("Client:ffaBrowser:createBrowser");
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Paintball-Menu] " + ex.Message);
                Logger.Print("[EXCEPTION Paintball-Menu] " + ex.StackTrace);
            }
        }

        [RemoteEvent("Server:ffaBrowser:chooseFFA")]
        public void chooseFFA(Player player, int ffaid)
        {
            DbPlayer dbPlayer = player.GetPlayer();
            if (ffaid > 3)
            {
                dbPlayer.SendNotification("Du kannst nur in FFA 1 - 3", 3000, "grey");
                return;
            }

            if (dbPlayer.HasData("isInFFA"))
            {
                dbPlayer.SendNotification("Du bist bereits in einer FFA Zone!", 3000, "grey");
                return;
            }
            dbPlayer.player.RemoveAllWeapons();
            player.TriggerEvent("client:createKitBrowser", true);
            //Würfelpark
            if (ffaid == 1)
            {
                dbPlayer.SetData("isInFFA", true);
                dbPlayer.SetData("FFA1", true);
                dbPlayer.Dimension = 187;
                dbPlayer.SetData("PBZone", ffaid);
                dbPlayer.SetData("PBKills", 0);
                dbPlayer.SetData("PBDeaths", 0);
                dbPlayer.SetData("PBZoneplayer", 0);

                dbPlayer.SetArmor(100);

                dbPlayer.initializePaintball();

                dbPlayer.SendNotification("FFA Zone | Würfelpark", 3000, "grey");

                int rnd = new Random().Next(1, 120);

                if (rnd >= 3 && rnd <= 24)
                {
                    dbPlayer.Position = new Vector3(258.52747f, -875.9077f, 29.212402f);
                }
                else if (rnd >= 24 && rnd <= 48)
                {
                    dbPlayer.Position = new Vector3(218.42638f, -937.5165f, 24.140625f);
                }
                else if (rnd >= 48 && rnd <= 72)
                {
                    dbPlayer.Position = new Vector3(204.03957f, -993.7187f, 30.088623f);
                }
                else if (rnd >= 72 && rnd <= 96)
                {
                    dbPlayer.Position = new Vector3(207.53406f, -994.66825f, 29.279907f);
                }
                else if (rnd >= 96 && rnd <= 120)
                {
                    dbPlayer.Position = new Vector3(160.68132f, -999.0857f, 29.330444f);
                }
            }

            //Triaden
            if (ffaid == 2)
            {
                dbPlayer.SetData("isInFFA", true);
                dbPlayer.SetData("FFA2", true);
                dbPlayer.Dimension = 187;
                dbPlayer.SetData("PBZone", ffaid);
                dbPlayer.SetData("PBKills", 0);
                dbPlayer.SetData("PBDeaths", 0);
                dbPlayer.SetData("PBZoneplayer", 0);

                dbPlayer.SetArmor(100);

                dbPlayer.initializePaintball();

                dbPlayer.SendNotification("FFA Zone | Triaden beigetreten", 3000, "grey");

                int rnd = new Random().Next(1, 120);

                if (rnd >= 3 && rnd <= 24)
                {
                    dbPlayer.Position = new Vector3(1481.1019f, 1114.8245f, 114.33454f);
                }
                else if (rnd >= 24 && rnd <= 48)
                {
                    dbPlayer.Position = new Vector3(1481.1019f, 1114.8245f, 114.33454f);
                }
                else if (rnd >= 48 && rnd <= 72)
                {
                    dbPlayer.Position = new Vector3(1433.3723f, 1184.334f, 114.15171f);
                }
                else if (rnd >= 72 && rnd <= 96)
                {
                    dbPlayer.Position = new Vector3(1370.4058f, 1147.5156f, 113.75894f);
                }
                else if (rnd >= 96 && rnd <= 120)
                {
                    dbPlayer.Position = new Vector3(1406.6914f, 1117.7511f, 114.83774f);
                }
            }

            //Kirche
            if (ffaid == 3)
            {
                dbPlayer.SetData("isInFFA", true);
                dbPlayer.SetData("FFA3", true);
                dbPlayer.Dimension = 187;

                dbPlayer.SendNotification("FFA-Kirche", 3000, "grey");
                dbPlayer.SetData("PBZone", ffaid);
                dbPlayer.SetData("PBKills", 0);
                dbPlayer.SetData("PBDeaths", 0);
                dbPlayer.SetData("PBZoneplayer", 0);

                dbPlayer.SetArmor(100);

                dbPlayer.initializePaintball();

                int rnd = new Random().Next(1, 120);

                if (rnd >= 3 && rnd <= 24)
                {
                    dbPlayer.Position = new Vector3(-297.96716f, 2791.2646f, 60.21791f);
                }
                else if (rnd >= 24 && rnd <= 48)
                {
                    dbPlayer.Position = new Vector3(-352.33957f, 2776.2683f, 58.762897f);
                }
                else if (rnd >= 48 && rnd <= 72)
                {
                    dbPlayer.Position = new Vector3(-324.68524f, 2826.234f, 58.090813f);
                }
                else if (rnd >= 72 && rnd <= 96)
                {
                    dbPlayer.Position = new Vector3(-287.95786f, 2851.0635f, 53.976055f);
                }
                else if (rnd >= 96 && rnd <= 120)
                {
                    dbPlayer.Position = new Vector3(-270.33453f, 2802.7888f, 54.74635f);
                }
            }
        }

        public static void leavePaintball(Player c)
        {
            try
            {
                if (c == null || !c.Exists) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                    return;

                int ffaid = dbPlayer.GetData<int>("PBZone");
                if (ffaid == 8888 || c.Dimension != 187) return;

                dbPlayer.ACWait();

                dbPlayer.SetData("PBZone", null);
                dbPlayer.SetData("PBKills", 0);
                dbPlayer.SetData("PBDeaths", 0);
                dbPlayer.SetData("isInFFA", false);

                dbPlayer.finishPaintball();
                dbPlayer.SetArmor(0);
                dbPlayer.SetPosition(new Vector3(437.18, -623.83, 28.2));
                dbPlayer.player.RemoveAllWeapons();

                NAPI.Task.Run(() =>
                {
                    WeaponManager.loadWeapons(c);
                    Console.WriteLine("Load Weapons JA");
                    dbPlayer.SetDimension(0);
                    Console.WriteLine("Set Dimension 0 JA");
                    dbPlayer.SetData("isInFFA", false);
                    Console.WriteLine("Set FFA Zone 0 JA");
                }, 5000);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION leavePaintball] " + ex.Message);
                Logger.Print("[EXCEPTION leavePaintball] " + ex.StackTrace);
            }
        }

        public static void PaintballDeath(DbPlayer dbPlayer, DbPlayer dbPlayer2)
        {
            try
            {
                if (dbPlayer == null || dbPlayer2 == null) return;

                Random r = new Random();

                if (!dbPlayer.HasData("PBZone") || !dbPlayer.HasData("PBDeaths") || !dbPlayer.HasData("PBKills") || dbPlayer.GetData<int>("PBZone") == null && dbPlayer.GetData<int>("PBDeaths") == null || dbPlayer.GetData<int>("PBKills") == null) return;

                int ffaid = dbPlayer.GetData<int>("PBZone");
                int newdeaths = 1;
                if (!dbPlayer.HasData("PBDeaths") || dbPlayer.GetData<int>("PBDeaths") == null)
                {
                    dbPlayer.SetData("PBDeaths", 1);
                }
                else
                {
                    newdeaths = dbPlayer.GetData<int>("PBDeaths");
                    newdeaths += 1;
                }

                dbPlayer.SetData("PBKillstreak", 0);
                dbPlayer.SetData("PBDeaths", newdeaths);
                int rnd = new Random().Next(1, 120);
                switch (ffaid)
                {
                    case 1:

                        if (rnd >= 3 && rnd <= 24)
                        {
                            dbPlayer.Position = new Vector3(258.52747f, -875.9077f, 29.212402f);
                        }
                        else if (rnd >= 24 && rnd <= 48)
                        {
                            dbPlayer.Position = new Vector3(218.42638f, -937.5165f, 24.140625f);
                        }
                        else if (rnd >= 48 && rnd <= 72)
                        {
                            dbPlayer.Position = new Vector3(204.03957f, -993.7187f, 30.088623f);
                        }
                        else if (rnd >= 72 && rnd <= 96)
                        {
                            dbPlayer.Position = new Vector3(207.53406f, -994.66825f, 29.279907f);
                        }
                        else if (rnd >= 96 && rnd <= 120)
                        {
                            dbPlayer.Position = new Vector3(160.68132f, -999.0857f, 29.330444f);
                        }
                        break;
                    case 2:

                        if (rnd >= 3 && rnd <= 24)
                        {
                            dbPlayer.Position = new Vector3(1481.1019f, 1114.8245f, 114.33454f);
                        }
                        else if (rnd >= 24 && rnd <= 48)
                        {
                            dbPlayer.Position = new Vector3(1481.1019f, 1114.8245f, 114.33454f);
                        }
                        else if (rnd >= 48 && rnd <= 72)
                        {
                            dbPlayer.Position = new Vector3(1433.3723f, 1184.334f, 114.15171f);
                        }
                        else if (rnd >= 72 && rnd <= 96)
                        {
                            dbPlayer.Position = new Vector3(1370.4058f, 1147.5156f, 113.75894f);
                        }
                        else if (rnd >= 96 && rnd <= 120)
                        {
                            dbPlayer.Position = new Vector3(1406.6914f, 1117.7511f, 114.83774f);
                        }
                        break;
                    case 3:

                        if (rnd >= 3 && rnd <= 24)
                        {
                            dbPlayer.Position = new Vector3(-297.96716f, 2791.2646f, 60.21791f);
                        }
                        else if (rnd >= 24 && rnd <= 48)
                        {
                            dbPlayer.Position = new Vector3(-352.33957f, 2776.2683f, 58.762897f);
                        }
                        else if (rnd >= 48 && rnd <= 72)
                        {
                            dbPlayer.Position = new Vector3(-324.68524f, 2826.234f, 58.090813f);
                        }
                        else if (rnd >= 72 && rnd <= 96)
                        {
                            dbPlayer.Position = new Vector3(-287.95786f, 2851.0635f, 53.976055f);
                        }
                        else if (rnd >= 96 && rnd <= 120)
                        {
                            dbPlayer.Position = new Vector3(-270.33453f, 2802.7888f, 54.74635f);
                        }
                        break;
                }

                int newkills = dbPlayer2.GetData<int>("PBKills");
                int killstreak = 0;

                if (dbPlayer2.HasData("PBKillstreak") && dbPlayer2.GetData<int>("PBKillstreak") != null && dbPlayer2.GetData<int>("PBKillstreak") is int)
                    killstreak = dbPlayer2.GetData<int>("PBKillstreak");

                killstreak += 1;
                newkills += 1;

                if (killstreak == 3)
                {
                    dbPlayer.SendNotification("Du hast dir eine Streak aufgebaut", 3000, "grey");
                }

                else if (killstreak == 5)
                {
                    dbPlayer.SendNotification("Du hast deine Gegner eingeschüchtert", 3000, "grey");
                }

                else if (killstreak == 10)
                {
                    dbPlayer.SendNotification("Gnadenloser Killer", 3000, "grey");
                }

                dbPlayer2.SetData("PBKills", newkills);
                dbPlayer2.SetData("PBKillstreak", killstreak);

                //         dbPlayer.GiveWeapon(WeaponHash.Advancedrifle, 9999);
                //         dbPlayer.GiveWeapon(WeaponHash.Gusenberg, 9999);
                //         dbPlayer.GiveWeapon(WeaponHash.Heavypistol, 9999);
                //         dbPlayer.GiveWeapon(WeaponHash.Hatchet, 9999);

                dbPlayer.updatePaintballScore((int)dbPlayer.GetData<int>("PBKills"), (int)dbPlayer.GetData<int>("PBDeaths"));
                dbPlayer2.updatePaintballScore((int)dbPlayer2.GetData<int>("PBKills"), (int)dbPlayer2.GetData<int>("PBDeaths"));

                dbPlayer.StopAnimation();
                dbPlayer.SetInvincible(false);
                dbPlayer.SetArmor(100);
                dbPlayer.disableAllPlayerActions(false);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION PaintballDeath] " + ex.Message);
                Logger.Print("[EXCEPTION PaintballDeath] " + ex.StackTrace);
            }
        }

        public static void PaintballDeath2(DbPlayer dbPlayer)
        {
            try
            {
                if (dbPlayer == null) return;

                Random r = new Random();

                if (!dbPlayer.HasData("PBZone") || !dbPlayer.HasData("PBDeaths") || !dbPlayer.HasData("PBKills") || dbPlayer.GetData<int>("PBZone") == 8888 && dbPlayer.GetData<int>("PBDeaths") == 8888 || dbPlayer.GetData<int>("PBKills") == null) return;

                int paintballModel = dbPlayer.GetData<int>("PBZone");
                int newdeaths = 1;
                if (!dbPlayer.HasData("PBDeaths") || dbPlayer.GetData<int>("PBDeaths") == 8888)
                {
                    dbPlayer.SetData("PBDeaths", 1);
                }
                else
                {
                    newdeaths = dbPlayer.GetData<int>("PBDeaths");
                    newdeaths += 1;
                }

                dbPlayer.SetData("PBKillstreak", 0);
                dbPlayer.SetData("PBDeaths", newdeaths);
                int rnd = new Random().Next(1, 120);
                int ffaid = dbPlayer.GetData<int>("PBZone");
                switch (ffaid)
                {
                    case 1:

                        if (rnd >= 3 && rnd <= 24)
                        {
                            dbPlayer.Position = new Vector3(258.52747f, -875.9077f, 29.212402f);
                        }
                        else if (rnd >= 24 && rnd <= 48)
                        {
                            dbPlayer.Position = new Vector3(218.42638f, -937.5165f, 24.140625f);
                        }
                        else if (rnd >= 48 && rnd <= 72)
                        {
                            dbPlayer.Position = new Vector3(204.03957f, -993.7187f, 30.088623f);
                        }
                        else if (rnd >= 72 && rnd <= 96)
                        {
                            dbPlayer.Position = new Vector3(207.53406f, -994.66825f, 29.279907f);
                        }
                        else if (rnd >= 96 && rnd <= 120)
                        {
                            dbPlayer.Position = new Vector3(160.68132f, -999.0857f, 29.330444f);
                        }
                        break;
                    case 2:

                        if (rnd >= 3 && rnd <= 24)
                        {
                            dbPlayer.Position = new Vector3(1481.1019f, 1114.8245f, 114.33454f);
                        }
                        else if (rnd >= 24 && rnd <= 48)
                        {
                            dbPlayer.Position = new Vector3(1481.1019f, 1114.8245f, 114.33454f);
                        }
                        else if (rnd >= 48 && rnd <= 72)
                        {
                            dbPlayer.Position = new Vector3(1433.3723f, 1184.334f, 114.15171f);
                        }
                        else if (rnd >= 72 && rnd <= 96)
                        {
                            dbPlayer.Position = new Vector3(1370.4058f, 1147.5156f, 113.75894f);
                        }
                        else if (rnd >= 96 && rnd <= 120)
                        {
                            dbPlayer.Position = new Vector3(1406.6914f, 1117.7511f, 114.83774f);
                        }
                        break;
                    case 3:

                        if (rnd >= 3 && rnd <= 24)
                        {
                            dbPlayer.Position = new Vector3(-297.96716f, 2791.2646f, 60.21791f);
                        }
                        else if (rnd >= 24 && rnd <= 48)
                        {
                            dbPlayer.Position = new Vector3(-352.33957f, 2776.2683f, 58.762897f);
                        }
                        else if (rnd >= 48 && rnd <= 72)
                        {
                            dbPlayer.Position = new Vector3(-324.68524f, 2826.234f, 58.090813f);
                        }
                        else if (rnd >= 72 && rnd <= 96)
                        {
                            dbPlayer.Position = new Vector3(-287.95786f, 2851.0635f, 53.976055f);
                        }
                        else if (rnd >= 96 && rnd <= 120)
                        {
                            dbPlayer.Position = new Vector3(-270.33453f, 2802.7888f, 54.74635f);
                        }
                        break;
                }



                //         dbPlayer.GiveWeapon(WeaponHash.Advancedrifle, 9999);
                //          dbPlayer.GiveWeapon(WeaponHash.Gusenberg, 9999);
                //          dbPlayer.GiveWeapon(WeaponHash.Heavypistol, 9999);
                //          dbPlayer.GiveWeapon(WeaponHash.Hatchet, 9999);

                dbPlayer.updatePaintballScore((int)dbPlayer.GetData<int>("PBKills"), (int)dbPlayer.GetData<int>("PBDeaths"));

                dbPlayer.SendNotification("Du hast noch " + (10 - (int)dbPlayer.GetData<int>("PBDeaths")) + " Leben!", 3000, "#2f2f30");

                if (dbPlayer.GetData<int>("PBDeaths") >= 10) leavePaintball(dbPlayer.player);

                dbPlayer.StopAnimation();
                dbPlayer.SetInvincible(false);
                dbPlayer.SetArmor(100);
                dbPlayer.disableAllPlayerActions(false);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION PaintballDeath] " + ex.Message);
                Logger.Print("[EXCEPTION PaintballDeath] " + ex.StackTrace);
            }
        }
    }
}