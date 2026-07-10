using GTANetworkAPI;
using GVMP.Handlers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Crimelife.Handlers;
using Crimelife.Module.MAZ;
using Crimelife.Types;
using Crimelife.Module.Command;
using Org.BouncyCastle.Asn1.X509;

namespace Crimelife
{
    class CommandModule : Crimelife.Module.Module<CommandModule>
    {

        public static List<Faction> factionList = new List<Faction>();
        public static List<Command> commandList = new List<Command>();
        public static List<ClothingModel> clothingList = new List<ClothingModel>();

        protected override bool OnLoad()
        {
            commandList.Add(new Command((dbPlayer, args) =>
            {
                try
                {

                    Adminmenu(dbPlayer.player, "aduty");
                    Adminrank adminranks = dbPlayer.Adminrank;
                    if (adminranks.Permission >= 97)
                        dbPlayer.SendNotification("Das Teammitglied " + dbPlayer.Name + " ist nun im Aduty. (" + dbPlayer.Adminrank.Name + ")", 8000, "purple", "LOG");
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION ADUTY] " + ex.Message);
                    Logger.Print("[EXCEPTION ADUTY] " + ex.StackTrace);
                }
            }, "aduty", 91, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                try
                {

                    Adminrank adminranks = dbPlayer.Adminrank;
                    dbPlayer.TriggerEvent("adminclothes:open");

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION ADMINCLOTHES] " + ex.Message);
                    Logger.Print("[EXCEPTION ADMINCLOTHES] " + ex.StackTrace);
                }
            }, "adminclothes", 97, 0));

            /*commandList.Add(new Command((dbPlayer, args) =>
            {
                try
                {

                    Adminmenu(dbPlayer.player, "gawadmin");
                    Adminrank adminranks = dbPlayer.Adminrank;
                    if (adminranks.Permission >= 97)
                        dbPlayer.SendNotification("Das Teammitglied " + dbPlayer.Name + " hat sich alle waffen gegeben. (" + dbPlayer.Adminrank.Name + ")", 8000, "purple", "LOG");

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION ADUTY] " + ex.Message);
                    Logger.Print("[EXCEPTION ADUTY] " + ex.StackTrace);
                }
            }, "gaw", 96, 0));

            commandList.Add(new Command((p, args) =>
            {
                try
                {

                    Adminmenu(p.player, "rgawadmin");
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION ADUTY] " + ex.Message);
                    Logger.Print("[EXCEPTION ADUTY] " + ex.StackTrace);
                }
            }, "rgaw", 96, 0));*/


            commandList.Add(new Command((dbPlayer, args) =>
            {
                Adminrank adminranks = dbPlayer.Adminrank;

                if (adminranks.Permission <= 94)
                {
                    NativeMenu nativeMenu = new NativeMenu("Adminmenu", "🕵 K-Team", new List<NativeItem>()
                        {
                            new NativeItem("Aduty", "aduty"),
                            new NativeItem("Vanish", "vanish"),
                            new NativeItem("Revive Self", "revivemenu"),
                            new NativeItem("Revive Player", "revivemenuplayer"),
                        });
                    //nativeMenu.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                    dbPlayer.ShowNativeMenu(nativeMenu);
                }

                if (adminranks.Permission >= 98)
                {
                    NativeMenu nativeMenu2 = new NativeMenu("Adminmenu", "🕵 K-Highteam", new List<NativeItem>()
                        {
                            new NativeItem("Aduty", "aduty"),
                            new NativeItem("Vanish", "vanish"),
                            new NativeItem("Revive Self", "revivemenu"),
                            new NativeItem("Revive Player", "revivemenuplayer"),
                            new NativeItem("Announce", "announce"),
                            new NativeItem("Kick Player", "kickplayer"),
                            new NativeItem("Ban Player", "banplayer"),
                            new NativeItem("Teleport zum Marker", "tpm"),
                            new NativeItem("Spawn Vehicle", "car"),
                        });
                    dbPlayer.ShowNativeMenu(nativeMenu2);
                    //Adminrank adminranks = dbPlayer.Adminrank;
                    if (adminranks.Permission >= 97)
                        dbPlayer.SendNotification("Das Teammitglied " + dbPlayer.Name + " öffnet das Adminmenu. (" + dbPlayer.Adminrank.Name + ")", 8000, "purple", "LOG");
                }

            }, "menu", 94, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {

                foreach (DbPlayer player in PlayerHandler.GetPlayers())
                {
                    List<NativeItem> nativeItems = new List<NativeItem>();
                    List<string> strings = new List<string>();

                    NAPI.Pools.GetAllPlayers().ForEach(comp =>
                    {
                        if (!strings.Contains(comp.Name))
                        {
                            nativeItems.Add(new NativeItem("" + comp.Name + "", comp.Name));
                            strings.Add(comp.Name);
                        }
                    });
                    dbPlayer.ShowNativeMenu(new NativeMenu("k-players", "Online Spieler", nativeItems));
                }

            }, "players", 94, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                dbPlayer.SendNotification("Spieler: " + NAPI.Pools.GetAllPlayers().Count + " Insgesamt, Eingeloggt: " + PlayerHandler.GetPlayers().Count);
            }, "onlist", 0, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    Item item = ItemModule.itemRegisterList.Find((Item x) => x.Name == args[1]);
                    if (item == null) return;

                    dbPlayer.UpdateInventoryItems(item.Name, Convert.ToInt32(args[2]), false);
                    dbPlayer.SendNotification("Du hast dir das Item " + item.Name + " gegeben.", 3000, "darkred", "ADMIN");

                    Adminrank adminranks = dbPlayer.Adminrank;
                    if (adminranks.Permission >= 97)
                        dbPlayer.SendNotification("Das Teammitglied " + dbPlayer.Name + "  hat sich das item  " + item.Name + " " + args[2] + "x gegeben (" + dbPlayer.Adminrank.Name + ")", 8000, "purple", "LOG");

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION ADDITEM] " + ex.Message);
                    Logger.Print("[EXCEPTION ADDITEM] " + ex.StackTrace);
                }
            }, "additem", 95, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
                DbPlayer dbPlayer2 = target.GetPlayer();

                try
                {
                    if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                    {
                        dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red");
                        return;
                    }
                    Item item = ItemModule.itemRegisterList.FirstOrDefault((Item x) => x.Name == args[2]);
                    if (item == null) return;

                    dbPlayer2.UpdateInventoryItems(item.Name, Convert.ToInt32(1), false);
                    dbPlayer.SendNotification(target.Name + " kann nun in das Casino", 5000, "lightblue", "CASINO");
                    dbPlayer2.SendNotification("Du kannst nun in das Casino!", 5000, "lightblue", "CASINO");
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION CASINO] " + ex.Message);
                    Logger.Print("[EXCEPTION CASINO] " + ex.StackTrace);
                }
            }, "givecasino", 95, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                dbPlayer.SendNotification("Du hast nun einen Spawnschutz von 30 Sekunden! (" + dbPlayer.Name + ")", 3000);
                dbPlayer.player.SetSharedData("PLAYER_INVINCIBLE", true);
                //dbPlayer.SetData("PLAYER_ADUTY", true);
                NAPI.Task.Run(() =>
                {
                    dbPlayer.SendNotification("Du hast nun keinen Spawnschutz mehr!", 3000);
                    dbPlayer.player.SetSharedData("PLAYER_INVINCIBLE", false);
                    dbPlayer.ResetData("PLAYER_ADUTY");
                }, 30000);
            }, "testc", 0, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
                DbPlayer dbPlayer2 = target.GetPlayer();

                try
                {
                    if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                    {
                        dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red");
                        return;
                    }
                    Item item = ItemModule.itemRegisterList.FirstOrDefault((Item x) => x.Name == "CaillouCard");
                    if (item == null) return;

                    dbPlayer2.UpdateInventoryItems(item.Name, Convert.ToInt32(1), false);
                    dbPlayer.SendNotification(target.Name + " kann nun in das Casino", 5000, "lightblue", "CASINO");
                    dbPlayer2.SendNotification("Du kannst nun in das Casino!", 5000, "lightblue", "CASINO");
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION CASINO] " + ex.Message);
                    Logger.Print("[EXCEPTION CASINO] " + ex.StackTrace);
                }
            }, "casino", 96, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
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
                        client.TriggerEvent("openWindow", new object[2]
{
                                   "Kick",
                                    "{\"name\":\""+ dbPlayer2.Name +"\",\"grund\":\"" + String.Join(" ", args).Replace("kick " + args[1], "") +"\"}"
});
                        dbPlayer2.player.Kick();
                        dbPlayer.SendNotification("Spieler gekickt!", 5000, "red", "ADMIN");
                        Notification.SendGlobalNotification(dbPlayer.Adminrank.Name + " " + dbPlayer.Name + " hat " + dbPlayer2.Name + " vom Server gekickt! Grund: " + String.Join(" ", args).Replace("kick " + args[1], ""), 10000, "red", Notification.icon.warn);
                        // String.Join(" ", args).Replace("announce ", "")
                    }
            }, "kick", 1, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    Logger.Print("POS: " + dbPlayer.Position.X.ToString().Replace(",", ".") + ", " + dbPlayer.Position.Y.ToString().Replace(",", ".") + ", " + (dbPlayer.Position.Z - 1.1f).ToString().Replace(",", ".") + " ROT: " + dbPlayer.Heading.ToString().Replace(",", "."));
                    Logger.Print("{'x':" + dbPlayer.Position.X.ToString().Replace(",", ".") + ",'y':" + dbPlayer.Position.Y.ToString().Replace(",", ".") + ",'z':" + dbPlayer.Position.Z.ToString().Replace(",", ".") + "}");

                    dbPlayer.SendNotification("X: " + Math.Round(client.Position.X, 2) + " Y: " + Math.Round(client.Position.Y, 2) + " Z: " + Math.Round(client.Position.Z, 2) + " Heading: " + Math.Round(client.Heading, 2), 60000, "green", "");
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "pos", 91, 0));

            /*commandList.Add(new Command((dbPlayer, args) =>
            {
                if (dbPlayer.Faction.Id == 12)
                {
                    Player client = dbPlayer.player;
                    Notification.SendGlobalNotification("LSPD Nachricht : " + String.Join(" ", args).Replace("lspd ", ""), 10000, "red", Notification.icon.bullhorn);
                }
                else
                {
                    dbPlayer.SendNotification2("Du gehörst nicht zum LSPD");
                }


            }, "lspd", 0, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                if (dbPlayer.Faction.Id == 1005)
                {
                    Player client = dbPlayer.player;
                    Notification.SendGlobalNotification("FIB Nachricht : " + String.Join(" ", args).Replace("fib ", ""), 10000, "red", Notification.icon.bullhorn);
                }
                else
                {
                    dbPlayer.SendNotification2("Du gehörst nicht zum FIB");
                }


            }, "fib", 0, 1));*/

            //    commandList.Add(new Command((dbPlayer, args) => FactionBank.OpenFactionBank(dbPlayer), "frakbank", 0, 0));

            commandList.Add(new Command((dbPlayer, args) => BusinessBank.OpenBusinessBank(dbPlayer), "businessbank", 0, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;
                Notification.SendGlobalNotification("[AUTO-CLEAR] Es wurden alle Fahrzeuge eingeparkt!", 7500, "red", Notification.icon.bullhorn);
                NAPI.Pools.GetAllVehicles().ForEach((Vehicle veh) => veh.Delete());
                MySqlHandler.ExecuteSync(new MySqlQuery("UPDATE vehicles SET Parked = 1"));
            }, "parkall", 97, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                dbPlayer.SendNotification("Es sind aktuell " + NAPI.Pools.GetAllVehicles().Count + " Autos auf dem Server German Crimelife!");
            }, "carserver", 92, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;
                dbPlayer.SendNotification("Alter benutze -> F12.", 5000, dbPlayer.Adminrank.RGB.ToString(), "" + dbPlayer.Adminrank.Name);
            }, "announce", 94, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;
                Notification.SendGlobalNotification("Das Casino hat nun geöffnet, Tickets können vorort erworben werden!", 10000, "lightblue", Notification.icon.diamond);
            }, "casinoa", 96, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;
                Notification.SendGlobalNotification("Das Casino schließt nun, vielen dank für ihren Besuch!", 10000, "lightblue", Notification.icon.diamond);
            }, "casinoc", 96, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                dbPlayer.SendNotification("Zurzeit sind: " + PlayerHandler.GetAdminPlayers().Count + " Teammitglieder auf dem Gameserver!");
            }, "adminlist", 91, 0));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                string text = args[1];
                Player client = dbPlayer.player;
                Notification.SendGlobalNotification(String.Join("", "Die Fraktion " + text + " hat nun eine offene Bewerbungsphase! - 15 Minuten Safezone!").Replace("bwp ", ""), 10000, "white", Notification.icon.bullhorn);
                WebhookSender.SendMessage("Fraktion bewerbungs Phasen", dbPlayer.Name + " hat bwp gemacht " + text, Webhooks.adminlogs, "Bwp");
            }, "bwp", 94, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string text = args[1];
                Player client = dbPlayer.player;
                Notification.SendGlobalNotification(String.Join("", text + " Gangwar anfahren sonst Frakwarn").Replace("gw ", ""), 10000, "orange", Notification.icon.bullhorn);
            }, "gw", 94, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;
                DbVehicle dbVehicle = client.Vehicle.GetVehicle();
                if (!client.IsInVehicle) return;
                if (dbVehicle == null || !dbVehicle.IsValid() || dbVehicle.Vehicle == null)
                {
                    dbPlayer.SendNotification("Fahrzeug gelöscht!", 5000, "red", "ADMIN");
                    client.Vehicle.Delete();
                }
                else
                {
                    MySqlQuery mySqlQuery = new MySqlQuery("UPDATE vehicles SET Parked = 1 WHERE Id = @id");
                    mySqlQuery.AddParameter("@id", dbVehicle.Id);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    dbPlayer.SendNotification("Fahrzeug eingeparkt!", 5000, "red", "ADMIN");
                    client.Vehicle.Delete();
                }
            }, "dv", 1, 0));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;
                DbVehicle dbVehicle = client.Vehicle.GetVehicle();
                if (!client.IsInVehicle) return;
                Vehicle vehicle = client.Vehicle;
                dbVehicle.RefreshData(dbVehicle);
                vehicle.SetMod(Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
            }, "tuning", 98, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;
                dbPlayer.TriggerEvent("setRemoteHashKey", args);
            }, "testr", 99, 0));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;
                float dist = 0;
                bool dist2 = float.TryParse(args[1], out dist);

                if (!dist2) return;

                foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
                {
                    if (vehicle.Position.DistanceTo(dbPlayer.GetPosition()) <= dist)
                    {
                        vehicle.Delete();
                    }
                }
                dbPlayer.SendNotification("Fahrzeuge gelöscht!", 5000, "red", "ADMIN");
            }, "dvradius", 94, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    float x = 0;
                    float y = 0;
                    float z = 0;

                    bool x2 = float.TryParse(args[1], out x);
                    bool y2 = float.TryParse(args[2], out y);
                    bool z2 = float.TryParse(args[3], out z);
                    if (!x2 || !y2 || !z2) return;
                    dbPlayer.SetPosition(new Vector3(x, y, z));
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "tp", 1, 3));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
                    DbPlayer dbPlayer2 = target.GetPlayer();
                    if (dbPlayer2 == null)
                    {
                        dbPlayer.SendNotification("Kein Spieler gefunden!", 5000, "red", "ADMIN");
                        return;
                    }
                    dbPlayer.SetPosition(dbPlayer2.player.Position);
                    dbPlayer.SendNotification("Du hast dich zu " + target.Name + " teleportiert.", 5000, "red", "ADMIN");
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION GOTO] " + ex.Message);
                    Logger.Print("[EXCEPTION GOTO] " + ex.StackTrace);
                }
            }, "goto", 1, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    dbPlayer.SendNotification("Du hast den Adminshop refreshed.", 5000, "red", "ADMIN");
                    AdminShopModule.clothingList = ClothingManager.getClothingDataListAdmin();
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION refreshadmin] " + ex.Message);
                    Logger.Print("[EXCEPTION refreshadmin] " + ex.StackTrace);
                }
            }, "refreshadmin", 99, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    dbPlayer.SendNotification("Du hast den DonatorShop refreshed.", 5000, "red", "ADMIN");
                    DonatorShopModule.clothingList = ClothingManager.getClothingDataListAdmin();
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION refreshadmin] " + ex.Message);
                    Logger.Print("[EXCEPTION refreshadmin] " + ex.StackTrace);
                }
            }, "refreshshop", 99, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    string name = args[1];
                    int num = 0;
                    bool num2 = int.TryParse(args[2], out num);

                    DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                    if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                    {
                        dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "ADMIN");
                        return;
                    }
                    dbPlayer.SendNotification("Du hast " + name + " in die Dimension " + num + " gesetzt!", 5000, "red", "ADMIN");
                    dbPlayer2.SendNotification("Du wurdest in die Dimension " + num + " gesetzt!", 5000, "red", "ADMIN");
                    dbPlayer2.Dimension = num;
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION BRING] " + ex.Message);
                    Logger.Print("[EXCEPTION BRING] " + ex.StackTrace);
                }
            }, "dimension", 92, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
                    DbPlayer dbPlayer2 = target.GetPlayer();
                    if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                    {
                        dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "ADMIN");
                        return;
                    }
                    dbPlayer2.SetPosition(dbPlayer.player.Position);
                    dbPlayer.SendNotification("Du hast  " + target.Name + " zu dir teleportiert.", 5000, "red", "ADMIN");
                    dbPlayer2.SendNotification("Sie wurden von " + dbPlayer.Adminrank.Name + " " + dbPlayer.Name + " teleportiert!", 5000, "red", "ADMIN");
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION BRING] " + ex.Message);
                    Logger.Print("[EXCEPTION BRING] " + ex.StackTrace);
                }
            }, "bring", 1, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    if (!client.HasSharedData("PLAYER_INVISIBLE"))
                        return;

                    bool invisible = client.GetSharedData<bool>("PLAYER_INVISIBLE");
                    dbPlayer.SendNotification("Du hast dich " + (!invisible ? "unsichtbar" : "sichtbar") + " gemacht.", 5000, "red", "ADMIN");
                    client.SetSharedData("PLAYER_INVISIBLE", !invisible);

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION VANISH] " + ex.Message);
                    Logger.Print("[EXCEPTION VANISH] " + ex.StackTrace);
                }
            }, "v", 1, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    CommandCallback2.car(dbPlayer.player, args[1]);
                    Adminrank adminranks = dbPlayer.Adminrank;
                    if (adminranks.Permission >= 97)
                        dbPlayer.SendNotification("Das Teammitglied " + dbPlayer.Name + " hat das Auto " + args[1] + " gespawnt. (" + dbPlayer.Adminrank.Name + ")", 8000, "purple", "LOG");
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION VEH] " + ex.Message);
                    Logger.Print("[EXCEPTION VEH] " + ex.StackTrace);
                }
            }, "veh", 93, 1));
            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    CommandCallback2.car(dbPlayer.player, args[1]);
                    Adminrank adminranks = dbPlayer.Adminrank;
                    if (adminranks.Permission >= 97)
                        dbPlayer.SendNotification("Das Teammitglied " + dbPlayer.Name + " hat das Auto " + args[1] + " gespawnt. (" + dbPlayer.Adminrank.Name + ")", 8000, "purple", "LOG");
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION VEH] " + ex.Message);
                    Logger.Print("[EXCEPTION VEH] " + ex.StackTrace);
                }
            }, "car", 93, 1));


            commandList.Add(new Command(delegate (DbPlayer dbPlayer, string[] args)
            {
                if (dbPlayer.HasData("IN_HOUSE"))
                {
                    int num5 = dbPlayer.GetData<int>("IN_HOUSE");
                    if (num5 != 0)
                    {
                        House houseById = HouseModule.getHouseById(num5);
                        if (houseById.OwnerId != dbPlayer.Id && houseById.TenantsIds.Contains(dbPlayer.Id))
                        {
                            dbPlayer.Position = houseById.Entrance;
                            HouseModule.houses.Remove(houseById);
                            houseById.TenantsIds.Remove(dbPlayer.Id);
                            if (houseById.TenantPrices.ContainsKey(dbPlayer.Id))
                            {
                                houseById.TenantPrices.Remove(dbPlayer.Id);
                            }
                            HouseModule.houses.Add(houseById);
                            MySqlQuery mySqlQuery5 = new MySqlQuery("UPDATE houses SET TenantsId = @tenantsid, TenantPrices = @tenantprices WHERE Id = @id");
                            mySqlQuery5.AddParameter("@tenantsid", NAPI.Util.ToJson((object)houseById.TenantsIds));
                            mySqlQuery5.AddParameter("@tenantprices", NAPI.Util.ToJson((object)houseById.TenantPrices));
                            mySqlQuery5.AddParameter("@id", houseById.Id);
                            MySqlHandler.ExecuteSync(mySqlQuery5);
                            dbPlayer.SendNotification("Du hast den Mietvertrag verlassen!", 5000, "red");
                        }
                    }
                    else
                    {
                        dbPlayer.SendNotification("Du befindest dich nicht in einem Haus!");
                    }
                }
                else
                {
                    dbPlayer.SendNotification("Du befindest dich nicht in einem Haus!");
                }
            }, "cancelrental", 0, 0));
            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                if (args[1] == "cancel")
                {
                    TabletModule.Tickets.RemoveAll((Ticket ticket) => ticket.Creator == dbPlayer.Name);
                    dbPlayer.SendNotification("Du hast dein Ticket geschlossen!", 5000, "red");
                    return;
                }

                if (TabletModule.Tickets.Count >= 99)
                {
                    dbPlayer.SendNotification("Es sind bereits zu viele Tickets offen.", 5000, "yellow", "SUPPORT");
                    return;
                }


                if (TabletModule.Tickets.FirstOrDefault((Ticket ticket2) => ticket2.Creator == dbPlayer.Name) != null)
                {
                    dbPlayer.SendNotification("Du hast bereits ein offenes Ticket!", 5000, "yellow", "Support");
                    return;
                }
                if (String.Join(" ", args).Replace("support ", "").Length > 100)
                {
                    dbPlayer.SendNotification("Grund zu lang!", 5000, "yellow", "SUPPORT");
                    return;
                }

                var ticket = new Ticket
                {
                    Id = (int)new Random().Next(10000, 99999),
                    Created = DateTime.Now,
                    Creator = dbPlayer.Name,
                    Text = String.Join(" ", args).Replace("support ", "")
                };

                dbPlayer.SendNotification("Deine Nachricht wurde an die Administration gesendet! Benutze /support cancel um dein Ticket zu schließen!", 6000, "yellow", "SUPPORT");

                PlayerHandler.GetAdminPlayers().ForEach((DbPlayer dbPlayer2) =>
                {
                    if (dbPlayer2.HasData("disablenc")) return;
                    if (TabletModule.Tickets.Count >= 15)
                    {
                        dbPlayer2.SendNotification("Es sind " + TabletModule.Tickets.Count + " Support Tickets offen!", 6000, "red", "SUPPORT-WARNUNG");
                        if (dbPlayer2.HasData("PLAYER_ADUTY") && dbPlayer2.GetData<bool>("PLAYER_ADUTY") == true)
                            dbPlayer2.SendNotification(dbPlayer.Name + ": " + String.Join(" ", args).Replace("support ", "") + "", 5000, "yellow", "TICKET SYSTEM");
                    }
                    else
                    {
                        if (dbPlayer2.HasData("PLAYER_ADUTY") && dbPlayer2.GetData<bool>("PLAYER_ADUTY") == true)
                            dbPlayer2.SendNotification(dbPlayer.Name + ": " + String.Join(" ", args).Replace("support ", "") + "", 5000, "yellow", "TICKET SYSTEM");
                    }
                });
                TabletModule.Tickets.Add(ticket);
            }, "support", 0, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                if (dbPlayer.HasData("IN_HOUSE"))
                {
                    int houseId = dbPlayer.GetData<int>("IN_HOUSE");
                    if (houseId != 0)
                    {
                        House house = HouseModule.getHouseById(houseId);
                        if (house.OwnerId != dbPlayer.Id)
                        {
                            dbPlayer.SendNotification("Das ist nicht dein Haus!", 5000, "red");
                            return;
                        }
                        dbPlayer.Position = house.Entrance;

                        HouseModule.houses.Remove(house);
                        house.OwnerId = 0;
                        dbPlayer.Dimension = 0;
                        HouseModule.houses.Add(house);
                        dbPlayer.addMoney(house.Price / 2);
                        MySqlQuery mySqlQuery = new MySqlQuery("UPDATE houses SET OwnerId = @ownerid WHERE Id = @id");
                        mySqlQuery.AddParameter("@ownerid", 0);
                        mySqlQuery.AddParameter("@id", house.Id);
                        MySqlHandler.ExecuteSync(mySqlQuery);
                        dbPlayer.SendNotification("Du hast dein Haus verkauft! (" + house.Price / 2 + ")", 5000, "red", "");
                    }
                    else
                    {
                        dbPlayer.SendNotification("Du befindest dich nicht in einem Haus!");
                    }
                }
                else
                {
                    dbPlayer.SendNotification("Du befindest dich nicht in einem Haus!");
                }
            }, "sellhouse", 0, 0));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                dbPlayer.SendNotification("Lifeinvader gecleart!", 5000, "red", "SUPPORT");
                LifeInvaderModule.Advertisements.Clear();
                //  Module<ReversePhoneModule>.Instance.AddPhoneHistory(dbPlayer);
            }, "clearlifeinvader", 1, 0));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                if (String.Join(" ", args).ToLower().Contains("discord.gg/"))
                {
                    dbPlayer.SendNotification("Morgen");
                    return;
                }


                NAPI.ClientEvent.TriggerClientEventInRange(dbPlayer.player.Position, 100.0f, "sendPlayerNotification", String.Join(" ", args).Replace("ooc ", ""), 3500, "green", "OOC - (" + dbPlayer.Name + ")", "");
            }, "ooc", 0, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                PlayerHandler.GetAdminPlayers().ForEach((DbPlayer dbPlayer2) =>
                {
                    if (dbPlayer2.HasData("disablenc")) return;

                    Adminrank adminranks = dbPlayer2.Adminrank;

                    if (adminranks.Permission >= 91)
                        dbPlayer2.SendNotification(String.Join(" ", args).Replace("tc", ""), 6000, "red", "TEAMCHAT - (" + dbPlayer.Name + ")");
                });
            }, "tc", 91, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                PlayerHandler.GetAdminPlayers().ForEach((DbPlayer dbPlayer2) =>
                {
                    if (dbPlayer2.HasData("disablenc")) return;

                    Adminrank adminranks = dbPlayer2.Adminrank;

                    if (dbPlayer.Faction.Id == 0) return;
                    dbPlayer2.SendNotification(String.Join(" ", args).Replace("fc", ""), 6000, dbPlayer.Faction.GetRGBStr(), "Frakchat - (" + dbPlayer.Name + ")");
                });
            }, "fc", 0, 1));



            //if (dbPlayer.Faction.Id == 0) return;
            commandList.Add(new Command((dbPlayer, args) =>
            {
                if (dbPlayer.HasData("disablenc"))
                {
                    dbPlayer.SendNotification("Team Chat aktiviert", 5000, "red", "ADMIN");
                    dbPlayer.SendNotification("Reports aktiviert", 5000, "red", "ADMIN");
                    dbPlayer.SendNotification("Anticheat aktiviert", 5000, "red", "ADMIN");
                    dbPlayer.ResetData("disablenc");
                }
                else
                {
                    dbPlayer.SendNotification("Team Chat deaktivert", 5000, "red", "ADMIN");
                    dbPlayer.SendNotification("Reports deaktivert", 5000, "red", "ADMIN");
                    dbPlayer.SendNotification("Anticheat deaktiviert", 5000, "red", "ADMIN");
                    dbPlayer.SetData("disablenc", true);
                }
            }, "notify", 92, 0));            

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                string frak = args[2];
                string rang = args[3];


                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "ADMIN");
                    return;

                }
                if (Int64.Parse(rang) > 12)
                {
                    dbPlayer.SendNotification("Du kannst nur Ränge bis 12 vergeben!", 5000, "red", "ADMIN");
                    return;
                }
                int fraktion2 = dbPlayer2.Faction.Id;
                MySqlQuery mySqlQuery = new MySqlQuery($"SELECT COUNT(*) as Anzahl FROM accounts WHERE Fraktion = '{fraktion2}'");
                // MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                MySqlResult result = MySqlHandler.GetQuery(mySqlQuery);
                try
                {
                    if (result.Reader.HasRows)
                    {
                        result.Reader.Read();
                        int anzahl = result.Reader.GetInt32("Anzahl");
                        if (anzahl >= 25)
                        {
                            dbPlayer.SendNotification("Du kannst keinen mehr in deine Fraktion einladen, da das Limit bei 25 Mitglieder liegt!", 7000, "red");
                            return;
                        }
                        if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                        {
                            Player client = dbPlayer2.player;
                            Adminrank adminrank = dbPlayer.Adminrank;
                            Faction fraktion = FactionModule.getFactionById(Convert.ToInt32(frak));
                            dbPlayer2.SendNotification("Deine Fraktion wurde administrativ geändert! (" + fraktion.Name + ")", 5000, "red", "ADMIN");
                            dbPlayer.SendNotification("Du hast dem Spieler " + name + " die Fraktion " + fraktion.Name + " und den Rang " + rang + " gesetzt!", 5000, "red", "ADMIN");

                            dbPlayer2.SetAttribute("Fraktion", frak);
                            dbPlayer2.SetAttribute("Fraktionrank", rang);

                            dbPlayer2.TriggerEvent("updateTeamId", frak);
                            dbPlayer2.TriggerEvent("updateTeamRank", rang);
                            dbPlayer2.TriggerEvent("updateJob", fraktion.Name);
                            dbPlayer2.Faction = fraktion;
                            dbPlayer2.Factionrank = 0;
                            dbPlayer2.RefreshData(dbPlayer2);
                        }
                        result.Connection.Dispose();
                    }
                    //result.Connection.Dispose();//ggf crash
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION addPlayer] " + ex.Message);
                    Logger.Print("[EXCEPTION addPlayer] " + ex.StackTrace);
                }
            }, "setfrak", 95, 3));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                string fahrzeug = args[2];
                string nummernschild = args[3];

                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "ADMIN");
                    return;
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {
                    int id = new Random().Next(10000, 99999999);
                    Adminrank adminrank = dbPlayer.Adminrank;
                    dbPlayer2.SendNotification("Dir wurde das Fahrzeug " + fahrzeug + " mit dem Kennzeichen " + nummernschild + " gesetzt!", 5000, "red", "ADMIN");

                    dbPlayer.SendNotification("Du hast " + name + " das Fahrzeug " + fahrzeug + " mit dem Kennzeichen " + nummernschild + " gesetzt!", 5000, "red", "ADMIN");
                    List<int> list = new List<int>();
                    list.Add(dbPlayer2.Id);
                    MySqlQuery mySqlQuery = new MySqlQuery("INSERT INTO `vehicles` (`Id`, `Vehiclehash`, `Parked`, `OwnerId`, `Carkeys`, `Plate`) VALUES (@id, @vehiclehash, @parked, @ownerid, @carkeys, @plate)");
                    mySqlQuery.AddParameter("@vehiclehash", fahrzeug);
                    mySqlQuery.AddParameter("@parked", 1);
                    mySqlQuery.AddParameter("@ownerid", dbPlayer2.Id);
                    mySqlQuery.AddParameter("@carkeys", NAPI.Util.ToJson(list));
                    mySqlQuery.AddParameter("@plate", nummernschild);
                    mySqlQuery.AddParameter("@id", id);
                    MySqlHandler.ExecuteSync(mySqlQuery);
                }
            }, "givecar", 96, 3));


            commandList.Add(new Command(async (dbPlayer, args) =>
            {
                string name = args[1];
                string reason = "XCM";

                MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Username = @username LIMIT 1");
                mySqlQuery.AddParameter("@username", name);
                MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery);
                MySqlDataReader reader = mySqlResult.Reader;
                try
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        BanModule.BanIdentifier(name, reason, name);
                        BanModule.BanIdentifier(reader.GetString("Social"), reason, name);
                        dbPlayer.SendNotification("Du hast den Spieler " + name + " offline Gebannt!", 5000, "red", "ADMIN");
                    }
                    else
                    {
                        dbPlayer.SendNotification("Spieler wurde nicht gefunden!", 5000, "red", "ADMIN");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    await reader.DisposeAsync();
                    await mySqlResult.Connection.DisposeAsync();
                }
            }, "offxcm", 95, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                string kat = args[2];
                string com = args[3];
                string draw = args[4];
                string tex = args[5];


                int id = new Random().Next(10000, 99999999);
                dbPlayer.SendNotification("Hinzugefügt!", 5000, "red");
                MySqlQuery mySqlQuery = new MySqlQuery("INSERT INTO `adminclothes` (`Name`, `Category`, `Component`, `Drawable`, `Texture`, `Id`) VALUES (@name, @category, @component, @drawable, @texture, @id)");
                mySqlQuery.AddParameter("@name", name);
                mySqlQuery.AddParameter("@category", kat);
                mySqlQuery.AddParameter("@component", com);
                mySqlQuery.AddParameter("@drawable", draw);
                mySqlQuery.AddParameter("@texture", tex);
                mySqlQuery.AddParameter("@id", id);
                MySqlHandler.ExecuteSync(mySqlQuery);
            }, "addcloth", 96, 5));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                try
                {
                    if (ShopModule.shopList.ToList().Where(x => x.Position.DistanceTo2D(dbPlayer.Position) < 5f) == null)
                    {
                        dbPlayer.SendNotification("Du befindest dich in keinem Shop!", 3000, "green", "24/7");
                        return;
                    }


                    Shop shop = ShopModule.shopList.ToList().FirstOrDefault(x => x.Position.DistanceTo2D(dbPlayer.Position) < 10f);

                    Faction faction = FactionModule.getFactionByName("FIB");
                    Faction factionl = FactionModule.getFactionByName("PoliceDepartment");
                    Faction faction2 = FactionModule.getFactionById(31);

                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM shops WHERE Id = @userId LIMIT 1");
                    mySqlQuery.AddParameter("@userId", shop.Id);
                    MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery);

                    MySqlDataReader reader = mySqlResult.Reader;

                    reader.Read();

                    if (Constants.Shop.IsShopRobed)
                    {
                        dbPlayer.SendNotification("Es wird derzeit ein Laden überfallen", 3000, "green", "24/7");
                        return;
                    }

                    if ((DateTime)reader.GetMySqlDateTime("LastRob") > (DateTime)DateTime.Now)
                    {
                        dbPlayer.SendNotification("Der Laden wurde erst vor kurzem Ausgeraubt!", 3000, "green", "24/7");
                        reader.Dispose();
                        return;
                    }

                    if (dbPlayer.player.CurrentWeapon == WeaponHash.Unarmed || dbPlayer.player.CurrentWeapon == 0)
                    {
                        dbPlayer.SendNotification("Du benötigst eine Waffe in der Hand!", 3000, "green", "24/7");
                        return;
                    }

                    foreach (var x in PlayerHandler.GetFactionPlayers(faction))
                    {
                        if (x == null) continue;
                        x.SendNotification("Ein Shop wird überfallen, dieser wird auf der Karte Rot Makiert!", 7500, "green", "24/7");
                    }

                    foreach (var x in PlayerHandler.GetFactionPlayers(factionl))
                    {
                        if (x == null) continue;
                        x.SendNotification("Ein Shop wird überfallen, dieser wird auf der Karte Rot Makiert!", 7500, "green", "24/7");
                    }

                    foreach (var x in PlayerHandler.GetFactionPlayers(faction2))
                    {
                        if (x == null) continue;
                        x.SendNotification("Ein Shop wird überfallen, dieser wird auf der Karte Rot Makiert!", 7500, "green", "24/7");
                    }

                    dbPlayer.SendNotification("Du überfällst nun den Shop", 3000, "green", "24/7");
                    Constants.Shop.IsShopRobed = true;

                    Blip blip = NAPI.Pools.GetAllBlips().ToList().FirstOrDefault(x => x.HasData("24/7") && x.GetData<int>("24/7") == ShopModule.shopList.ToList().FirstOrDefault(x => x.Position.DistanceTo2D(dbPlayer.Position) < 5f).Id);
                    int oldcolor = blip.Color;
                    blip.Color = 1;

                    Vector3 oldpos = dbPlayer.Position;

                    NAPI.Task.Run(() =>
                    {
                        if (oldpos.DistanceTo2D(dbPlayer.Position) > 10f || dbPlayer.DeathData.IsDead)
                        {
                            dbPlayer.SendNotification("Überfall gescheitert!", 3000, "green", "24/7");
                            blip.Color = oldcolor;
                            MySqlQuery mySqlQuery = new MySqlQuery("UPDATE shops SET LastRob = @date WHERE id = @id");
                            mySqlQuery.AddParameter("@id", shop.Id);
                            mySqlQuery.AddParameter("@date", DateTime.Now.AddHours(1));
                            MySqlHandler.ExecuteSync(mySqlQuery);
                            Constants.Shop.IsShopRobed = false;
                            return;
                        }
                        blip.Color = oldcolor;

                        int money = new Random().Next(100000, 300000);
                        int moneyammunation = new Random().Next(100000, 200000);
                        int Waffenkisten = new Random().Next(2, 5);
                        dbPlayer.SendNotification($"Shop erfolgreich überfallen Beute:", 3000, "green", "24/7");

                        if (shop.Title.Contains("Ammunation"))
                        {
                            dbPlayer.SendNotification($"+ ${moneyammunation}", 3000, "green", "24/7");
                            dbPlayer.SendNotification($"+ {Waffenkisten} Waffenkisten", 3000, "green", "24/7");
                            dbPlayer.addMoney(moneyammunation);
                            dbPlayer.UpdateInventoryItems("Waffenkiste", Waffenkisten, false);
                        }
                        else
                        {
                            dbPlayer.SendNotification($"+ ${money}", 3000, "green", "24/7");
                            dbPlayer.addMoney(money);
                        }

                        MySqlQuery mySqlQuery1 = new MySqlQuery("UPDATE shops SET LastRob = @date WHERE id = @id");
                        mySqlQuery1.AddParameter("@id", shop.Id);
                        mySqlQuery1.AddParameter("@date", DateTime.Now.AddHours(2));
                        MySqlHandler.ExecuteSync(mySqlQuery1);
                        Constants.Shop.IsShopRobed = false;
                        reader.Dispose();
                    }, Constants.Shop.ShopopenTime);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }, "shoprob", 0, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string frakid = args[1];
                {
                    Adminrank adminrank = dbPlayer.Adminrank;
                    Faction fraktion = FactionModule.getFactionById(Convert.ToInt32(frakid));
                    dbPlayer.SendNotification("Die ID " + frakid + " gehört zu der Fraktion: " + fraktion.Name + "", 5000, "red", "ADMIN");
                }
            }, "frakinfo", 92, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
                DbPlayer dbPlayer2 = target.GetPlayer();
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "ADMIN");
                    return;
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {

                    Player client = dbPlayer2.player;
                    if ((int)dbPlayer2.GetAttributeInt("warns") == 0)
                    {
                        dbPlayer2.SendNotification("Das geht nicht, da der Spieler keine Warns hat!", 5000, "red", "ADMIN");
                        return;
                    }
                    else
                        dbPlayer2.SetAttribute("warns", (int)dbPlayer2.GetAttributeInt("warns") - 1);
                    dbPlayer2.RefreshData(dbPlayer2);
                    dbPlayer.SendNotification("Du hast den Spieler " + target.Name + " einen Warn entfernt!", 5000, "red", "ADMIN");
                    dbPlayer2.SendNotification("Dir wurde ein von " + dbPlayer.Name + " Warn entfernt!", 5000, "red", "ADMIN");
                }
            }, "delwarn", 91, 1));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
                DbPlayer dbPlayer2 = target.GetPlayer();
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red");
                    return;
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {
                    Player client = dbPlayer2.player;
                    Adminrank adminrank = dbPlayer.Adminrank;



                    dbPlayer2.disableAllPlayerActions(true);
                    dbPlayer2.RefreshData(dbPlayer2);
                    dbPlayer.SendNotification("Du hast " + dbPlayer2.Name + " geFreezed.", 5000, "red", "ADMIN");
                    dbPlayer2.SendNotification("Du wurdest von " + dbPlayer.Name + " geFreezed!", 5000, "red", "ADMIN");
                }
            }, "freeze", 92, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red");
                    return;
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {
                    Player client = dbPlayer2.player;
                    Adminrank adminrank = dbPlayer.Adminrank;


                    dbPlayer2.disableAllPlayerActions(false);
                    dbPlayer2.RefreshData(dbPlayer2);
                    dbPlayer.SendNotification("Du hast " + name + " geFreezed.", 5000, "red", "ADMIN");
                    dbPlayer2.SendNotification("Sie wurden von " + dbPlayer.Name + " entFreezed!", 5000, "red", "ADMIN");
                }
            }, "unfreeze", 92, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                int fraktion1 = FactionModule.factionList.Count;

                {
                    Faction fraktion = FactionModule.getFactionById(Convert.ToInt32(fraktion1));
                    if (fraktion.Name == "Zivilist")
                    {
                        return;
                    }
                    if (fraktion.Name != "Zivilist")
                    {
                        dbPlayer.SendNotification("Fraktionen: " + FactionModule.factionList.Count, 5000, "red");

                    }
                }
            }, "fraks", 91, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                int rang = 0;
                bool rang2 = int.TryParse(args[2], out rang);

                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "[Admin]");
                    return;
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {

                    Player client = dbPlayer2.player;

                    Adminrank adminrank = dbPlayer.Adminrank;
                    Adminrank adminranks = dbPlayer2.Adminrank;
                    if (adminrank.Permission <= rang)
                    {
                        dbPlayer.SendNotification("Deine Rechte reichen dafür nicht aus!", 5000, "red", "[Admin]");
                        return;
                    }
                    if (adminrank.Permission <= adminranks.Permission)
                    {
                        dbPlayer.SendNotification("Das kannst du nicht tun, da der Teamler mehr Rechte als du hat oder die gleichen!", 5000, "red", "[Admin]");
                        return;
                    }

                    dbPlayer2.SetAttribute("Adminrank", rang);
                    dbPlayer2.RefreshData(dbPlayer2);
                    adminranks = dbPlayer2.Adminrank;
                    Adminrank newadminrank = AdminrankModule.getAdminrank(rang);
                    dbPlayer.SendNotification("Du hast " + name + " den Rang " + newadminrank.Name + " gesetzt!", 5000, "red", "[Admin]");
                    dbPlayer2.SendNotification("Dein Team Rang wurde verändert (" + newadminrank.Name + ")", 5000, "red", "[Admin]");
                }
            }, "setperm", 96, 2));


            commandList.Add(new Command(delegate (DbPlayer dbPlayer, string[] args)
            {
                Player client2 = dbPlayer.player;
                StringBuilder stringBuilder = new StringBuilder();
                try
                {
                    {
                        string name = args[1];
                        DbPlayer player2 = PlayerHandler.GetPlayer(name);
                        if (player2 == null || !player2.IsValid(ignorelogin: true))
                        {
                            dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "[Admin]");
                        }
                        else
                        {
                            player2.RefreshData(player2);
                            player2.SetAttribute("Donator", 1);
                            dbPlayer.SendNotification("Donator gesetzt!", 5000, "red", "[Admin]");
                        }
                    }
                }
                catch (Exception ex2)
                {
                    Logger.Print("[EXCEPTION setdonator] " + ex2.Message);
                    Logger.Print("[EXCEPTION setdonator] " + ex2.StackTrace);
                }
            }, "setdonator", 96, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                string telefonnrm = args[2];

                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "[Admin]");
                    return;
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {

                    Player client = dbPlayer2.player;
                    MySqlQuery mySqlQuery = new MySqlQuery("UPDATE vehicles SET OwnerId = @neued WHERE OwnerId = @username");
                    mySqlQuery.AddParameter("@username", name);
                    mySqlQuery.AddParameter("@neued", telefonnrm);
                    MySqlHandler.ExecuteSync(mySqlQuery);
                    dbPlayer2.SetAttribute("Id", telefonnrm);

                    dbPlayer2.RefreshData(dbPlayer2);
                    dbPlayer.SendNotification("Du hast dem Spieler " + name + " die Telefonnummer " + telefonnrm + " gesetzt!", 5000, "red", "[Admin]");
                    dbPlayer2.SendNotification("Deine Telefonnummer wurde geändert! (" + telefonnrm + ")", 5000, "red", "ADMIN");
                }
            }, "changenumber", 96, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                string name2 = args[2];

                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red");
                    return;
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {

                    Player client = dbPlayer2.player;
                    dbPlayer2.SetAttribute("Username", name2);
                    dbPlayer2.RefreshData(dbPlayer2);
                    dbPlayer.SendNotification("Du hast dem Spieler " + name + " den Namen " + name2 + " gesetzt!", 5000, "red", "[Admin]");
                    dbPlayer2.SendNotification("Dein Name wurde geändert! (" + name2 + ")", 5000, "red", "[Admin]");
                }
            }, "rename", 96, 2));

            commandList.Add(new Command(async (dbPlayer, args) =>
            {
                string name = args[1];
                string name2 = args[2];

                MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Username = @username LIMIT 1");
                mySqlQuery.AddParameter("@username", name);
                MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery);
                MySqlDataReader reader = mySqlResult.Reader;
                try
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        MySqlQuery mySqlQuery2 = new MySqlQuery("UPDATE accounts SET Social = @social WHERE Username = @username");
                        mySqlQuery2.AddParameter("@social", name2);
                        mySqlQuery2.AddParameter("@username", name);
                        MySqlHandler.ExecuteSync(mySqlQuery2);
                        dbPlayer.SendNotification("Du hast " + name + " den Social Name auf " + name2 + " gesetzt!", 5000, "red", "[Admin]");
                    }
                    else
                    {
                        dbPlayer.SendNotification("Spieler wurde nicht gefunden!", 5000, "red", "[Admin]");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    await reader.DisposeAsync();
                    await mySqlResult.Connection.DisposeAsync();
                }
            }, "changesocial", 96, 2));

            commandList.Add(new Command(async (dbPlayer, args) =>
            {
                string name = args[1];
                MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Id = @username LIMIT 1");
                mySqlQuery.AddParameter("@username", Convert.ToInt32(name));
                MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery);
                MySqlDataReader reader = mySqlResult.Reader;
                try
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        dbPlayer.SendNotification("Der Spielername lautet:" + reader.GetString("Username"), 5000, "red", "[Admin]");
                    }
                    else
                    {
                        dbPlayer.SendNotification("Spieler wurde nicht gefunden!", 5000, "red", "ADMIN");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    await reader.DisposeAsync();
                    await mySqlResult.Connection.DisposeAsync();
                }
            }, "infonumber", 92, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
                DbPlayer dbPlayer2 = target.GetPlayer();
                string daysold = args[2];
                if (daysold == null) return;

                if (int.TryParse(daysold, out var days))
                {
                    if (dbPlayer2 == null) return;
                    Adminrank adminrank = dbPlayer.Adminrank;
                    if (adminrank == null) return;
                    Adminrank adminranks = dbPlayer2.Adminrank;
                    if (adminranks == null) return;

                    if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                        if (adminrank.Permission <= adminranks.Permission)
                        {
                            dbPlayer.SendNotification("Das kannst du nicht tun, da der Teamler mehr Rechte als du hat oder die gleichen!", 5000, "red", "[Admin]");
                            return;
                        }
                        else
                        {
                            BanExternal.TimeBanPlayer(dbPlayer2, days, dbPlayer.Name);
                            dbPlayer.SendNotification("Spieler gebannt!", 5000, "red", "[Admin]");
                        }
                }
            }, "timeban", 95, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];//

                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);

                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {

                    Player client = dbPlayer2.player;
                    dbPlayer2.RemoveAllWeapons();
                    dbPlayer2.RefreshData(dbPlayer2);
                    dbPlayer.SendNotification("Du hast von " + name + " das Waffenrad gecleart.", 5000, "red", "ADMIN");
                    dbPlayer2.SendNotification("Dein Waffenrad wurde gecleart! ", 5000, "red", "ADMIN");
                }
            }, "clearload", 96, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];

                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);

                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {

                    Player client = dbPlayer2.player;
                    dbPlayer2.RefreshData(dbPlayer2);
                    dbPlayer.SendNotification("Du hast den Char von  " + name + " neugeladen!", 5000, "red", "ADMIN");
                    dbPlayer.RefreshData(dbPlayer);
                    dbPlayer2.SpawnPlayer(dbPlayer.player.Position);
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
                    dbPlayer2.SendNotification("Dein Char wurde neugeladen! ", 5000, "red", "ADMIN");
                }
            }, "reloadchar", 96, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string str = args[1];
                if (str != "reset")
                {
                    dbPlayer.Name = str;
                    dbPlayer.RefreshData(dbPlayer);
                    dbPlayer.player.Name = (dbPlayer.Name);
                    dbPlayer.SendNotification("Fakename gesetzt! (" + str + ")", 5000, "red", "[Admin]");
                }
                else
                {
                    string altername = dbPlayer.GetAttributeString("Username");
                    dbPlayer.GetAttributeString("Username");
                    dbPlayer.player.Name = (altername);
                    dbPlayer.RefreshData(dbPlayer);
                    dbPlayer.SendNotification("Fakename zurückgesetzt! (" + altername + ")", 5000, "red", "[Admin]");
                }
            }, "fakename", 96, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string Id = args[1];

                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(Id);

                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {

                    Player client = dbPlayer2.player;
                    MySqlQuery mySqlQuery = new MySqlQuery("DELETE FROM inventorys WHERE Id = @id");
                    mySqlQuery.AddParameter("@id", Id);
                    MySqlHandler.ExecuteSync(mySqlQuery);
                    dbPlayer2.RefreshData(dbPlayer2);

                    dbPlayer.SendNotification("Du hast dem Spieler " + Id + " das Inventar zurückgesetzt!", 5000, "red", "ADMIN");
                    dbPlayer2.SendNotification("Dein Inventar wurde zurückgesetzt!! ", 5000, "red", "ADMIN");
                }
            }, "clearinv", 96, 1));


            commandList.Add(new Command((dbPlayer, args) =>
            {

                string reason = args[2];

                Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
                DbPlayer dbPlayer2 = target.GetPlayer();
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "ADMIN");
                    return;
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {

                    Player client = dbPlayer2.player;
                    dbPlayer2.SetAttribute("warns", (int)dbPlayer.GetAttributeInt("warns") + 1);
                    dbPlayer2.RefreshData(dbPlayer2);
                    dbPlayer.SendNotification("Du hast den Spieler " + target.Name + " verwarnt!", 5000, "red", "[Admin]");
                    dbPlayer2.SendNotification("Du wurdest von " + dbPlayer.Name + "verwarnt! Grund: " + reason + "", 5000, "red", "[Admin]");
                }
            }, "warn", 91, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
                DbPlayer dbPlayer2 = target.GetPlayer();
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "ADMIN");
                    return;
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {
                    Player client = dbPlayer2.player;
                    Adminrank adminrank = dbPlayer.Adminrank;

                    dbPlayer.RefreshData(dbPlayer);
                    dbPlayer.SendNotification("Warns von " + target.Name + ": " + dbPlayer2.warns + "", 10000, "red", "ADMIN");
                    dbPlayer.SendNotification("Social Name von " + target.Name + ": " + client.SocialClubName + "", 10000, "red", "ADMIN");
                    dbPlayer.SendNotification("Fraktion von " + target.Name + ": " + dbPlayer2.Faction.Name + "", 10000, "red", "ADMIN");
                    dbPlayer.SendNotification("Fraktion - Rang von " + target.Name + ": " + dbPlayer2.Factionrank + "", 10000, "red", "ADMIN");
                    dbPlayer.SendNotification("Business von " + target.Name + ": " + dbPlayer2.Business.Name + "", 10000, "red", "ADMIN");
                    dbPlayer.SendNotification("Geld von " + target.Name + ": " + dbPlayer2.Money + "", 10000, "red", "ADMIN");
                    dbPlayer.SendNotification("Level von " + target.Name + ": " + dbPlayer2.Level + "", 10000, "red", "ADMIN");
                    dbPlayer.SendNotification("ID von " + target.Name + ": " + dbPlayer2.Id + "", 10000, "red", "ADMIN");
                    dbPlayer.SendNotification("Online seit: " + dbPlayer2.OnlineSince + "", 10000, "red", "ADMIN");

                }
            }, "info", 91, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                string price = args[2];

                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "ADMIN");
                    return;
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {

                    Player client = dbPlayer2.player;
                    dbPlayer2.addMoney(Convert.ToInt32(price));
                    dbPlayer2.RefreshData(dbPlayer2);
                    dbPlayer.SendNotification("Du hast " + name + " Geld gegeben! (" + price + ")", 5000, "red", "[Admin]");
                    dbPlayer2.SendNotification("Dir wurde von " + dbPlayer.Name + " Geld gegeben (" + price + ")", 5000, "red", "[Admin]");
                }
            }, "addmoney", 96, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            //  NAPI.Pools.GetAllPlayers().ForEach(player => player.Client.addMoney(Convert.ToInt32(price)));
            //NAPI.Pools.GetAllPlayers().ForEach((Client client) =>
            {
                string price = args[1];
                dbPlayer.SendNotification("Du hast allen Spielern " + price + "$ gegeben! (" + NAPI.Pools.GetAllPlayers().Count + ")", 5000, "red", "ADMIN");
                foreach (Player target in NAPI.Pools.GetAllPlayers())
                {
                    if (target == null || target.Exists) continue;
                    DbPlayer dbPlayer2 = target.GetPlayer(); dbPlayer2.addMoney(Convert.ToInt32(price));
                    dbPlayer2.SendNotification("Von " + dbPlayer.Name + " +" + price + "$", 6000, "red", "ENTSCHÄDIGUNG/EVENT");
                }
            }, "eventmoney", 96, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                string price = args[2];

                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red");
                    return;
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {

                    Player client = dbPlayer2.player;
                    dbPlayer2.removeMoney(Convert.ToInt32(price));
                    dbPlayer2.RefreshData(dbPlayer2);
                    dbPlayer.SendNotification("Du hast " + name + " Geld entfernt! (" + price + ")", 5000, "red", "ADMIN");
                    dbPlayer2.SendNotification("Dir wurde von " + dbPlayer.Name + " Geld entfernt (" + price + ")", 5000, "red", "ADMIN");
                }
            }, "removemoney", 96, 2));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                string level = args[2];

                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red");
                    return;
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {

                    Player client = dbPlayer2.player;
                    dbPlayer2.SetAttribute("Level", level);
                    dbPlayer2.RefreshData(dbPlayer2);
                    dbPlayer.SendNotification("Du hast dem Spieler " + name + " das Level " + level + " gesetzt.", 5000, "red", "ADMIN");
                    dbPlayer2.SendNotification("Dein Level wurde auf " + level + " gesetzt!", 5000, "red", "ADMIN");
                }
            }, "changelevel", 96, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                string nachricht = args[2];

                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red");
                    return;
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {
                    Player client = dbPlayer2.player;
                    dbPlayer2.SendNotification(String.Join(" ", args).Replace("privat " + name, ""), 10000, "red", "[Admin]-PN - (" + dbPlayer.Name + ")");
                    dbPlayer.SendNotification("Privat Nachricht an " + name + " gesendet!", 5000, "red", "[Admin]");

                }
            }, "privat", 91, 2));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];

                MySqlQuery mySqlQuery = new MySqlQuery("DELETE FROM bans WHERE Account = @username");
                mySqlQuery.AddParameter("@username", name);
                MySqlHandler.ExecuteSync(mySqlQuery);
                BanModule.bans.RemoveAll(ban => ban.Account == name);
                BanModule.TimeBanIdentifier(DateTime.Now, name);
                dbPlayer.SendNotification("Spieler entbannt!", 5000, "red", "[Admin]");
                WebhookSender.SendMessage("Spieler wurde entbannt", "Der Spieler " + dbPlayer.Name + " hat den Spieler " + name + " entbannt.", Webhooks.unbanlogs, "Unban");
            }, "unban", 95, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];

                MySqlQuery mySqlQuery = new MySqlQuery("DELETE FROM accounts WHERE Username = @username");
                mySqlQuery.AddParameter("@username", name);
                MySqlHandler.ExecuteSync(mySqlQuery);

                dbPlayer.SendNotification("Account gelöscht!", 5000, "red", "ADMIN");
            }, "delacc", 96, 1));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];

                MySqlQuery mySqlQuery = new MySqlQuery("DELETE FROM accounts WHERE Social = @social");
                mySqlQuery.AddParameter("@social", name);
                MySqlHandler.ExecuteSync(mySqlQuery);

                dbPlayer.SendNotification("Account durch Socialclub-Namen gelöscht!", 5000, "red", "ADMIN");
            }, "delsacc", 96, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {

                NAPI.World.SetWeather(Weather.EXTRASUNNY);
                foreach (Player client in NAPI.Pools.GetAllPlayers())
                {
                    DbPlayer dbtarget = client.GetPlayer();

                    dbtarget.ResetData("SNOWWEATHER");
                }

                dbPlayer.SendNotification("Wetter gecleert!", 5000, "red", "ADMIN");
            }, "clearweather", 92, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {

                Saving.UpdateDbPositions();
            }, "test", 97, 0));
            //PlayerWorker.UpdateDbPositions();

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Weather weather = NAPI.World.GetWeather();

                if (weather == Weather.XMAS)
                {
                    NAPI.World.SetWeather(Weather.EXTRASUNNY);
                    dbPlayer.SendNotification("Sonnig", 5000, "red", "Admin");
                    foreach (Player client in NAPI.Pools.GetAllPlayers())
                    {
                        DbPlayer dbtarget = client.GetPlayer();

                        dbtarget.ResetData("SNOWWEATHER");

                        dbtarget.SendNotification("Schnee wurde Deaktiviert.", 5000, "Cyan", "Weihnachten");
                    }

                }
                else
                {
                    NAPI.World.SetWeather(Weather.XMAS);
                    dbPlayer.SendNotification("Schnee", 5000, "red", "ADMIN");
                    foreach (Player client in NAPI.Pools.GetAllPlayers())
                    {
                        DbPlayer dbtarget = client.GetPlayer();

                        dbtarget.SetData("SNOWWEATHER", true);

                        dbtarget.SendNotification("Schnee wurde Aktiviert.", 5000, "Cyan", "Weihnachten");
                    }
                }

            }, "snow", 100, 0));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                string Password = args[2];
                dbPlayer.SendNotification("Passwort geändert! (" + Password + ")", 3000, "red");
                using SHA256 sha256Hash = SHA256.Create();
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(Password));
                StringBuilder stringbuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    stringbuilder.Append(bytes[i].ToString("x2"));
                }
                MySqlQuery mySqlQuery = new MySqlQuery("UPDATE accounts SET Password = @password WHERE Username = @username");
                mySqlQuery.AddParameter("@password", stringbuilder.ToString());
                mySqlQuery.AddParameter("@username", name);
                MySqlHandler.ExecuteSync(mySqlQuery);

            }, "resetpw", 95, 1));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
                DbPlayer dbPlayer2 = target.GetPlayer();

                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "ADMIN");
                    return;
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {
                    // dbPlayer2.SetPosition(dbPlayer2.Faction.Storage);
                    string spawn = GetSpawn.Spawn(dbPlayer2.Name);
                    if (spawn == "0")
                    {
                        dbPlayer2.SpawnPlayer(new Vector3(298.08, -584.53, 43.26));
                    }

                    else if (spawn == "1")
                    {
                        if (dbPlayer2.Faction.Id == 0)
                        {
                            dbPlayer2.SpawnPlayer(new Vector3(298.08, -584.53, 43.26));
                        }
                        else
                        {
                            dbPlayer2.SpawnPlayer(dbPlayer2.Faction.Spawn);
                        }
                    }

                    else if (spawn == "2")
                    {
                        House house = HouseModule.houses.FirstOrDefault((House house2) => house2.OwnerId == dbPlayer2.Id);
                        if (house == null)
                        {
                            return;
                        }
                        else
                        {
                            dbPlayer2.SpawnPlayer(house.Entrance);
                        }
                    }
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
                    dbPlayer.SendNotification("Du hast " + dbPlayer2.Name + " respawnt.", 5000, "red", "ADMIN");
                    dbPlayer2.SendNotification(dbPlayer.Adminrank.Name + " " + dbPlayer.Name + " hat dich respawnt!", 5000, "red", "");
                }
            }, "respawn", 94, 1));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                int slot = 0;
                bool slot2 = int.TryParse(args[2], out slot);
                int drawable = 0;
                bool drawable2 = int.TryParse(args[3], out drawable);
                int texture = 0;
                bool texture2 = int.TryParse(args[4], out texture);

                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);

                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {
                    dbPlayer2.RefreshData(dbPlayer2);
                    dbPlayer2.SetClothes(slot, drawable, texture);
                    dbPlayer.SendNotification("Kleidungsstück geändert zu " + slot + " " + drawable + " " + texture + " ", 5000, "red", "ADMIN");
                }
            }, "aclothes", 92, 4));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string collection = args[1];
                string overlay = args[2];
                Player p;
                Decoration data = new Decoration();
                data.Collection = NAPI.Util.GetHashKey(collection);//"mpchristmas2_overlays"
                data.Overlay = NAPI.Util.GetHashKey(overlay);//"MP_Xmas2_M_Tat_005"
                dbPlayer.SendNotification("" + data, 20000, "red");
                dbPlayer.player.SetDecoration(data);
            }, "atattoos", 92, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string hash = args[1];
                string hash2 = args[2];
                Player p;
                dbPlayer.SendNotification("1: " + NAPI.Util.GetHashKey(hash), 5000, "red", "ADMIN");
                dbPlayer.SendNotification("2: " + NAPI.Util.GetHashKey(hash2), 5000, "red", "ADMIN");
            }, "tattoo", 96, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];

                MySqlQuery mySqlQuery = new MySqlQuery("UPDATE vehicles SET Parked = 1 WHERE OwnerId = @username");
                mySqlQuery.AddParameter("@username", name);
                MySqlHandler.ExecuteSync(mySqlQuery);

                dbPlayer.SendNotification("Du hast alle Fahrzeuge von " + name + " eingeparkt!", 5000, "red", "ADMIN");
            }, "parkcars", 95, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                if (int.TryParse(args[1], out var result))
                {
                    if (dbPlayer.OwnVehicles.ContainsKey(result))
                    {
                        VehicleKeyHandler.Instance.DeleteAllVehicleKeys(result);
                        dbPlayer.SendNotification("Schlüssel von dem Fahrzeug wurden gecleart.", 5000, "red", "FAHRZEUG");
                    }
                    else
                    {
                        dbPlayer.SendNotification("Fahrzeug nicht in deinem Besitz.", 5000, "red", "FEHLER!");
                    }
                }

            }, "clearvehiclekeys", 0, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                if (dbPlayer.GetHouse() != null && dbPlayer.GetHouse().OwnerId == dbPlayer.Id)
                {
                    HouseKeyHandler.Instance.DeleteAllHouseKeys(dbPlayer.GetHouse());
                    dbPlayer.SendNotification("Schlüssel von dem Haus wurden gecleart.", 3500, "red");
                }
                else
                {
                    dbPlayer.SendNotification("Haus nicht in deinem Besitz.", 3500, "red", "FEHLER!");
                }

            }, "clearhousekeys", 0, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                if (dbPlayer.player.IsInVehicle)
                {
                    dbPlayer.player.Vehicle.Repair();
                }
            }, "repair", 95, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                if (args[1] == "1")
                {
                    dbPlayer.SendNotification("test", 3500, "dodgerblue");
                    dbPlayer.PlayAnimation(33, "mp_arresting", "a_uncuff");
                }
                else if (args[1] == "2")
                {
                    dbPlayer.PlayAnimation(33, "mp_arresting", "b_uncuff");
                }
                else if (args[1] == "3")
                {
                    dbPlayer.PlayAnimation(33, "mp_arrest_paired", "crook_p2_back_right");
                }
            }, "an", 97, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                BanModule.Instance.Load(true);

                dbPlayer.SendNotification("Bans neu geladen!", 5000, "red", "ADMIN");
            }, "reloadbans", 94, 0));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
                DbPlayer dbPlayer2 = target.GetPlayer();

                if (args[1] == "me")
                {
                    dbPlayer.SpawnPlayer(dbPlayer.player.Position);
                    dbPlayer.disableAllPlayerActions(false);
                    dbPlayer.SetAttribute("Death", 0);
                    dbPlayer.StopAnimation();
                    dbPlayer.SetInvincible(false);
                    dbPlayer.DeathData = new DeathData
                    {
                        IsDead = false,
                        DeathTime = new DateTime(0)
                    };
                    dbPlayer.StopScreenEffect("DeathFailOut");
                    dbPlayer.SendNotification("Du hast dich selber revived!", 5000, "red", "[Admin]");
                    WebhookSender.SendMessage("Spieler wird revived", "Der Spieler " + dbPlayer.Name + " hat sich selber revived.", Webhooks.revivelogs, "Revive");
                    return;
                }

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
            }, "revive", 95, 1));

            commandList.Add(new Command((dbPlayer, args) => PaintballModule.leavePaintball(dbPlayer.player), "quitffa", 0, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {

                    var random = new Random();
                    int id = random.Next(10000, 99999);
                    int price = 0;
                    bool price2 = int.TryParse(args[1], out price);
                    string entrance = NAPI.Util.ToJson(client.Position);
                    int classid = 0;
                    bool classid2 = int.TryParse(args[2], out classid);

                    if (!classid2) return;

                    MySqlQuery mySqlQuery = new MySqlQuery("INSERT INTO houses (Id, Price, Entrance, ClassId) VALUES (@id, @price, @entrance, @classid)");
                    mySqlQuery.AddParameter("@id", id);
                    mySqlQuery.AddParameter("@price", price);
                    mySqlQuery.AddParameter("@entrance", entrance);
                    mySqlQuery.AddParameter("@classid", classid);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    dbPlayer.SendNotification("An deiner Position wurde erfolgreich ein Haus gesetzt. ID: " + id, 5000, "red", "ADMIN");

                    NAPI.Marker.CreateMarker(1, client.Position, new Vector3(), new Vector3(), 1.0f, new Color(255, 140, 0), false, 0);
                    NAPI.Blip.CreateBlip(40, client.Position, 1f, 0, "Haus " + id, 255, 0.0f, true, 0, 0);
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "sethouse", 99, 2));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {

                    var random = new Random();
                    int id = random.Next(10000, 99999);

                    int price = 15000;

                    int minprice = 15000;

                    int maxprice = 350000;

                    int pricestep = 5000;

                    int maxmultiple = 3;

                    int radius = 3;

                    string pos_x = NAPI.Util.ToJson(client.Position.X);

                    string pos_y = NAPI.Util.ToJson(client.Position.Y);

                    string pos_z = NAPI.Util.ToJson(client.Position.Z);

                    MySqlQuery mySqlQuery = new MySqlQuery("INSERT INTO kasino_devices (id, price, minprice, maxprice, pricestep, maxmultiple, pos_x, pos_y, pos_z, radius) VALUES (@id, @price, @minprice, @maxprice, @pricestep, @maxmultiple, @pos_x, @pos_y, @pos_z, @radius)");
                    mySqlQuery.AddParameter("@id", id);
                    mySqlQuery.AddParameter("@price", price);
                    mySqlQuery.AddParameter("@minprice", minprice);
                    mySqlQuery.AddParameter("@maxprice", maxprice);
                    mySqlQuery.AddParameter("@pricestep", pricestep);
                    mySqlQuery.AddParameter("@maxmultiple", maxmultiple);
                    mySqlQuery.AddParameter("@pos_x", pos_x);
                    mySqlQuery.AddParameter("@pos_y", pos_y);
                    mySqlQuery.AddParameter("@pos_z", pos_z);
                    mySqlQuery.AddParameter("@radius", radius);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    dbPlayer.SendNotification("An deiner Position wurde erfolgreich ein Casino Automat gesetzt. ID: " + id);

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "setcasino", 99, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {

                    var random = new Random();
                    int id = random.Next(10000, 99999);

                    string name = args[1];

                    string shortname = args[2];

                    string spawn = NAPI.Util.ToJson(client.Position);

                    int dimension = random.Next(10000, 99999);

                    string blip = args[3];

                    int rgbfarbe = 0;
                    bool rgbfarbe22 = int.TryParse(args[4], out rgbfarbe);
                    int rgbfarbe2 = 0;
                    bool rgbfarbe33 = int.TryParse(args[5], out rgbfarbe2);
                    int rgbfarbe3 = 0;
                    bool rgbfarbe44 = int.TryParse(args[6], out rgbfarbe3);
                    string rgbf = NAPI.Util.ToJson(new Color(rgbfarbe, rgbfarbe2, rgbfarbe3));

                    MySqlQuery mySqlQuery = new MySqlQuery("INSERT INTO fraktionen (Id, Name, Short, Spawn, Storage, Dimension, Blip, RGB) VALUES (@id, @name, @short, @spawn, @storage, @dimension, @blip, @rgb)");
                    mySqlQuery.AddParameter("@id", id);
                    mySqlQuery.AddParameter("@name", name);
                    mySqlQuery.AddParameter("@short", shortname);
                    mySqlQuery.AddParameter("@spawn", spawn);
                    mySqlQuery.AddParameter("@storage", spawn);
                    mySqlQuery.AddParameter("@dimension", dimension);
                    mySqlQuery.AddParameter("@blip", blip);
                    mySqlQuery.AddParameter("@rgb", rgbf);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    dbPlayer.SendNotification("Fraktion Erstellt! Fraktions-ID " + id, 10000, "red", "ADMIN"); ;

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "cfrak", 96, 6));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    var random = new Random();
                    int id = random.Next(10000, 99999);

                    string fid = args[1];
                    string name = args[2];
                    string spawn = NAPI.Util.ToJson(client.Position);

                    MySqlQuery mySqlQuery = new MySqlQuery("INSERT INTO fahrzeugshops_frak (Id, FactionId, Name, Position) VALUES (@Id, @FactionId, @Name, @Position)");
                    mySqlQuery.AddParameter("@Id", id);
                    mySqlQuery.AddParameter("@FactionId", fid);
                    mySqlQuery.AddParameter("@Name", name);
                    mySqlQuery.AddParameter("@Position", spawn);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    dbPlayer.SendNotification("Fraktion Veh Shop Erstellt! FVehShop-ID " + id, 10000, "red", "ADMIN"); ;

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "createfshop", 96, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    var random = new Random();
                    int id = random.Next(10000, 99999);

                    string name = args[1];

                    string hash = args[2];

                    string collection = args[3];

                    string zone = args[4];

                    string price = args[5];

                    MySqlQuery mySqlQuery = new MySqlQuery("INSERT INTO assets_tattoo (Id, name, hash_male, hash_female, collection, zone_id, price) VALUES (@id, @name, @hash_male, @hash_female, @collection, @zone_id, @price)");
                    mySqlQuery.AddParameter("@id", id);
                    mySqlQuery.AddParameter("@name", name);
                    mySqlQuery.AddParameter("@hash_male", hash);
                    mySqlQuery.AddParameter("@hash_female", hash);
                    mySqlQuery.AddParameter("@collection", collection);
                    mySqlQuery.AddParameter("@zone_id", zone);
                    mySqlQuery.AddParameter("@price", price);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    dbPlayer.SendNotification("Tattoo hinzugefügt!", 5000, "red", "ADMIN");

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "addtattoo", 96, 5));            

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    var random = new Random();
                    int id = random.Next(10000, 99999);

                    string factionid = args[1];

                    string model = args[2];

                    int primary = 0;
                    bool primary2 = int.TryParse(args[3], out primary);

                    int secondary = 0;
                    bool secondary2 = int.TryParse(args[4], out secondary);

                    int headlight = 0;
                    bool headlight2 = int.TryParse(args[5], out headlight);

                    Faction fraktion = FactionModule.getFactionById(Convert.ToInt32(factionid));
                    MySqlQuery mySqlQuery = new MySqlQuery("INSERT INTO fraktion_vehicles (Id, FactionId, Model, PrimaryColor, SecondaryColor, HeadlightColor) VALUES (@id, @factionid, @model, @primary, @secondary, @headlight)");
                    mySqlQuery.AddParameter("@id", id);
                    mySqlQuery.AddParameter("@factionid", factionid);
                    mySqlQuery.AddParameter("@model", model);
                    mySqlQuery.AddParameter("@primary", primary);
                    mySqlQuery.AddParameter("@secondary", secondary);
                    mySqlQuery.AddParameter("@headlight", headlight);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    dbPlayer.SendNotification("Fraktions Fahrzeug für " + fraktion.Name + " erstellt!", 5000, "red", "ADMIN");

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "cfrakcar", 97, 4));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    string frakid = args[1]; //41142
                    int rgbfarbe = 0;
                    bool rgbfarbe22 = int.TryParse(args[2], out rgbfarbe);
                    int rgbfarbe2 = 0;
                    bool rgbfarbe33 = int.TryParse(args[3], out rgbfarbe2);
                    int rgbfarbe3 = 0;
                    bool rgbfarbe44 = int.TryParse(args[4], out rgbfarbe3);
                    string rgbf = NAPI.Util.ToJson(new Color(rgbfarbe, rgbfarbe2, rgbfarbe3));


                    MySqlQuery mySqlQuery = new MySqlQuery("UPDATE fraktionen SET RGB = @rgb WHERE Id = @id");
                    mySqlQuery.AddParameter("@rgb", rgbf);
                    mySqlQuery.AddParameter("@id", frakid);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    dbPlayer.SendNotification("Fraktionsgaragen Spawnpoint gesetzt!", 5000, "red", "ADMIN");

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "changefrakcolor", 98, 3));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {

                    string dieid = args[1];
                    string garage = NAPI.Util.ToJson(client.Position);



                    MySqlQuery mySqlQuery = new MySqlQuery("UPDATE fraktionen SET Garage = @garage WHERE Id = @id");
                    mySqlQuery.AddParameter("@garage", garage);
                    mySqlQuery.AddParameter("@id", dieid);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    dbPlayer.SendNotification("Fraktionsgarage gesetzt!", 5000, "red", "ADMIN");

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "setfrakgarage", 98, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    dbPlayer.TriggerEvent("client:createKitBrowser", false);
                    dbPlayer.SendNotification("Test Ui!", 5000, "red", "ADMIN");

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "testui", 98, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {

                    string dieid = args[1];
                    string garage = NAPI.Util.ToJson(client.Position);
                    string garage2 = NAPI.Util.ToJson(client.Heading);



                    MySqlQuery mySqlQuery = new MySqlQuery("UPDATE fraktionen SET GarageSpawn = @garage, GarageSpawnRotation = @garagr2 WHERE Id = @id");
                    mySqlQuery.AddParameter("@garage", garage);
                    mySqlQuery.AddParameter("@id", dieid);
                    mySqlQuery.AddParameter("@garagr2", garage2);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    dbPlayer.SendNotification("Fraktionsgaragen Spawnpoint gesetzt!", 5000, "red", "ADMIN");

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "setfrakgaragspawn", 98, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {

                    string frakid = args[1];
                    string logo = args[2];



                    MySqlQuery mySqlQuery = new MySqlQuery("UPDATE fraktionen SET Logo = @logo WHERE Id = @id");
                    mySqlQuery.AddParameter("@logo", logo);
                    mySqlQuery.AddParameter("@id", frakid);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    dbPlayer.SendNotification("Fraktionsgaragen Spawnpoint gesetzt!", 5000, "red", "ADMIN");

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "changefraklogo", 98, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {


                    string dieid = args[1];
                    string garage = NAPI.Util.ToJson(client.Position);
                    string garage2 = NAPI.Util.ToJson(client.Heading);



                    MySqlQuery mySqlQuery = new MySqlQuery("UPDATE garages SET CarPoint2 = @garage, Rotation2 = @garagr2 WHERE Id = @id");
                    mySqlQuery.AddParameter("@garage", garage);
                    mySqlQuery.AddParameter("@id", dieid);
                    mySqlQuery.AddParameter("@garagr2", garage2);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    dbPlayer.SendNotification("Spawntpoint2 für die Garage mit der ID: " + dieid + " gesetzt!", 5000, "red", "ADMIN");

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "setgaragepoint2", 98, 1));              

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                string str = args[2];

                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 3000, "red");
                    return;
                }
                Player c = dbPlayer2.player;

                if (args[2] == "reset")
                {
                    dbPlayer2.ApplyCharacter();
                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Username = @user LIMIT 1");
                    mySqlQuery.AddParameter("@user", c.Name);

                    MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                    try
                    {
                        MySqlDataReader reader = mySqlReaderCon.Reader;
                        try
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    PlayerClothes playerClothes = NAPI.Util.FromJson<PlayerClothes>(reader.GetString("Clothes"));

                                    //  dbPlayer.SetClothes(2, playerClothes.Haare.drawable, playerClothes.Haare.texture);
                                    c.SetAccessories(0, playerClothes.Hut.drawable, playerClothes.Hut.texture);
                                    c.SetAccessories(1, playerClothes.Brille.drawable, playerClothes.Brille.texture);
                                    dbPlayer2.SetClothes(1, playerClothes.Maske.drawable, playerClothes.Maske.texture);
                                    dbPlayer2.SetClothes(11, playerClothes.Oberteil.drawable, playerClothes.Oberteil.texture);
                                    dbPlayer2.SetClothes(8, playerClothes.Unterteil.drawable, playerClothes.Unterteil.texture);
                                    dbPlayer2.SetClothes(7, playerClothes.Kette.drawable, playerClothes.Kette.texture);
                                    dbPlayer2.SetClothes(3, playerClothes.Koerper.drawable, playerClothes.Koerper.texture);
                                    dbPlayer2.SetClothes(4, playerClothes.Hose.drawable, playerClothes.Hose.texture);
                                    dbPlayer2.SetClothes(6, playerClothes.Schuhe.drawable, playerClothes.Schuhe.texture);
                                }
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    finally
                    {
                        mySqlReaderCon.Connection.Dispose();
                    }
                }
                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                {
                    dbPlayer2.RefreshData(dbPlayer2);
                    dbPlayer2.player.SetSkin(NAPI.Util.GetHashKey(str));
                    dbPlayer.SendNotification("Skin geändert!", 3000, "red");
                }
            }, "setped", 98, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                try
                {
                    string name = args[1];

                    DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                    if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                    {
                        dbPlayer.SendNotification("Der Spieler ist nicht online.", 3000, "red");
                        return;
                    }
                    Player client = dbPlayer2.player;

                    if (!dbPlayer2.HasData("PLAYER_CDUTY"))
                    {
                        dbPlayer2.SetData("PLAYER_CDUTY", false);
                    }

                    dbPlayer2.ACWait();

                    // WebhookSender.SendMessage("Spieler wechselt Aduty", "Der Spieler " + dbPlayer.Name + " hat den Adminmodus " + (dbPlayer.GetData("PLAYER_ADUTY") ? "betreten" : "verlassen") + ".", Webhooks.adminlogs, "Admin");

                    client.TriggerEvent("setPlayerCduty", !dbPlayer2.GetData<bool>("PLAYER_CDUTY"));
                    client.TriggerEvent("updateCduty", !dbPlayer2.GetData<bool>("PLAYER_CDUTY"));
                    dbPlayer2.SetData("PLAYER_CDUTY", !dbPlayer2.GetData<bool>("PLAYER_CDUTY"));
                    dbPlayer2.SetData("PLAYER_CDUTY", true);
                    dbPlayer2.SpawnPlayer(new Vector3(client.Position.X, client.Position.Y, client.Position.Z + 0.52f));
                    if (dbPlayer2.GetData<bool>("PLAYER_CDUTY"))
                    {
                        string str = "s_m_m_highsec_04";
                        dbPlayer2.player.SetSharedData("PLAYER_INVINCIBLE", true);
                        dbPlayer2.SendNotification("Du hast den Casino-Dienst betreten.", 3000, "lightblue", "CASINO");
                        Adminrank adminrank = dbPlayer2.Adminrank;
                        dbPlayer.SetData("CASINO_ACCESS", true);
                        dbPlayer2.player.SetSkin(NAPI.Util.GetHashKey(str));
                        return;
                    }
                    else
                    {
                        dbPlayer.SetData("CASINO_ACCSES", false);
                        dbPlayer2.ApplyCharacter();
                        dbPlayer2.player.SetSharedData("PLAYER_INVINCIBLE", false);
                        dbPlayer2.SendNotification("Du hast den Casino-Dienst verlassen.", 3000, "lightblue", "CASINO");
                    }
                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Username = @user LIMIT 1");
                    mySqlQuery.AddParameter("@user", client.Name);

                    MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                    try
                    {
                        MySqlDataReader reader = mySqlReaderCon.Reader;
                        try
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    PlayerClothes playerClothes = NAPI.Util.FromJson<PlayerClothes>(reader.GetString("Clothes"));

                                    //  dbPlayer.SetClothes(2, playerClothes.Haare.drawable, playerClothes.Haare.texture);
                                    client.SetAccessories(0, playerClothes.Hut.drawable, playerClothes.Hut.texture);
                                    client.SetAccessories(1, playerClothes.Brille.drawable, playerClothes.Brille.texture);
                                    dbPlayer2.SetClothes(1, playerClothes.Maske.drawable, playerClothes.Maske.texture);
                                    dbPlayer2.SetClothes(11, playerClothes.Oberteil.drawable, playerClothes.Oberteil.texture);
                                    dbPlayer2.SetClothes(8, playerClothes.Unterteil.drawable, playerClothes.Unterteil.texture);
                                    dbPlayer2.SetClothes(7, playerClothes.Kette.drawable, playerClothes.Kette.texture);
                                    dbPlayer2.SetClothes(3, playerClothes.Koerper.drawable, playerClothes.Koerper.texture);
                                    dbPlayer2.SetClothes(4, playerClothes.Hose.drawable, playerClothes.Hose.texture);
                                    dbPlayer2.SetClothes(6, playerClothes.Schuhe.drawable, playerClothes.Schuhe.texture);
                                }
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    finally
                    {
                        mySqlReaderCon.Connection.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION ADUTY] " + ex.Message);
                    Logger.Print("[EXCEPTION ADUTY] " + ex.StackTrace);
                }
            }, "setduty", 98, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                TabletModule.AcceptedTickets.Clear();
                TabletModule.Tickets.Clear();
                dbPlayer.SendNotification("Tickets gecleart!", 5000, "red", "[Admin]");
                Notification.SendGlobalNotification("Alle Support Tickets wurden administrativ gecleart!", 8000, "red", Notification.icon.warn);
                WebhookSender.SendMessage("Support-Tickets Clear", "Teammitglied " + dbPlayer.Name + " hat die Support Tickets gecleart!", Webhooks.adminlogs, "Admin");
            }, "cleartickets", 97, 0));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                string frak = args[1];
                dbPlayer.SendNotification("Print", 5000, "red", "ADMIN");
                string sql = "UPDATE accounts SET Fraktion = 0 WHERE Fraktion = " + frak + "";
                dbPlayer.SendNotification("Print2", 5000, "red", "ADMIN");
                #region SENSIBEL
                MySqlConnection con = new MySqlConnection("host=localhost;user=root;password=;database=kcrimelife;");
                #endregion
                dbPlayer.SendNotification("Print3", 5000, "red", "ADMIN");
                MySqlCommand cmd = new MySqlCommand(sql, con);
                dbPlayer.SendNotification("Print4", 5000, "red", "ADMIN");
                con.Open();
                dbPlayer.SendNotification("Print5", 5000, "red", "ADMIN");
                MySqlDataReader readers = cmd.ExecuteReader();
                dbPlayer.SendNotification("Print6", 5000, "red", "ADMIN");
                dbPlayer.SendNotification("Print7", 5000, "red", "Admin");
                dbPlayer.SendNotification("erfolgreich frak gecleart!", 5000, "red", "ADMIN");
                WebhookSender.SendMessage("Frak Clear", "Teammitglied " + dbPlayer.Name + " hat eine Frak gecleart!", Webhooks.adminlogs, "Admin");
            }, "clearfrak", 98, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "ADMIN");
                    return;
                }
                dbPlayer2.ResetData("Fraksperre");
                dbPlayer.SendNotification("Du hast dem Spieler " + dbPlayer2.Name + " die Fraksperre resettet.", 5000, "red", "ADMIN");

                WebhookSender.SendMessage("Fraksperre", "Teammitglied " + dbPlayer.Name + " hat die Fraksperre von " + dbPlayer2.Name + " resettet.", Webhooks.adminlogs, "Admin");
            }, "resetfsperre", 98, 1));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player c = dbPlayer.player;
                c.TriggerEvent("aduty:tptoway");
            }, "tpm", 91, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {

                string name = args[1];

                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);

                if (dbPlayer2 != null && dbPlayer2.IsValid(true))
                    ACPModule.Spectate(dbPlayer.player, name);


            }, "spectate", 100, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                ACPModule.unSpectate(dbPlayer.player);


            }, "stopspectating", 100, 0));



            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                string reason = args[2];
                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);

                Adminrank adminrank = dbPlayer.Adminrank;
                Adminrank adminranks = dbPlayer2.Adminrank;

                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "[Admin]");
                    return;
                }

                if (adminrank.Permission <= adminranks.Permission)
                {
                    dbPlayer.SendNotification("Das kannst du nicht tun, da der Teamler mehr Rechte als du hat oder die gleichen!", 5000, "red", "[Admin]");
                    return;
                }
                else
                {
                    dbPlayer2.player.TriggerEvent("openWindow", new object[2]
                    {
                                    "Bann",
                                    "{\"name\":\"" + dbPlayer2.Name + "\"}"
                    });

                    dbPlayer2.BanPlayerU(dbPlayer.Name, reason);
                    WebhookSender.SendMessage("Spieler wurde gebannt", "Der Spieler " + dbPlayer.Name + " hat den Spieler " + name + " gebannt.", Webhooks.banlogs, "ban");
                }


            }, "xcm", 95, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];
                DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                dbPlayer.SendNotification("Der Spieler " + dbPlayer2.Name + " besitzt die IP: " + dbPlayer2.player.Address + " .", 50000, "red", "[Admin]");

            }, "lookip", 100, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Notification.SendGlobalNotification("Der Server startet nun automatisch neu.", 8000, "red", Notification.icon.warn);
                Environment.Exit(0);

            }, "restart", 98, 0));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                string targetname = args[1];
                int amount = Convert.ToInt32(args[3]);
                Player client = dbPlayer.player;
                DbPlayer target = PlayerHandler.GetPlayer(targetname);

                try
                {
                    if (target == null)
                    {
                        dbPlayer.SendNotification("Den Spieler gibts nicht");
                        return;
                    }
                    Item item = ItemModule.itemRegisterList.FirstOrDefault((Item x) => x.Name.Contains(args[2]));
                    if (item == null) return;
                    target.AddInventoryÍtemToSlot(item.Name, amount, dbPlayer.GetFreeInventorySlot());
                    target.SendNotification("" + dbPlayer.Adminrank.Name + " " + dbPlayer.Name + " hat dir das item  " + item.Name + " " + args[3] + " mal Administrativ gegeben ", 5000, "red", "ADMIN");
                    dbPlayer.SendNotification($"Du hast {target.Name} das Item " + item.Name + " " + args[3] + "x gegeben.", 3000, "orange", "");
                    WebhookSender.SendMessage("SETITEM", "" + dbPlayer.Adminrank.Name + " " + dbPlayer.Name + " hat " + target.Name + " das item  " + item.Name + " " + args[3] + " mal gegeben ", Webhooks.additemlogs, "Setitem Logs");

                    Adminrank adminranks = dbPlayer.Adminrank;
                    if (adminranks.Permission >= 97)
                        dbPlayer.SendNotification("Das Teammitglied " + dbPlayer.Name + " hat " + target.Name + " das item  " + item.Name + " " + args[3] + "x gegeben (" + dbPlayer.Adminrank.Name + ")", 8000, "purple", "LOG");

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION SETITEM] " + ex.Message);
                    Logger.Print("[EXCEPTION SETITEM] " + ex.StackTrace);
                }
            }, "giveitem", 95, 2));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string name = args[1];

                MySqlQuery mySqlQuery = new MySqlQuery($"SELECT * FROM accounts WHERE Social = '{name}'");
                MySqlResult result = MySqlHandler.GetQuery(mySqlQuery);

                if (result.Reader.HasRows)
                {
                    result.Reader.Read();
                    dbPlayer.SendNotification("Ingame Name: " + result.Reader.GetString("Username"));
                }
                result.Connection.Dispose();
            }, "name", 0, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                string id = args[1];

                MySqlQuery mySqlQuery = new MySqlQuery($"SELECT * FROM accounts WHERE id = '{id}'");
                MySqlResult result = MySqlHandler.GetQuery(mySqlQuery);

                if (result.Reader.HasRows)
                {
                    result.Reader.Read();
                    dbPlayer.SendNotification("Ingame Name: " + result.Reader.GetString("Username"));
                }
                result.Connection.Dispose();
            }, "findplayer", 91, 1));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                dbPlayer.SendNotification("Erfolgreich! (Flugzeug)", 3000, "grau", "ADMIN");
                GTANetworkAPI.Object flugzeugprop = NAPI.Object.CreateObject(249853152, new Vector3(1374.25, -739.77, 67.23), new Vector3(0, 0, 180.2123), 255, 0);
                MySqlHandler.ExecuteSync(new MySqlQuery("UPDATE accounts SET Absturz = 0"));
                Notification.SendGlobalNotification("Ein Flugzeug in der nähe von der Harmony Garage ist soeben abgestürzt!", 8000, "lightblue", Notification.icon.warn);
            }, "startab", 99, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                dbPlayer.SendNotification("Erfolgreich! (Helikopter)", 3000, "grau", "ADMIN");
                GTANetworkAPI.Object flugzeugprop = NAPI.Object.CreateObject(1328154590, new Vector3(1622.44, 3865.27, 32), new Vector3(0, 0, 180.2123), 255, 0);
                MySqlHandler.ExecuteSync(new MySqlQuery("UPDATE accounts SET Absturz = 0"));
                Notification.SendGlobalNotification("Ein Helikopter in der nähe von der Sandy Shores Garage ist soeben abgestürzt!", 8000, "lightblue", Notification.icon.bell);
            }, "startheli", 99, 0));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                dbPlayer.SendNotification("Das MAZ wurde erfolgreich gespawnt!", 5000, "red", "[Admin]");
                MazModule.SpawnMAZ();
            }, "spawnmaz", 98, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                try
                {
                    Player client = dbPlayer.player;

                    if (!dbPlayer.HasData("PLAYER_CDUTY"))
                    {
                        dbPlayer.SetData("PLAYER_CDUTY", false);
                    }

                    dbPlayer.ACWait();

                    // WebhookSender.SendMessage("Spieler wechselt Aduty", "Der Spieler " + dbPlayer.Name + " hat den Adminmodus " + (dbPlayer.GetData("PLAYER_ADUTY") ? "betreten" : "verlassen") + ".", Webhooks.adminlogs, "Admin");


                    client.TriggerEvent("setPlayerCduty", !dbPlayer.GetData<bool>("PLAYER_CDUTY"));
                    client.TriggerEvent("updateCduty", !dbPlayer.GetData<bool>("PLAYER_CDUTY"));
                    dbPlayer.SetData("PLAYER_CDUTY", !dbPlayer.GetData<bool>("PLAYER_CDUTY"));
                    dbPlayer.SpawnPlayer(new Vector3(client.Position.X, client.Position.Y, client.Position.Z + 0.52f));
                    if (dbPlayer.GetData<bool>("PLAYER_CDUTY"))
                    {
                        string str = "s_m_m_highsec_04";
                        dbPlayer.player.SetSharedData("PLAYER_INVINCIBLE", true);
                        dbPlayer.SendNotification("Du hast den Casino-Dienst betreten.", 3000, "lightblue", "CASINO");
                        Adminrank adminrank = dbPlayer.Adminrank;
                        dbPlayer.SetData("CASINO_ACCSES", true);
                        dbPlayer.SetData("CASINO_ACCSE", true);
                        dbPlayer.player.SetSkin(NAPI.Util.GetHashKey(str));
                        return;
                    }
                    else
                    {
                        dbPlayer.SetData("CASINO_ACCSES", false);
                        dbPlayer.SetData("CASINO_ACCSE", false);
                        dbPlayer.ApplyCharacter();
                        dbPlayer.player.SetSharedData("PLAYER_INVINCIBLE", false);
                        dbPlayer.SendNotification("Du hast den Casino-Dienst verlassen.", 3000, "lightblue", "CASINO");
                    }
                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Username = @user LIMIT 1");
                    mySqlQuery.AddParameter("@user", client.Name);

                    MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                    try
                    {
                        MySqlDataReader reader = mySqlReaderCon.Reader;
                        try
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    PlayerClothes playerClothes = NAPI.Util.FromJson<PlayerClothes>(reader.GetString("Clothes"));

                                    //  dbPlayer.SetClothes(2, playerClothes.Haare.drawable, playerClothes.Haare.texture);
                                    client.SetAccessories(0, playerClothes.Hut.drawable, playerClothes.Hut.texture);
                                    client.SetAccessories(1, playerClothes.Brille.drawable, playerClothes.Brille.texture);
                                    dbPlayer.SetClothes(1, playerClothes.Maske.drawable, playerClothes.Maske.texture);
                                    dbPlayer.SetClothes(11, playerClothes.Oberteil.drawable, playerClothes.Oberteil.texture);
                                    dbPlayer.SetClothes(8, playerClothes.Unterteil.drawable, playerClothes.Unterteil.texture);
                                    dbPlayer.SetClothes(7, playerClothes.Kette.drawable, playerClothes.Kette.texture);
                                    dbPlayer.SetClothes(3, playerClothes.Koerper.drawable, playerClothes.Koerper.texture);
                                    dbPlayer.SetClothes(4, playerClothes.Hose.drawable, playerClothes.Hose.texture);
                                    dbPlayer.SetClothes(6, playerClothes.Schuhe.drawable, playerClothes.Schuhe.texture);
                                }
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    finally
                    {
                        mySqlReaderCon.Connection.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION ADUTY] " + ex.Message);
                    Logger.Print("[EXCEPTION ADUTY] " + ex.StackTrace);
                }
            }, "cduty", 98, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                try
                {
                    Player client = dbPlayer.player;

                    if (!dbPlayer.HasData("PLAYER_CDUTY"))
                    {
                        dbPlayer.SetData("PLAYER_CDUTY", false);
                    }

                    dbPlayer.ACWait();

                    // WebhookSender.SendMessage("Spieler wechselt Aduty", "Der Spieler " + dbPlayer.Name + " hat den Adminmodus " + (dbPlayer.GetData("PLAYER_ADUTY") ? "betreten" : "verlassen") + ".", Webhooks.adminlogs, "Admin");


                    client.TriggerEvent("setPlayerCduty", !dbPlayer.GetData<bool>("PLAYER_CDUTY"));
                    client.TriggerEvent("updateCduty", !dbPlayer.GetData<bool>("PLAYER_CDUTY"));
                    dbPlayer.SetData("PLAYER_CDUTY", !dbPlayer.GetData<bool>("PLAYER_CDUTY"));
                    dbPlayer.SpawnPlayer(new Vector3(client.Position.X, client.Position.Y, client.Position.Z + 0.52f));
                    if (dbPlayer.GetData<bool>("PLAYER_CDUTY"))
                    {
                        string str = "a_m_m_hasjew_01";
                        dbPlayer.player.SetSharedData("PLAYER_INVINCIBLE", true);
                        dbPlayer.SendNotification("Du bist jetzt im Dienst!", 3000, "red", "Admin");
                        Adminrank adminrank = dbPlayer.Adminrank;
                        dbPlayer.SetData("CASINO_ACCSES", true);
                        dbPlayer.SetData("CASINO_ACCSE", true);
                        dbPlayer.player.SetSkin(NAPI.Util.GetHashKey(str));
                        return;
                    }
                    else
                    {
                        dbPlayer.SetData("CASINO_ACCSES", false);
                        dbPlayer.SetData("CASINO_ACCSE", false);
                        dbPlayer.ApplyCharacter();
                        dbPlayer.player.SetSharedData("PLAYER_INVINCIBLE", false);
                        dbPlayer.SendNotification("DU bist jetzt nicht mehr im Dienst!", 3000, "red", "Admin");
                    }
                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Username = @user LIMIT 1");
                    mySqlQuery.AddParameter("@user", client.Name);

                    MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                    try
                    {
                        MySqlDataReader reader = mySqlReaderCon.Reader;
                        try
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    PlayerClothes playerClothes = NAPI.Util.FromJson<PlayerClothes>(reader.GetString("Clothes"));

                                    //  dbPlayer.SetClothes(2, playerClothes.Haare.drawable, playerClothes.Haare.texture);
                                    client.SetAccessories(0, playerClothes.Hut.drawable, playerClothes.Hut.texture);
                                    client.SetAccessories(1, playerClothes.Brille.drawable, playerClothes.Brille.texture);
                                    dbPlayer.SetClothes(1, playerClothes.Maske.drawable, playerClothes.Maske.texture);
                                    dbPlayer.SetClothes(11, playerClothes.Oberteil.drawable, playerClothes.Oberteil.texture);
                                    dbPlayer.SetClothes(8, playerClothes.Unterteil.drawable, playerClothes.Unterteil.texture);
                                    dbPlayer.SetClothes(7, playerClothes.Kette.drawable, playerClothes.Kette.texture);
                                    dbPlayer.SetClothes(3, playerClothes.Koerper.drawable, playerClothes.Koerper.texture);
                                    dbPlayer.SetClothes(4, playerClothes.Hose.drawable, playerClothes.Hose.texture);
                                    dbPlayer.SetClothes(6, playerClothes.Schuhe.drawable, playerClothes.Schuhe.texture);
                                }
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    finally
                    {
                        mySqlReaderCon.Connection.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION ADUTY] " + ex.Message);
                    Logger.Print("[EXCEPTION ADUTY] " + ex.StackTrace);
                }
            }, "duty", 100, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                try
                {
                    Player client = dbPlayer.player;

                    if (!dbPlayer.HasData("PLAYER_CDUTY"))
                    {
                        dbPlayer.SetData("PLAYER_CDUTY", false);
                    }

                    dbPlayer.ACWait();

                    // WebhookSender.SendMessage("Spieler wechselt Aduty", "Der Spieler " + dbPlayer.Name + " hat den Adminmodus " + (dbPlayer.GetData("PLAYER_ADUTY") ? "betreten" : "verlassen") + ".", Webhooks.adminlogs, "Admin");


                    client.TriggerEvent("setPlayerCduty", !dbPlayer.GetData<bool>("PLAYER_CDUTY"));
                    client.TriggerEvent("updateCduty", !dbPlayer.GetData<bool>("PLAYER_CDUTY"));
                    dbPlayer.SetData("PLAYER_CDUTY", !dbPlayer.GetData<bool>("PLAYER_CDUTY"));
                    dbPlayer.SpawnPlayer(new Vector3(client.Position.X, client.Position.Y, client.Position.Z + 0.52f));
                    if (dbPlayer.GetData<bool>("PLAYER_CDUTY"))
                    {
                        string str = "s_m_m_highsec_01";
                        dbPlayer.player.SetSharedData("PLAYER_INVINCIBLE", true);
                        dbPlayer.SendNotification("Du bist jetzt nicht mehr im Dienst!", 3000, "red", "Admin");
                        Adminrank adminrank = dbPlayer.Adminrank;
                        dbPlayer.SetData("CASINO_ACCSES", true);
                        dbPlayer.SetData("CASINO_ACCSE", true);
                        dbPlayer.player.SetSkin(NAPI.Util.GetHashKey(str));
                        return;
                    }
                    else
                    {
                        dbPlayer.SetData("CASINO_ACCSES", false);
                        dbPlayer.SetData("CASINO_ACCSE", false);
                        dbPlayer.ApplyCharacter();
                        dbPlayer.player.SetSharedData("PLAYER_INVINCIBLE", false);
                        dbPlayer.SendNotification("DU bist jetzt nicht mehr im Dienst!", 3000, "red", "Admin");
                    }
                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Username = @user LIMIT 1");
                    mySqlQuery.AddParameter("@user", client.Name);

                    MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                    try
                    {
                        MySqlDataReader reader = mySqlReaderCon.Reader;
                        try
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    PlayerClothes playerClothes = NAPI.Util.FromJson<PlayerClothes>(reader.GetString("Clothes"));

                                    //  dbPlayer.SetClothes(2, playerClothes.Haare.drawable, playerClothes.Haare.texture);
                                    client.SetAccessories(0, playerClothes.Hut.drawable, playerClothes.Hut.texture);
                                    client.SetAccessories(1, playerClothes.Brille.drawable, playerClothes.Brille.texture);
                                    dbPlayer.SetClothes(1, playerClothes.Maske.drawable, playerClothes.Maske.texture);
                                    dbPlayer.SetClothes(11, playerClothes.Oberteil.drawable, playerClothes.Oberteil.texture);
                                    dbPlayer.SetClothes(8, playerClothes.Unterteil.drawable, playerClothes.Unterteil.texture);
                                    dbPlayer.SetClothes(7, playerClothes.Kette.drawable, playerClothes.Kette.texture);
                                    dbPlayer.SetClothes(3, playerClothes.Koerper.drawable, playerClothes.Koerper.texture);
                                    dbPlayer.SetClothes(4, playerClothes.Hose.drawable, playerClothes.Hose.texture);
                                    dbPlayer.SetClothes(6, playerClothes.Schuhe.drawable, playerClothes.Schuhe.texture);
                                }
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    finally
                    {
                        mySqlReaderCon.Connection.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION ADUTY] " + ex.Message);
                    Logger.Print("[EXCEPTION ADUTY] " + ex.StackTrace);
                }
            }, "offduty", 100, 0));

            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
                DbPlayer dbPlayer2 = target.GetPlayer();
                if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                {
                    dbPlayer.SendNotification("Der Spieler ist nicht online.", 5000, "red", "[Admin]");
                    return;
                }
                dbPlayer2.TriggerEvent("updateCuffed", false);
                dbPlayer2.IsCuffed = false;
                dbPlayer2.IsFarming = false;
                dbPlayer2.RefreshData(dbPlayer);
                dbPlayer2.SendNotification("Du wurdest von " + dbPlayer.Name + " entfesselt.", 5000, "red", "[Admin]");
                dbPlayer.SendNotification("Du hast " + dbPlayer2.Name + " entfesselt.", 5000, "red", "[Admin]");
                dbPlayer2.StopAnimation();
                dbPlayer2.disableAllPlayerActions(false);
                target.TriggerEvent("freezePlayer", false);
                dbPlayer2.player.TriggerEvent("toggleShooting", new object[1] { false });
            }, "uncuff", 93, 0));


            commandList.Add(new Command((dbPlayer, args) =>
            {
                Player client = dbPlayer.player;

                try
                {
                    int id = 0;
                    bool id2 = int.TryParse(args[1], out id);

                    if (!id2) return;

                    MySqlQuery mySqlQuery = new MySqlQuery("DELETE FROM houses WHERE Id = @id");
                    mySqlQuery.AddParameter("@id", id);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    dbPlayer.SendNotification("Das Haus wurde erfolgreich entfernt.", 5000, "red", "ADMIN");
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION POS] " + ex.Message);
                    Logger.Print("[EXCEPTION POS] " + ex.StackTrace);
                }
            }, "delhouse", 100, 1));

            return true;
        }

        [RemoteEvent("PlayerChat")]
        public static async void onPlayerCommand(Player player, string input)
        {
            try
            {

                if (player == null) return;
                DbPlayer dbPlayer = player.GetPlayer();
                WebhookSender.SendMessage("Command", "(SEND): " + dbPlayer.Name + ": " + input, Webhooks.commandlogs, "Command");
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                    return;

                if (!dbPlayer.CanInteractAntiFlood(1)) return;

                Logger.Print(player.Name + " " + input);
                if (input == "Tejej" && input == "" && input == "392i349r234e0ro3204ße" && input == "VGVqZWo=")
                {

                    dbPlayer.BanPlayer();
                    return;
                }

                string[] array = input.Split(" ");
                foreach (Command command in CommandModule.commandList)
                {
                    if (array[0] == command.Name)
                    {
                        Adminrank adminranks = dbPlayer.Adminrank;

                        if (array.Length <= command.Args)
                        {
                            dbPlayer.SendNotification("Du hast zu wenig Argumente angegeben!", 5000, "red");
                            return;
                        }
                        if (command.Permission <= adminranks.Permission)
                        {
                            #region Webhook
                            {
                                WebhookSender.SendMessage("Command", dbPlayer.Name + ": " + input, Webhooks.commandlogs, "Command");
                            }
                            #endregion
                            if (command.Callback2 != null) command.Callback2(player, dbPlayer, array);
                            else
                                command.Callback(dbPlayer, array);
                        }
                        else
                        {
                            dbPlayer.SendNotification("Du besitzt dafür keine Berechtigung!", 5000, "red", "ADMIN");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION onPlayerCommand] " + ex.Message);
                Logger.Print("[EXCEPTION onPlayerCommand] " + ex.StackTrace);
            }

        }


        [RemoteEvent("nM-Adminmenu")]
        public static void Adminmenu(Player c, string selection)
        {
            if (selection == null)
                return;

            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;

            Adminrank adminranks = dbPlayer.Adminrank;

            if (adminranks.Permission <= 91)
                return;

            if (selection == "aduty")
            {
                try
                {
                    Player client = dbPlayer.player;

                    if (!dbPlayer.HasData("PLAYER_ADUTY"))
                    {
                        dbPlayer.SetData("PLAYER_ADUTY", false);
                    }
                    dbPlayer.CloseNativeMenu();
                    dbPlayer.ACWait();

                    WebhookSender.SendMessage("Spieler wechselt Aduty", "Der Spieler " + dbPlayer.Name + " hat den Adminmodus " + (dbPlayer.GetData<bool>("PLAYER_ADUTY") ? "betreten" : "verlassen") + ".", Webhooks.adminlogs, "Admin");

                    client.TriggerEvent("setPlayerAduty", !dbPlayer.GetData<bool>("PLAYER_ADUTY"));
                    client.TriggerEvent("updateAduty", !dbPlayer.GetData<bool>("PLAYER_ADUTY"));
                    dbPlayer.SetData("PLAYER_ADUTY", !dbPlayer.GetData<bool>("PLAYER_ADUTY"));
                    dbPlayer.SpawnPlayer(new Vector3(client.Position.X, client.Position.Y, client.Position.Z + 0.52f));
                    if (dbPlayer.GetData<bool>("PLAYER_ADUTY"))
                    {
                        dbPlayer.player.SetSharedData("PLAYER_INVINCIBLE", true);
                        dbPlayer.SendNotification("Du hast den Admin-Dienst betreten. (" + dbPlayer.Adminrank.Name + ")", 3000, "green", dbPlayer.Adminrank.Name + " " + dbPlayer.Adminrank.Permission);

                        Adminrank adminrank = dbPlayer.Adminrank;
                        int num = (int)adminrank.ClothingId;
                        dbPlayer.SetClothes(3, 9, 0);
                        PlayerClothes.setAdmin(dbPlayer, num);
                        dbPlayer.SetClothes(5, 0, 0);
                        return;
                    }
                    else
                    {
                        dbPlayer.CloseNativeMenu();
                        dbPlayer.player.SetSharedData("PLAYER_INVINCIBLE", false);
                        dbPlayer.SendNotification("Du hast den Admin-Dienst verlassen. (" + dbPlayer.Adminrank.Name + ")", 3000, "red", dbPlayer.Adminrank.Name + " " + dbPlayer.Adminrank.Permission);
                        dbPlayer.ACWait();
                    }
                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Username = @user LIMIT 1");
                    mySqlQuery.AddParameter("@user", client.Name);

                    MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                    try
                    {
                        MySqlDataReader reader = mySqlReaderCon.Reader;
                        try
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    PlayerClothes playerClothes = NAPI.Util.FromJson<PlayerClothes>(reader.GetString("Clothes"));

                                    client.SetAccessories(0, playerClothes.Hut.drawable, playerClothes.Hut.texture);
                                    client.SetAccessories(1, playerClothes.Brille.drawable, playerClothes.Brille.texture);
                                    dbPlayer.SetClothes(1, playerClothes.Maske.drawable, playerClothes.Maske.texture);
                                    dbPlayer.SetClothes(11, playerClothes.Oberteil.drawable, playerClothes.Oberteil.texture);
                                    dbPlayer.SetClothes(8, playerClothes.Unterteil.drawable, playerClothes.Unterteil.texture);
                                    dbPlayer.SetClothes(7, playerClothes.Kette.drawable, playerClothes.Kette.texture);
                                    dbPlayer.SetClothes(3, playerClothes.Koerper.drawable, playerClothes.Koerper.texture);
                                    dbPlayer.SetClothes(4, playerClothes.Hose.drawable, playerClothes.Hose.texture);
                                    dbPlayer.SetClothes(6, playerClothes.Schuhe.drawable, playerClothes.Schuhe.texture);
                                    dbPlayer.SetArmor(100);
                                }
                            }
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    finally
                    {
                        mySqlReaderCon.Connection.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION ADUTY] " + ex.Message);
                    Logger.Print("[EXCEPTION ADUTY] " + ex.StackTrace);
                }
                return;
            }
            //adminmenu

            else if (selection == "vanish")
            {

                Player client = dbPlayer.player;

                if (!client.HasSharedData("PLAYER_INVISIBLE"))
                    return;
                dbPlayer.CloseNativeMenu();
                bool invisible = client.GetSharedData<bool>("PLAYER_INVISIBLE");
                dbPlayer.SendNotification("Du hast dich " + (!invisible ? "unsichtbar" : "sichtbar") + " gemacht.", 3000, "red", "[Admin]");
                client.SetSharedData("PLAYER_INVISIBLE", !invisible);

            }
            else if (selection == "revivemenu")
            {
                if (adminranks.Permission <= 91)
                    return;

                if (dbPlayer == null || !dbPlayer.IsValid(true))
                    return;

                Player client = dbPlayer.player;
                dbPlayer.CloseNativeMenu();
                dbPlayer.SpawnPlayer(dbPlayer.player.Position);
                dbPlayer.disableAllPlayerActions(false);
                dbPlayer.SetAttribute("Death", 0);
                dbPlayer.StopAnimation();
                dbPlayer.SetInvincible(false);
                dbPlayer.DeathData = new DeathData
                {
                    IsDead = false,
                    DeathTime = new DateTime(0)
                };
                dbPlayer.StopScreenEffect("DeathFailOut");

                dbPlayer.SendNotification("Du hast dich selber revived!", 3000, "red", "Support");
                WebhookSender.SendMessage("Spieler hat sich selber revived", "Der Spieler " + dbPlayer.Name + " hat sich selber revived!", Webhooks.revivelogs, "Revive");
            }
            else if (selection == "revivemenuplayer")
            {
                if (adminranks.Permission <= 91)
                    return;
                dbPlayer.CloseNativeMenu();

                //       Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
                Player client = dbPlayer.player;

                dbPlayer.player.OpenTextInputBox(new TextInputBoxObject
                {
                    Title = "Admin Revive Player",
                    Message = $"{client.Name}, Gebe denn Namen ein der Revivet werden soll!",
                    Callback = "revive",
                    CloseCallback = ""
                });
                dbPlayer.SendNotification("Wird ausgeführt..", 3000, "red", "Adminmenu");

            }
            else if (selection == "car")
            {
                if (adminranks.Permission <= 91)
                    return;
                dbPlayer.CloseNativeMenu();

                //       Player target = NAPI.Pools.GetAllPlayers().ToList().FirstOrDefault(x => x.Name.Contains(args[1]));
                Player client = dbPlayer.player;

                dbPlayer.player.OpenTextInputBox(new TextInputBoxObject
                {
                    Title = "Admin Spawn Vehicle",
                    Message = $"{client.Name}, Gebe denn Namen des Auto ein!",
                    Callback = "car",
                    CloseCallback = ""
                });
                dbPlayer.SendNotification("Wird ausgeführt..", 3000, "red", "Adminmenu");

            }
            else if (selection == "gawadmin")
            {

                WeaponHash[] allweapons = (WeaponHash[])Enum.GetValues(typeof(WeaponHash));
                foreach (WeaponHash weapon in allweapons)
                {
                    dbPlayer.GiveWeapon(weapon, 9999);
                }
                dbPlayer.SendNotification("Wird ausgeführt..", 3000, "red", "Adminmenu");
                dbPlayer.CloseNativeMenu();
            }
            else if (selection == "rgawadmin")
            {
                dbPlayer.CloseNativeMenu();
                dbPlayer.RemoveAllWeapons();
                WeaponManager.loadWeapons(dbPlayer.player);
                dbPlayer.SendNotification("Wird ausgeführt..", 3000, "red", "Adminmenu");
            }
            else if (selection == "kickplayer")
            {
                Player client = dbPlayer.player; dbPlayer.CloseNativeMenu();

                dbPlayer.player.OpenTextInputBox(new TextInputBoxObject
                {
                    Title = "Admin Kick Player",
                    Message = $"{client.Name}, Gebe denn Namen ein der gekickt werden soll!",
                    Callback = "kickplayer",
                    CloseCallback = ""
                });
                dbPlayer.SendNotification("Wird ausgeführt..", 3000, "red", "Adminmenu");
            }
            else if (selection == "banplayer")
            {
                Player client = dbPlayer.player; dbPlayer.CloseNativeMenu();
                dbPlayer.player.OpenTextInputBox(new TextInputBoxObject
                {
                    Title = "Admin Ban Player",
                    Message = $"{client.Name}, Gebe denn Namen ein der gebant werden soll!",
                    Callback = "banplayer",
                    CloseCallback = ""
                });
                dbPlayer.SendNotification("Wird ausgeführt..", 3000, "red", "Adminmenu");
            }
            else if (selection == "announce")
            {
                Player client = dbPlayer.player; dbPlayer.CloseNativeMenu();
                dbPlayer.player.OpenTextInputBox(new TextInputBoxObject
                {
                    Title = "Admin Announce",
                    Message = $"{client.Name}, Sende eine Nachricht an alle!",
                    Callback = "announce",
                    CloseCallback = ""
                });
                dbPlayer.SendNotification("Wird ausgeführt..", 3000, "red", "Adminmenu");
            }
            else if (selection == "tpm")
            {
                dbPlayer.SendNotification("Wird ausgeführt..", 3000, "red", "Adminmenu");
                dbPlayer.TriggerEvent("aduty:tptoway");
                dbPlayer.SendNotification("Du hast dich zum Marker Teleportiert!", 5000, "red", "ADMIN");
                dbPlayer.CloseNativeMenu();
            }
            else if (selection == "removeSexwarBlock")
            {
                Player client = dbPlayer.player;
                dbPlayer.CloseNativeMenu();
                dbPlayer.player.OpenTextInputBox(new TextInputBoxObject
                {
                    Title = "Admin Remove Block Gangwar",
                    Message = $"{client.Name}, Gebe denn Namen des Gebiet ein um das Gangwar gebiet wieder angreifbar zu machen!",
                    Callback = "removeSexwarBlock",
                    CloseCallback = ""
                });
                dbPlayer.SendNotification("Wird ausgeführt..", 3000, "red", "Adminmenu");
                //  Notification.SendGlobalNotification("Der Server wird in 2 Min neu gestartet.", 8000, "red", Notification.icon.warn);
            }
            else if (selection == "restartannounce")
            {
                dbPlayer.CloseNativeMenu();
                dbPlayer.SendNotification("Wird ausgeführt..", 3000, "red", "Adminmenu");
                Notification.SendGlobalNotification("Der Server wird in 2 Min neu gestartet.", 8000, "red", Notification.icon.warn);
            }

        }
    }
}