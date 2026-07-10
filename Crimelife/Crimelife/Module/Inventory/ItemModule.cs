using GTANetworkAPI;
using Crimelife.Types;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using GVMP;
using Crimelife.Module;

namespace Crimelife
{
    internal class ItemModule : Module<ItemModule>
    {
        public static List<Item> itemRegisterList = new List<Item>();
        public static List<string> equipItems = new List<string>();
        protected override bool OnLoad()
        {
            equipItems.Add("revolter");
            equipItems.Add("cyclone");
            equipItems.Add("revolter");
            equipItems.Add("schafterg");
            equipItems.Add("bati");
            equipItems.Add("bati");
            equipItems.Add("t20");
            equipItems.Add("schafterg");
            equipItems.Add("schafterg");
            equipItems.Add("cyclone");
            equipItems.Add("t20");
            equipItems.Add("bati");
            equipItems.Add("gtr");
            equipItems.Add("bati");
            equipItems.Add("revolter");
            equipItems.Add("bati");
            using MySqlConnection con = new MySqlConnection(Configuration.connectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM items";
                MySqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Item item = new Item
                            {
                                Id = reader.GetInt32("Id"),
                                ImagePath = reader.GetString("Image"),
                                MaxStackSize = reader.GetInt32("Stack"),
                                Name = reader.GetString("Name"),
                                WeightInG = reader.GetInt32("Weight"),
                                Whash = (WeaponHash)NAPI.Util.GetHashKey(reader.GetString("Whash"))
                            };

                            itemRegisterList.Add(item);
                        }
                    }
                }
                finally
                {
                    con.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION loadItems] " + ex.Message);
                Logger.Print("[EXCEPTION loadItems] " + ex.StackTrace);
            }
            finally
            {
                con.Dispose();
            }

            return true;
        }

        [RemoteEvent("Pressed_KOMMA")]
        public static void komma(Player client)
        {
            try
            {
                if (client == null) return;
                DbPlayer dbPlayer = client.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                    return;

                if (dbPlayer.DeathData.IsDead || dbPlayer.player.IsInVehicle) return;

                if (dbPlayer.IsFarming)
                {
                    return;
                }

                if (dbPlayer.Usingwest)
                {
                    return;
                }

                MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM inventorys WHERE Id = @userId LIMIT 1");
                mySqlQuery.AddParameter("@userId", dbPlayer.Id);
                MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                try
                {
                    MySqlDataReader reader = mySqlReaderCon.Reader;
                    try
                    {
                        if (!reader.HasRows)
                        {
                            return;
                        }

                        reader.Read();
                        List<ItemModel> list = new List<ItemModel>();
                        string @string = reader.GetString("Items");
                        list = NAPI.Util.FromJson<List<ItemModel>>(@string);
                        ItemModel itemToUse = list.FirstOrDefault((ItemModel x) => x.Name == "Verbandskasten");
                        if (itemToUse == null)
                        {
                            return;
                        }

                        int index = list.IndexOf(itemToUse);
                        if (itemToUse.Amount == 1)
                        {
                            list.Remove(itemToUse);
                        }
                        else
                        {
                            itemToUse.Amount--;
                            list[index] = itemToUse;
                        }

                        Item item = itemRegisterList.FirstOrDefault((Item x) => x.Name == itemToUse.Name);
                        reader.Close();
                        if (reader.IsClosed)
                        {
                            mySqlQuery.Query = "UPDATE inventorys SET Items = @invItems WHERE Id = @pId";
                            mySqlQuery.Parameters = new List<MySqlParameter>()
                            {
                                new MySqlParameter("@invItems", NAPI.Util.ToJson(list)),
                                new MySqlParameter("@pId", dbPlayer.Id)
                            };
                            MySqlHandler.ExecuteSync(mySqlQuery);
                            dbPlayer.disableAllPlayerActions(true);
                            dbPlayer.SendProgressbar(4000);
                            dbPlayer.IsFarming = true;
                            dbPlayer.Usingwest = true;
                            dbPlayer.RefreshData(dbPlayer);
                            dbPlayer.PlayAnimation(33, "amb@medic@standing@tendtodead@idle_a", "idle_a", 8f);
                            NAPI.Task.Run(delegate
                            {
                                if (dbPlayer.Usingwest)
                                {
                                    dbPlayer.SetHealth(99);
                                    dbPlayer.TriggerEvent("client:respawning");
                                    dbPlayer.StopProgressbar();
                                    dbPlayer.IsFarming = false;
                                    dbPlayer.Usingwest = false;
                                    dbPlayer.RefreshData(dbPlayer);
                                    dbPlayer.StopAnimation();
                                    dbPlayer.disableAllPlayerActions(false);
                                    dbPlayer.SendNotification("Du hast einen Verbandskasten benutzt.", 3000, "green");
                                }
                            }, 4000);
                        }

                        if (client.Dimension == 22750)
                        {
                            dbPlayer.disableAllPlayerActions(true);
                            dbPlayer.SendProgressbar(4000);
                            dbPlayer.IsFarming = true;
                            dbPlayer.Usingwest = true;
                            dbPlayer.RefreshData(dbPlayer);
                            dbPlayer.PlayAnimation(33, "amb@medic@standing@tendtodead@idle_a", "idle_a", 8f);
                            NAPI.Task.Run(delegate
                            {
                                if (dbPlayer.Usingwest)
                                {
                                    dbPlayer.SetHealth(99);
                                    dbPlayer.TriggerEvent("client:respawning");
                                    dbPlayer.StopProgressbar();
                                    dbPlayer.IsFarming = false;
                                    dbPlayer.Usingwest = false;

                                    dbPlayer.RefreshData(dbPlayer);
                                    dbPlayer.disableAllPlayerActions(false);
                                    dbPlayer.SendNotification("Du hast einen FFA-Verbandskasten benutzt.", 3000, "green");
                                    dbPlayer.StopAnimation();
                                }
                            }, 4000);
                            return;
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
                Logger.Print("[EXCEPTION Pressed_KOMMA] " + ex.Message);
                Logger.Print("[EXCEPTION Pressed_KOMMA] " + ex.StackTrace);
            }
        }

        [RemoteEvent("Pressed_PUNKT")]
        public static void punkt(Player client)
        {
            try
            {
                if (client == null) return;
                DbPlayer dbPlayer = client.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                    return;

                if (dbPlayer.DeathData.IsDead || dbPlayer.player.IsInVehicle) return;

                if (dbPlayer.IsFarming)
                {
                    return;
                }


                if (dbPlayer.Usingwest)
                {
                    return;
                }

                if (dbPlayer.Faction.Id == 5 || dbPlayer.Faction.Id == 6)
                {
                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM inventorys WHERE Id = @userId LIMIT 1");
                    mySqlQuery.AddParameter("@userId", dbPlayer.Id);
                    MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                    try
                    {
                        MySqlDataReader reader = mySqlReaderCon.Reader;
                        try
                        {
                            if (!reader.HasRows)
                            {
                                return;
                            }

                            reader.Read();
                            List<ItemModel> list = new List<ItemModel>();
                            string @string = reader.GetString("Items");
                            list = NAPI.Util.FromJson<List<ItemModel>>(@string);
                            ItemModel itemToUse = list.FirstOrDefault((ItemModel x) => x.Name == "BeamtenSchutzweste");
                            if (itemToUse == null)
                            {
                                return;
                            }


                            int index = list.IndexOf(itemToUse);
                            if (itemToUse.Amount == 1)
                            {
                                list.Remove(itemToUse);
                            }
                            else
                            {
                                itemToUse.Amount--;
                                list[index] = itemToUse;
                            }

                            Item item = itemRegisterList.FirstOrDefault((Item x) => x.Name == itemToUse.Name);
                            reader.Close();
                            if (reader.IsClosed)
                            {
                                mySqlQuery.Query = "UPDATE inventorys SET Items = @invItems WHERE Id = @pId";
                                mySqlQuery.Parameters = new List<MySqlParameter>()
                            {
                                new MySqlParameter("@invItems", NAPI.Util.ToJson(list)),
                                new MySqlParameter("@pId", dbPlayer.Id)
                            };
                                MySqlHandler.ExecuteSync(mySqlQuery);
                                dbPlayer.disableAllPlayerActions(true);
                                dbPlayer.SendProgressbar(4000);
                                dbPlayer.IsFarming = true;
                                dbPlayer.Usingwest = true;
                                dbPlayer.RefreshData(dbPlayer);
                                dbPlayer.PlayAnimation(33, "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 8f);
                                if (dbPlayer.Faction.Id == 6)
                                {


                                    NAPI.Task.Run(delegate
                                    {
                                        if (dbPlayer.Usingwest)
                                        {
                                            dbPlayer.SetArmor(100);
                                            dbPlayer.TriggerEvent("client:respawning");
                                            dbPlayer.StopProgressbar();
                                            dbPlayer.IsFarming = false;
                                            dbPlayer.Usingwest = false;
                                            dbPlayer.RefreshData(dbPlayer);
                                            dbPlayer.disableAllPlayerActions(false);
                                            dbPlayer.SendNotification("Du hast eine LSPD" + "-Schutzweste benutzt.", 3000, dbPlayer.Faction.GetRGBStr());
                                            if (dbPlayer.Factionrank <= 8)
                                            {
                                                // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 7 : 7, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 0);
                                                dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 7 : 7, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 0);
                                            }

                                            else if (dbPlayer.Factionrank == 9)
                                            {
                                                // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 17 : 17, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 8 : 8);
                                                dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 7 : 7, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 0);
                                            }
                                            else if (dbPlayer.Factionrank > 9)
                                            {
                                                // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 28 : 28, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 8 : 8);
                                                dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 7 : 7, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 0);
                                            }
                                            dbPlayer.StopAnimation();
                                        }
                                    }, 4000);
                                }
                                else if (dbPlayer.Faction.Id == 5)
                                {
                                    NAPI.Task.Run(delegate
                                    {
                                        if (dbPlayer.Usingwest)
                                        {
                                            dbPlayer.SetArmor(100);
                                            dbPlayer.TriggerEvent("client:respawning");
                                            dbPlayer.StopProgressbar();
                                            dbPlayer.IsFarming = false;
                                            dbPlayer.Usingwest = false;
                                            dbPlayer.RefreshData(dbPlayer);
                                            dbPlayer.disableAllPlayerActions(false);
                                            //  dbPlayer.SendNotification("Du hast eine LSPD-Schutzweste benutzt.", 3000, "green");
                                            dbPlayer.SendNotification("Du hast eine FIB" + "-Schutzweste benutzt.", 3000, dbPlayer.Faction.GetRGBStr());
                                            if (dbPlayer.Factionrank <= 5)
                                            {
                                                // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 7 : 7, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 0);
                                                dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 3 : 3, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 0);
                                            }


                                            else if (dbPlayer.Factionrank == 6)
                                            {
                                                // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 17 : 17, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 8 : 8);
                                                dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 3 : 3, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 1);
                                            }

                                            else if (dbPlayer.Factionrank <= 9)
                                            {
                                                // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 7 : 7, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 0);
                                                dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 3 : 3, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 1);
                                            }

                                            else if (dbPlayer.Factionrank > 9)
                                            {
                                                // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 28 : 28, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 8 : 8);
                                                dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 3 : 3, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 2 : 2);
                                            }
                                            dbPlayer.StopAnimation();
                                        }
                                    }, 4000);
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
                else
                {
                    if (client == null) return;
                    if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                        return;

                    if (dbPlayer.DeathData.IsDead) return;

                    if (dbPlayer.IsFarming)
                    {
                        return;
                    }

                    if (dbPlayer.Usingwest)
                    {
                        return;
                    }
                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM inventorys WHERE Id = @userId LIMIT 1");
                    mySqlQuery.AddParameter("@userId", dbPlayer.Id);
                    MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);

                    try
                    {
                        MySqlDataReader reader = mySqlReaderCon.Reader;
                        try
                        {
                            if (!reader.HasRows)
                            {
                                return;
                            }

                            if (client.Dimension == 187)
                            {
                                dbPlayer.disableAllPlayerActions(true);
                                dbPlayer.SendProgressbar(4000);
                                dbPlayer.IsFarming = true;
                                dbPlayer.Usingwest = true;
                                dbPlayer.RefreshData(dbPlayer);
                                dbPlayer.PlayAnimation(33, "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 8f);
                                NAPI.Task.Run(delegate
                                {
                                    if (dbPlayer.Usingwest)
                                    {
                                        dbPlayer.SetArmor(100);
                                        dbPlayer.TriggerEvent("client:respawning");
                                        dbPlayer.StopProgressbar();
                                        dbPlayer.IsFarming = false;
                                        dbPlayer.Usingwest = false;
                                        dbPlayer.RefreshData(dbPlayer);
                                        dbPlayer.disableAllPlayerActions(false);
                                        dbPlayer.SendNotification("Du hast eine FFA-Schutzweste benutzt.", 3000, "green");
                                        dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 16 : 15, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 2 : 2);
                                        dbPlayer.StopAnimation();
                                    }
                                }, 4000);
                                return;
                            }

                            reader.Read();
                            List<ItemModel> list = new List<ItemModel>();
                            string @string = reader.GetString("Items");
                            list = NAPI.Util.FromJson<List<ItemModel>>(@string);
                            ItemModel itemToUse = list.FirstOrDefault((ItemModel x) => x.Name == "Schutzweste" || x.Name == "Underarmor");
                            if (itemToUse == null)
                            {
                                return;
                            }

                            if (dbPlayer.Usingwest)
                            {
                                return;
                            }

                            int index = list.IndexOf(itemToUse);
                            if (itemToUse.Amount == 1)
                            {
                                list.Remove(itemToUse);
                            }
                            else
                            {
                                itemToUse.Amount--;
                                list[index] = itemToUse;
                            }

                            Item item = itemRegisterList.FirstOrDefault((Item x) => x.Name == itemToUse.Name);
                            reader.Close();
                            if (reader.IsClosed)
                            {
                                mySqlQuery.Query = "UPDATE inventorys SET Items = @invItems WHERE Id = @pId";
                                mySqlQuery.Parameters = new List<MySqlParameter>()
                            {
                                new MySqlParameter("@invItems", NAPI.Util.ToJson(list)),
                                new MySqlParameter("@pId", dbPlayer.Id)
                            };
                                MySqlHandler.ExecuteSync(mySqlQuery);
                                dbPlayer.disableAllPlayerActions(true);
                                dbPlayer.SendProgressbar(4000);
                                dbPlayer.IsFarming = true;
                                dbPlayer.Usingwest = true;
                                dbPlayer.RefreshData(dbPlayer);
                                dbPlayer.PlayAnimation(33, "anim@heists@narcotics@funding@gang_idle",
                                    "gang_chatting_idle01", 8f);
                                NAPI.Task.Run(delegate
                                {
                                    if (dbPlayer.Usingwest)
                                    {
                                        dbPlayer.SetArmor(100);
                                        dbPlayer.TriggerEvent("client:respawning");
                                        dbPlayer.StopProgressbar();
                                        dbPlayer.IsFarming = false;
                                        dbPlayer.Usingwest = false;
                                        dbPlayer.RefreshData(dbPlayer);
                                        dbPlayer.disableAllPlayerActions(false);
                                        dbPlayer.SendNotification("Du hast eine Schutzweste benutzt.", 3000, "green");
                                        if (!@string.Contains("Underarmor"))
                                        {
                                            dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 16 : 15, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 2 : 2);
                                        }
                                        dbPlayer.StopAnimation();
                                    }
                                }, 4000);
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
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Pressed_PUNKT] " + ex.Message);
                Logger.Print("[EXCEPTION Pressed_PUNKT] " + ex.StackTrace);
            }
        }


        public static bool getItemFunction(Player c, int id)
        {
            if (c == null) return false;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return false;

            var r = new Random();
            string itemm = equipItems[r.Next(0, equipItems.Count)];
            int id3 = new Random().Next(10000, 99999999);
            Item item = itemRegisterList.FirstOrDefault((Item item2) => item2.Id == id);
            if (item == null) return false;

            int paintballModel = dbPlayer.GetData<int>("PBZone");

            if (item.Whash != WeaponHash.Unarmed)
            {
                if (paintballModel != 8888)
                {
                    dbPlayer.SendNotification("Du kannst im Paintball keine Waffen ausrüsten! ", 5000, "red", "");
                }
                else
                {
                    WeaponManager.addWeapon(dbPlayer.player, item.Whash);
                    dbPlayer.SendNotification("Du hast die Waffe ausgerüstet.", 3000, "green", "Inventar");
                }
                return true;
            }
            else if (item.Name == "Schutzweste")
            {
                if (dbPlayer.player.IsInVehicle)
                {
                    return true;
                }

                if (!dbPlayer.IsFarming)
                    if (!dbPlayer.Usingwest)
                    {
                        dbPlayer.disableAllPlayerActions(true);
                        dbPlayer.SendProgressbar(4000);
                        dbPlayer.IsFarming = true;
                        dbPlayer.Usingwest = true;
                        dbPlayer.RefreshData(dbPlayer);
                        dbPlayer.PlayAnimation(33, "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 8f);
                        NAPI.Task.Run(delegate
                        {
                            if (dbPlayer.Usingwest)
                            {
                                dbPlayer.SetArmor(100);
                                dbPlayer.TriggerEvent("client:respawning");
                                dbPlayer.IsFarming = false;
                                dbPlayer.Usingwest = false;
                                dbPlayer.RefreshData(dbPlayer);
                                dbPlayer.StopProgressbar();
                                dbPlayer.disableAllPlayerActions(false);
                                dbPlayer.SendNotification("Du hast eine Schutzweste benutzt.", 3000, "green");
                                dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 16 : 15, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 2 : 2);
                                dbPlayer.StopAnimation();
                            }
                        }, 4000);
                        return true;
                    }
            }
            else if (item.Name == "ScussiWest")
            {
                if (dbPlayer.player.IsInVehicle)
                {
                    return true;
                }

                if (!dbPlayer.IsFarming)
                    if (!dbPlayer.Usingwest)
                    {
                        dbPlayer.disableAllPlayerActions(true);
                        dbPlayer.SendProgressbar(4000);
                        dbPlayer.IsFarming = true;
                        dbPlayer.Usingwest = true;
                        dbPlayer.RefreshData(dbPlayer);
                        dbPlayer.PlayAnimation(33, "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 8f);
                        NAPI.Task.Run(delegate
                        {
                            if (dbPlayer.Usingwest)
                            {
                                dbPlayer.SetArmor(100000);
                                dbPlayer.TriggerEvent("client:respawning");
                                dbPlayer.IsFarming = false;
                                dbPlayer.Usingwest = false;
                                dbPlayer.RefreshData(dbPlayer);
                                dbPlayer.StopProgressbar();
                                dbPlayer.disableAllPlayerActions(false);
                                dbPlayer.SendNotification("Du hast eine Schutzweste benutzt.", 3000, "green");
                                dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 16 : 15, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 2 : 2);
                                dbPlayer.StopAnimation();
                            }
                        }, 4000);
                        return true;
                    }
            }
            else if (item.Name == "Waffenkiste")
            {
                dbPlayer.SendNotification("Waffenkiste geöffnet.", 3000, "green");
                int randomitem = new Random().Next(1, 6);
                int randomanzahl = new Random().Next(1, 9);
                int marksmanorrevolver = new Random().Next(1, 100);

                switch (randomitem)
                {
                    case 1:
                        dbPlayer.UpdateInventoryItems("Advancedrifle", randomanzahl, false);
                        break;
                    case 2:
                        dbPlayer.UpdateInventoryItems("Bullpuprifle", randomanzahl, false);
                        break;
                    case 3:
                        dbPlayer.UpdateInventoryItems("Gusenberg", randomanzahl, false);
                        break;
                    case 4:
                        dbPlayer.UpdateInventoryItems("Schutzweste", randomanzahl, false);
                        if (marksmanorrevolver == 55)
                        {
                            dbPlayer.UpdateInventoryItems("Marksmanrifle", 1, false);
                        }
                        break;
                    case 5:
                        dbPlayer.UpdateInventoryItems("Verbandskasten", randomanzahl, false);
                        if (marksmanorrevolver == 55)
                        {
                            dbPlayer.UpdateInventoryItems("Revolver", 1, false);
                        }
                        break;
                    default:
                        break;
                }
                dbPlayer.UpdateInventoryItems("Waffenkiste", 1, true);
            }
            else if (item.Name == "FastEquipAG")
            {
                dbPlayer.SendNotification("FastEquip Advanced & Gusi geöffnet.", 3000, "green");
                WeaponManager.addWeapon(dbPlayer.player, WeaponHash.Advancedrifle);
                WeaponManager.addWeapon(dbPlayer.player, WeaponHash.Gusenberg);
                WeaponManager.addWeapon(dbPlayer.player, WeaponHash.Heavypistol);
                if (dbPlayer.Faction.Id == 5 || dbPlayer.Faction.Id == 6)
                {
                    dbPlayer.UpdateInventoryItems("BeamtenSchutzweste", 10, false);
                }
                else
                {
                    dbPlayer.UpdateInventoryItems("Schutzweste", 10, false);
                }
                dbPlayer.UpdateInventoryItems("Verbandskasten", 10, false);
                dbPlayer.UpdateInventoryItems("FastEquipAG", 1, true);
            }
            else if (item.Name == "FastEquipAC")
            {
                dbPlayer.SendNotification("FastEquip Advanced & Compact geöffnet.", 3000, "green");
                WeaponManager.addWeapon(dbPlayer.player, WeaponHash.Advancedrifle);
                WeaponManager.addWeapon(dbPlayer.player, WeaponHash.Compactrifle);
                WeaponManager.addWeapon(dbPlayer.player, WeaponHash.Heavypistol);
                if (dbPlayer.Faction.Id == 5 || dbPlayer.Faction.Id == 6)
                {
                    dbPlayer.UpdateInventoryItems("BeamtenSchutzweste", 10, false);
                }
                else
                {
                    dbPlayer.UpdateInventoryItems("Schutzweste", 10, false);
                }
                dbPlayer.UpdateInventoryItems("Verbandskasten", 10, false);
                dbPlayer.UpdateInventoryItems("FastEquipAC", 1, true);
            }
            else if (item.Name == "FastEquipBG")
            {
                dbPlayer.SendNotification("FastEquip Bullpup & Gusi geöffnet.", 3000, "green");
                WeaponManager.addWeapon(dbPlayer.player, WeaponHash.Bullpuprifle);
                WeaponManager.addWeapon(dbPlayer.player, WeaponHash.Gusenberg);
                WeaponManager.addWeapon(dbPlayer.player, WeaponHash.Heavypistol);
                if (dbPlayer.Faction.Id == 5 || dbPlayer.Faction.Id == 6)
                {
                    dbPlayer.UpdateInventoryItems("BeamtenSchutzweste", 10, false);
                }
                else
                {
                    dbPlayer.UpdateInventoryItems("Schutzweste", 10, false);
                }
                dbPlayer.UpdateInventoryItems("Verbandskasten", 10, false);
                dbPlayer.UpdateInventoryItems("FastEquipBG", 1, true);
            }
            else if (item.Name == "FastEquipBC")
            {
                dbPlayer.SendNotification("FastEquip Bullpup & Compact geöffnet.", 3000, "green");
                WeaponManager.addWeapon(dbPlayer.player, WeaponHash.Bullpuprifle);
                WeaponManager.addWeapon(dbPlayer.player, WeaponHash.Compactrifle);
                WeaponManager.addWeapon(dbPlayer.player, WeaponHash.Heavypistol);
                if (dbPlayer.Faction.Id == 5 || dbPlayer.Faction.Id == 6)
                {
                    dbPlayer.UpdateInventoryItems("BeamtenSchutzweste", 10, false);
                }
                else
                {
                    dbPlayer.UpdateInventoryItems("Schutzweste", 10, false);
                }
                dbPlayer.UpdateInventoryItems("Verbandskasten", 10, false);
                dbPlayer.UpdateInventoryItems("FastEquipBC", 1, true);
            }

            else if (item.Name == "BeamtenSchutzweste")
            {
                if (dbPlayer.Faction.Id == 5 || dbPlayer.Faction.Id == 6)
                {
                    if (dbPlayer.player.IsInVehicle)
                    {
                        return true;
                    }

                    dbPlayer.disableAllPlayerActions(true);
                    dbPlayer.SendProgressbar(4000);
                    dbPlayer.IsFarming = true;
                    dbPlayer.Usingwest = true;
                    dbPlayer.RefreshData(dbPlayer);
                    dbPlayer.PlayAnimation(33, "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 8f);
                    if (dbPlayer.Faction.Id == 5)
                    {

                        NAPI.Task.Run(delegate
                        {
                            if (dbPlayer.Usingwest)
                            {
                                dbPlayer.SetArmor(100);
                                dbPlayer.TriggerEvent("client:respawning");
                                dbPlayer.StopProgressbar();
                                dbPlayer.IsFarming = false;
                                dbPlayer.Usingwest = false;
                                dbPlayer.RefreshData(dbPlayer);
                                dbPlayer.disableAllPlayerActions(false);
                                dbPlayer.SendNotification("Du hast eine FIB-Schutzweste benutzt.", 3000, "green");
                                if (dbPlayer.Factionrank <= 5)
                                {
                                    // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 7 : 7, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 0);
                                    dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 3 : 3, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 0);
                                }


                                else if (dbPlayer.Factionrank == 6)
                                {
                                    // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 17 : 17, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 8 : 8);
                                    dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 3 : 3, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 1);
                                }

                                else if (dbPlayer.Factionrank <= 9)
                                {
                                    // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 7 : 7, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 0);
                                    dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 3 : 3, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 1);
                                }

                                else if (dbPlayer.Factionrank > 9)
                                {
                                    // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 28 : 28, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 8 : 8);
                                    dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 3 : 3, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 2 : 2);
                                }
                                dbPlayer.StopAnimation();
                            }
                        }, 4000);
                    }
                    else if (dbPlayer.Faction.Id == 6)
                    {
                        if (dbPlayer.player.IsInVehicle)
                        {
                            return true;
                        }

                        NAPI.Task.Run(delegate
                        {
                            if (dbPlayer.Usingwest)
                            {
                                dbPlayer.SetArmor(100);
                                dbPlayer.TriggerEvent("client:respawning");
                                dbPlayer.StopProgressbar();
                                dbPlayer.IsFarming = false;
                                dbPlayer.Usingwest = false;
                                dbPlayer.RefreshData(dbPlayer);
                                dbPlayer.disableAllPlayerActions(false);
                                dbPlayer.SendNotification("Du hast eine LSPD" + "-Schutzweste benutzt.", 3000, "green");

                                if (dbPlayer.Factionrank <= 8)
                                {
                                    // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 7 : 7, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 0);
                                    dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 7 : 7, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 0);
                                }

                                else if (dbPlayer.Factionrank == 9)
                                {
                                    // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 17 : 17, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 8 : 8);
                                    dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 7 : 7, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 0);
                                }
                                else if (dbPlayer.Factionrank > 9)
                                {
                                    // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 28 : 28, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 8 : 8);
                                    dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 7 : 7, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 0);
                                }
                                dbPlayer.StopAnimation();
                            }
                        }, 4000);
                    }
                    return true;
                }
            }

            else if (item.Name == "Underarmor")
            {
                if (!dbPlayer.IsFarming)
                {
                    if (dbPlayer.player.IsInVehicle)
                    {
                        return true;
                    }

                    dbPlayer.disableAllPlayerActions(true);
                    dbPlayer.SendProgressbar(4000);
                    dbPlayer.IsFarming = true;
                    dbPlayer.RefreshData(dbPlayer);
                    dbPlayer.PlayAnimation(33, "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 8f);
                    NAPI.Task.Run(delegate
                    {
                        if (dbPlayer.Usingwest)
                        {
                            dbPlayer.SetArmor(dbPlayer.GetArmor() + 100);
                            dbPlayer.TriggerEvent("client:respawning");
                            dbPlayer.IsFarming = false;
                            dbPlayer.RefreshData(dbPlayer);
                            dbPlayer.StopProgressbar();
                            dbPlayer.disableAllPlayerActions(false);
                            dbPlayer.SendNotification("Du hast eine Underarmor benutzt.", 3000, "green");
                            // dbPlayer.SetClothes(9, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 16 : 15, (dbPlayer.Faction.Id != 0 && !dbPlayer.Faction.BadFraktion) ? 0 : 2);
                            dbPlayer.StopAnimation();
                        }
                    }, 4000);
                    return true;
                }
            }
            else if (item.Name == "Verbandskasten")
            {
                if (!dbPlayer.IsFarming)
                {
                    if (dbPlayer.player.IsInVehicle)
                    {
                        return true;
                    }

                    dbPlayer.SendProgressbar(4000);
                    dbPlayer.disableAllPlayerActions(true);
                    dbPlayer.IsFarming = true;
                    dbPlayer.RefreshData(dbPlayer);
                    dbPlayer.PlayAnimation(33, "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 8f);
                    NAPI.Task.Run(delegate
                    {
                        dbPlayer.SetHealth(99);
                        dbPlayer.TriggerEvent("client:respawning");
                        dbPlayer.StopAnimation();
                        dbPlayer.disableAllPlayerActions(false);
                        dbPlayer.StopProgressbar();
                        dbPlayer.IsFarming = false;
                        dbPlayer.RefreshData(dbPlayer);
                        dbPlayer.SendNotification("Du hast einen Verbandskasten benutzt.", 3000, "green");
                    }, 4000);
                    return true;
                }
            }
            else if (item.Name == "Mietvertrag")
            {
                House house = HouseModule.houses.FirstOrDefault((House house2) => house2.OwnerId == dbPlayer.Id);
                if (house == null)
                {
                    dbPlayer.SendNotification("Du besitzt kein Haus!", 3000, "red");
                    return false;
                }

                dbPlayer.OpenTextInputBox(new TextInputBoxObject
                {
                    Title = "Mietvertrag",
                    Message = "Gebe bitte den Namen des Spielers ein, dem du gerne einen Mietvertrag machen möchtest.",
                    Callback = "sendMietvertrag"
                });
                return true;
            }
            else if (item.Name == "Notizblock")
            {
                try
                {
                    if (c == null) return true;
                    if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                        return true;

                    if (dbPlayer.DeathData.IsDead) return true;

                    if (dbPlayer.IsFarming)
                    {
                        return true;
                    }

                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM inventorys WHERE Id = @userId LIMIT 1");
                    mySqlQuery.AddParameter("@userId", dbPlayer.Id);
                    MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                    try
                    {
                        MySqlDataReader reader = mySqlReaderCon.Reader;
                        try
                        {
                            if (!reader.HasRows)
                            {
                                return true;
                            }

                            reader.Read();
                            List<ItemModel> list = new List<ItemModel>();
                            string @string = reader.GetString("Items");
                            list = NAPI.Util.FromJson<List<ItemModel>>(@string);
                            ItemModel itemToUse = list.FirstOrDefault((ItemModel x) => x.Name == "Notizblock");
                            if (itemToUse == null)
                            {
                                return true;
                            }

                            int index = list.IndexOf(itemToUse);
                            if (itemToUse.Amount == 1)
                            {
                                list.Remove(itemToUse);
                            }
                            else
                            {
                                itemToUse.Amount--;
                                list[index] = itemToUse;
                            }


                            reader.Close();
                            if (reader.IsClosed)
                            {
                                mySqlQuery.Query = "UPDATE inventorys SET Items = @invItems WHERE Id = @pId";
                                mySqlQuery.Parameters = new List<MySqlParameter>()
                            {
                                new MySqlParameter("@invItems", NAPI.Util.ToJson(list)),
                                new MySqlParameter("@pId", dbPlayer.Id)
                            };
                                MySqlHandler.ExecuteSync(mySqlQuery);
                                if (!c.IsInVehicle)
                                {
                                    return true;
                                }
                                dbPlayer.OpenTextInputBox(new TextInputBoxObject
                                {
                                    Title = "Notiz",
                                    Message = "Gib eine Notiz ein (15 Zeichen) Nur Buchstaben und Zahlen",
                                    Callback = "setnotiz"
                                });


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
                    Logger.Print("[EXCEPTION Pressed_KOMMA] " + ex.Message);
                    Logger.Print("[EXCEPTION Pressed_KOMMA] " + ex.StackTrace);
                }
            }
            else if (item.Name == "Nummernschild")
            {
                Player client = dbPlayer.player;
                if (client.IsInVehicle)
                {
                    TextInputBoxObject textInputBoxObject = new TextInputBoxObject
                    {
                        Title = "Nummernschild anlegen",
                        Message =
                    "Gebe dein Wunschkennzeichen ein und achte darauf das dein Kennzeichen nicht zu lang ist!",
                        Callback = "registerPlate"
                    };

                    c.OpenTextInputBox(textInputBoxObject);

                }
                else
                {
                    dbPlayer.SendNotification("Du bist in keinem Fahrzeug!", 3000, "red");
                }
                return true;
            }

            else if (item.Name == "Magazin")
            {
                Player client = dbPlayer.player;
                if (NAPI.Player.GetPlayerCurrentWeapon(client) != WeaponHash.Unarmed)
                {
                    WeaponHash weapon = client.CurrentWeapon;
                    dbPlayer.TriggerEvent("sendProgressbar", new object[1]
                    {
                    5000
                    });
                    dbPlayer.TriggerEvent("disableAllPlayerActions", new object[1]
                    {
                    true
                    });
                    //NAPI.Player.PlayPlayerAnimation(p, 33, "ac_ig_3_p3_b-0", "w_ar_assaultrifle_mag1-0", 8);
                    dbPlayer.PlayAnimation(33, "weapons@submg@micro_smg_str", "reload_aim", 8f);
                    NAPI.Task.Run(delegate
                    {
                        dbPlayer.GiveWeapon(weapon, 9999);
                        dbPlayer.SendNotification("Du hast 1 Magazin benutzt, deine Waffe ist nun wieder voll!", 3000, "green");
                        dbPlayer.TriggerEvent("disableAllPlayerActions", new object[1]
                        {
                            false
                        });
                        dbPlayer.StopAnimation();
                        dbPlayer.ResetData("IS_FARMING");

                    }, 5000);
                }
                else
                {
                    dbPlayer.SendNotification("Du musst eine Waffe in der Hand halten!", 3000, "red");
                }
            }

            else if (item.Name == "Nahkampf")
            {
                if (!dbPlayer.IsFarming || !c.IsInVehicle)
                {
                    if (dbPlayer.Faction.Name == "Marabunta Grande 13")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Machete);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "Front Yard Ballas")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Bat);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "Los Santos Vagos")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Machete);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "Sicario Cartel")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Machete);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "Medellín Cartel")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Wrench);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "Triaden Mafia")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Poolcue);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "Brigada")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Stone_hatchet);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "Black Gangster Disciples")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Knife);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "Bratwa")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Hatchet);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "Grove Street Families")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Bat);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "Grapeseed Rednecks")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Hammer);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "La Cosa Nostra")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Golfclub);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "Yakuza")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Switchblade);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "The Lost MC")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Bottle);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "Los Santos Police Department")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Nightstick);
                        return true;
                    }
                    if (dbPlayer.Faction.Name == "Federal Investigation Bureau")
                    {
                        WeaponManager.addWeapon(c, WeaponHash.Nightstick);
                        return true;
                    }
                    else
                    {
                        dbPlayer.SendNotification("Du bist nicht in der Fraktion.");
                    }
                }
                return true;
            }


            else if (item.Name == "Louis")
            {
                Player client = dbPlayer.player;
                {
                    //NAPI.Player.PlayPlayerAnimation(p, 33, "ac_ig_3_p3_b-0", "w_ar_assaultrifle_mag1-0", 8);
                    dbPlayer.PlayAnimation(33, "rcmpaparazzo_2", "shag_loop_poppy", 8f);
                }
            }


            else if (item.Name == "Geschenk")
            {
                try
                {
                    if (c == null) return true;
                    if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                        return true;

                    if (dbPlayer.DeathData.IsDead) return true;

                    if (dbPlayer.IsFarming)
                    {
                        return true;
                    }

                    MySqlQuery mySqlQuery2 = new MySqlQuery("SELECT * FROM inventorys WHERE Id = @userId LIMIT 1");
                    mySqlQuery2.AddParameter("@userId", dbPlayer.Id);
                    MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery2);
                    try
                    {
                        MySqlDataReader reader = mySqlReaderCon.Reader;
                        try
                        {
                            if (!reader.HasRows)
                            {
                                return true;
                            }

                            reader.Read();
                            List<ItemModel> list2 = new List<ItemModel>();
                            string @string = reader.GetString("Items");
                            list2 = NAPI.Util.FromJson<List<ItemModel>>(@string);
                            ItemModel itemToUse = list2.FirstOrDefault((ItemModel x) => x.Name == "Geschenk");
                            if (itemToUse == null)
                            {
                                return true;
                            }

                            int index = list2.IndexOf(itemToUse);
                            if (itemToUse.Amount == 1)
                            {
                                list2.Remove(itemToUse);
                            }
                            else
                            {
                                itemToUse.Amount--;
                                list2[index] = itemToUse;
                            }


                            reader.Close();
                            if (reader.IsClosed)
                            {
                                mySqlQuery2.Query = "UPDATE inventorys SET Items = @invItems WHERE Id = @pId";
                                mySqlQuery2.Parameters = new List<MySqlParameter>()
                            {
                                new MySqlParameter("@invItems", NAPI.Util.ToJson(list2)),
                                new MySqlParameter("@pId", dbPlayer.Id)
                            };
                                MySqlHandler.ExecuteSync(mySqlQuery2);
                                List<int> list = new List<int>();
                                list.Add(dbPlayer.Id);
                                MySqlQuery mySqlQuery = new MySqlQuery("INSERT INTO `vehicles` (`Id`, `Vehiclehash`, `Parked`, `OwnerId`, `Carkeys`, `Plate`) VALUES (@id, @vehiclehash, @parked, @ownerid, @carkeys, @plate)");
                                mySqlQuery.AddParameter("@vehiclehash", itemm);
                                mySqlQuery.AddParameter("@parked", 1);
                                mySqlQuery.AddParameter("@ownerid", dbPlayer.Id);
                                mySqlQuery.AddParameter("@carkeys", NAPI.Util.ToJson(list));
                                mySqlQuery.AddParameter("@plate", id3);
                                mySqlQuery.AddParameter("@id", id3);
                                MySqlHandler.ExecuteSync(mySqlQuery);
                                dbPlayer.SendNotification("Du hast aus deinem Geschenk das Fahrzeug " + itemm + " bekommen!", 3000, "red");


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
                    Logger.Print("[EXCEPTION Pressed_KOMMA] " + ex.Message);
                    Logger.Print("[EXCEPTION Pressed_KOMMA] " + ex.StackTrace);
                }
            }

            else if (item.Name == "Wuerfel")
            {
                int nrm = new Random().Next(1, 200);
                dbPlayer.disableAllPlayerActions(true);
                dbPlayer.IsFarming = true;
                dbPlayer.RefreshData(dbPlayer);
                dbPlayer.PlayAnimation(33, "mp_player_int_upperwank", "mp_player_int_wank_01");
                //Task.Delay(2000);
                NAPI.Task.Run(delegate
                {
                    dbPlayer.IsFarming = false;
                    dbPlayer.StopAnimation();
                    dbPlayer.disableAllPlayerActions(false);
                    dbPlayer.RefreshData(dbPlayer);
                    NAPI.ClientEvent.TriggerClientEventInRange(dbPlayer.player.Position, 100.0f, "sendPlayerNotification", "* " + dbPlayer.Name + " rollt die Würfel und bekommt eine " + nrm + ".", 5500, "black", "", "");
                }, 2000);

            }

            else if (item.Name == "Kriegsvertrag")
            {
                try
                {
                    if (c == null) return true;
                    if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                        return true;

                    if (dbPlayer.DeathData.IsDead) return true;

                    if (dbPlayer.IsFarming)
                    {
                        return true;
                    }

                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM inventorys WHERE Id = @userId LIMIT 1");
                    mySqlQuery.AddParameter("@userId", dbPlayer.Id);
                    MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                    try
                    {
                        MySqlDataReader reader = mySqlReaderCon.Reader;
                        try
                        {
                            if (!reader.HasRows)
                            {
                                return true;
                            }

                            reader.Read();
                            List<ItemModel> list = new List<ItemModel>();
                            string @string = reader.GetString("Items");
                            list = NAPI.Util.FromJson<List<ItemModel>>(@string);
                            ItemModel itemToUse = list.FirstOrDefault((ItemModel x) => x.Name == "Kriegsvertrag");
                            if (itemToUse == null)
                            {
                                return true;
                            }

                            int index = list.IndexOf(itemToUse);
                            if (itemToUse.Amount == 1)
                            {
                                list.Remove(itemToUse);
                            }
                            else
                            {
                                itemToUse.Amount--;
                                list[index] = itemToUse;
                            }


                            reader.Close();
                            if (reader.IsClosed)
                            {
                                mySqlQuery.Query = "UPDATE inventorys SET Items = @invItems WHERE Id = @pId";
                                mySqlQuery.Parameters = new List<MySqlParameter>()
                            {
                                new MySqlParameter("@invItems", NAPI.Util.ToJson(list)),
                                new MySqlParameter("@pId", dbPlayer.Id)
                            };
                                MySqlHandler.ExecuteSync(mySqlQuery);
                                if (dbPlayer.Factionrank <= 12)
                                {
                                    return true;
                                }
                                dbPlayer.OpenTextInputBox(new TextInputBoxObject
                                {
                                    Title = "Kriegsvertrag",
                                    Message = "Bitte gebe den Namen des Leaders der anderen Fraktion an!",
                                    Callback = "sendvertrag"
                                });


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
                    Logger.Print("[EXCEPTION Pressed_KOMMA] " + ex.Message);
                    Logger.Print("[EXCEPTION Pressed_KOMMA] " + ex.StackTrace);
                }
            }


            else if (item.Name == "Schweissgeraet")
            {
                return Schweissgeraet.useSchweissgeraet(dbPlayer);
            }
            return false;
        }

        [RemoteEvent("registerPlate")]
        public static void RegisterPlate(Player c, string plate)
        {
            try
            {
                DbPlayer dbPlayer = c.GetPlayer();
                bool flag = false;
                if (c.IsInVehicle)
                    flag = true;

                if (plate.Length > 5)
                    flag = false;

                if (plate != plate.RemoveSpecialCharacters())
                    flag = false;

                if (!flag)
                {
                    c.SendNotification("Dein Nummernschild darf nur 5 Zeichen lang sein. Beispiel: EINS", true);
                    TextInputBoxObject textInputBoxObject = new TextInputBoxObject
                    {
                        Title = "Nummernschild anlegen",
                        Message =
                        "Gebe dein Wunschkennzeichen ein und achte darauf das dein Kennzeichen nicht zu lang ist!",
                        Callback = "registerPlate"
                    };

                    c.OpenTextInputBox(textInputBoxObject);
                }
                else
                {
                    dbPlayer.SendNotification("Du hast dein Fahrzeug angemeldet! Nummernschild: " + plate + "");
                    //         WebhookSender.SendMessage("NUMMERNSCHILD", "" + c.Name + " -> " + plate + " - NUMMERNSCHILD", Webhooks.hauslagerlogs, "Text");
                    Player client = dbPlayer.player;
                    DbVehicle dbVehicle = client.Vehicle.GetVehicle();
                    if (dbVehicle == null || !dbVehicle.IsValid() || dbVehicle.Vehicle == null)
                        return;
                    if (!client.IsInVehicle) return;
                    MySqlQuery mySqlQuery = new MySqlQuery("UPDATE vehicles SET Plate = @plate WHERE Id = @id");
                    mySqlQuery.AddParameter("@id", dbVehicle.Id);
                    mySqlQuery.AddParameter("@plate", plate);
                    MySqlHandler.ExecuteSync(mySqlQuery);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION openVehicleShop] " + ex.Message);
                Logger.Print("[EXCEPTION openVehicleShop] " + ex.StackTrace);
            }
        }
    }
}
