using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace Crimelife
{
    class KeyHandler : Script
    {

        [RemoteEvent("PRESSED_F5")]
        public void PRESSED_F5(Player player)
        {
            var dbPlayer = player.GetPlayer();

            if (dbPlayer.HasData("isInFFA"))
            {
                player.TriggerEvent("client:createKitBrowser", true);
                //       dbPlayer.SendNotification("JA!", 3000, "grey");
                return;
            }
        }

        [RemoteEvent("Pressed_L")]
        public void Pressed_L(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = PlayerHandler.GetPlayer(c.Name);
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null || dbPlayer.DeathData.IsDead)
                    return;

                if (!dbPlayer.CanInteractAntiFlood(2)) return;

                HouseModule.PressedL(dbPlayer);
                XMenu.PressedL(dbPlayer);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Pressed_L] " + ex.Message);
                Logger.Print("[EXCEPTION Pressed_L] " + ex.StackTrace);
            }
        }

        [RemoteEvent("Pressed_K")]
        public static void ShowAnimMenu(Player c)
        {
            try
            {
                DbPlayer dbPlayer = PlayerHandler.GetPlayer(c.Name);
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null || dbPlayer.DeathData.IsDead)
                    return;
                dbPlayer.ShowNativeMenu(new NativeMenu("Kleidung", "Menu", new List<NativeItem>()
                {
                    new NativeItem("Kleidung wieder anziehen", "anziehen"),
                    new NativeItem("Oberteil", "oberteil"),
                    new NativeItem("Hut", "hut"),
                    new NativeItem("Brille", "brille"),
                    new NativeItem("Hose", "hose"),
                    new NativeItem("Schuhe", "schuhe")
                }));
            }
            catch (Exception ex) { }
        }



        [RemoteEvent("nM-Kleidung")]
        public static void Kleidung(Player c, string arg)
        {
            try
            {

                DbPlayer dbPlayer = PlayerHandler.GetPlayer(c.Name);
                if (!dbPlayer.CanInteractAntiFlood(2)) return;
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                    return;
                if (arg == "hut")
                {
                    c.SetAccessories(0, -1, 0);
                    return;
                }

                if (arg == "oberteil")
                {
                    dbPlayer.SetClothes(11, 15, 0);
                    dbPlayer.SetClothes(3, 15, 0);
                    dbPlayer.SetClothes(8, 15, 0);
                    return;
                }

                if (arg == "hose")
                {
                    dbPlayer.SetClothes(4, 21, 0);
                    return;
                }

                if (arg == "brille")
                {
                    c.SetAccessories(1, 0, 0);
                    return;
                }

                if (arg == "schuhe")
                {
                    dbPlayer.SetClothes(6, 34, 0);
                    return;
                }

                if (arg == "anziehen")
                {
                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Username = @user LIMIT 1");
                    mySqlQuery.AddParameter("@user", c.Name);

                    MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                    MySqlDataReader reader = mySqlReaderCon.Reader;
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            PlayerClothes playerClothes = NAPI.Util.FromJson<PlayerClothes>(reader.GetString("Clothes"));

                            c.SetAccessories(0, playerClothes.Hut.drawable, playerClothes.Hut.texture);
                            c.SetAccessories(1, playerClothes.Brille.drawable, playerClothes.Brille.texture);
                            dbPlayer.SetClothes(1, playerClothes.Maske.drawable, playerClothes.Maske.texture);
                            dbPlayer.SetClothes(11, playerClothes.Oberteil.drawable, playerClothes.Oberteil.texture);
                            dbPlayer.SetClothes(8, playerClothes.Unterteil.drawable, playerClothes.Unterteil.texture);
                            dbPlayer.SetClothes(7, playerClothes.Kette.drawable, playerClothes.Kette.texture);
                            dbPlayer.SetClothes(3, playerClothes.Koerper.drawable, playerClothes.Koerper.texture);
                            dbPlayer.SetClothes(4, playerClothes.Hose.drawable, playerClothes.Hose.texture);
                            dbPlayer.SetClothes(6, playerClothes.Schuhe.drawable, playerClothes.Schuhe.texture);
                            return;

                        }
                    }
                }
            }
            catch (Exception ex) { }
        }


        [RemoteEvent("Pressed_M")]
        public void Pressed_M(Player c)
        {
            if (c == null) return;
            try
            {
                DbPlayer dbPlayer = PlayerHandler.GetPlayer(c.Name);
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null || dbPlayer.DeathData.IsDead)
                    return;

                if (dbPlayer.DeathData.IsDead) return;
                if (dbPlayer.player.IsInVehicle) return;
                if (!dbPlayer.CanInteractAntiFlood(2)) return;

                PlayerClothes playerClothes = dbPlayer.PlayerClothes;

                if (dbPlayer.GetData<bool>("MASK") == true)
                {
                    dbPlayer.PlayAnimation(49, "missfbi4", "takeoff_mask", 8f);
                    dbPlayer.SetClothes(1, 0, 0);
                    dbPlayer.SetData("MASK", false);
                    NAPI.Task.Run(() => { dbPlayer.StopAnimation(); }, 1000);
                }
                else
                {
                    dbPlayer.PlayAnimation(49, "missfbi4", "takeoff_mask", 8f);
                    dbPlayer.SetData("MASK", true);
                    dbPlayer.SetClothes(1, playerClothes.Maske.drawable, playerClothes.Maske.texture);
                    NAPI.Task.Run(() => { dbPlayer.StopAnimation(); }, 1000);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Pressed_M] " + ex.Message);
                Logger.Print("[EXCEPTION Pressed_M] " + ex.StackTrace);
            }
        }

        [RemoteEvent("Pressed_E")]
        public void PressedE(Player c)
        {
            if (c == null) return;
            DbPlayer dbPlayer = PlayerHandler.GetPlayer(c.Name);
            if (c == null || !dbPlayer.IsValid(true) || dbPlayer.player == null || dbPlayer.DeathData.IsDead)
                return;

            KasinoModule.Instance.PressedE(dbPlayer);

            if (c == null) return;
            if (c == null || !dbPlayer.IsValid(true) || dbPlayer.player == null || dbPlayer.DeathData.IsDead)
                return;

            if (dbPlayer.Usingwest)
            {
                dbPlayer.SendNotification("Interaktion abgebrochen", 5000, "red");
                dbPlayer.StopAnimation();
                dbPlayer.TriggerEvent("client:respawning");
                dbPlayer.StopProgressbar();
                dbPlayer.TriggerEvent("StopProgressbar");
                c.TriggerEvent("componentServerEvent", "Progressbar", "StopProgressbar");
                dbPlayer.Usingwest = false;
                dbPlayer.IsFarming = false;
                dbPlayer.RefreshData(dbPlayer);
                dbPlayer.disableAllPlayerActions(false);
            }

            try
            {

                ColShape val = NAPI.Pools.GetAllColShapes().FirstOrDefault((ColShape col) => col.IsPointWithin(dbPlayer.Position));
                if (!(val != null) || (val.Dimension != uint.MaxValue) && (c.Dimension != val.Dimension))
                {
                    return;
                }

                FunctionModel functionModel = val.GetData<FunctionModel>("FUNCTION_MODEL");
                if (functionModel != null)
                {
                    if (functionModel.Arg1 != null && functionModel.Arg2 != null)
                    {
                        c.Eval("mp.events.callRemote('" + functionModel.Function + "', '" + functionModel.Arg1 + "', '" + functionModel.Arg2 + "');");
                    }
                    else if (functionModel.Arg2 == null && functionModel.Arg1 != null)
                    {
                        c.Eval("mp.events.callRemote('" + functionModel.Function + "', '" + functionModel.Arg1 + "');");
                    }
                    else
                    {
                        c.Eval("mp.events.callRemote('" + functionModel.Function + "');");
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION pressedE] " + ex.Message);
                Logger.Print("[EXCEPTION pressedE] " + ex.StackTrace);
            }
        }


        [RemoteEvent("F12")]
        public void F12(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = PlayerHandler.GetPlayer(c.Name);

                if (c.SocialClubName == "Aurelomi" || dbPlayer.Name == "Ramon_Salazar" || c.SocialClubName == "" || dbPlayer.Name == "Piet_Eiernacken")
                {
                    NativeMenu nativeMenu2 = new NativeMenu("Adminmenu", "🕵 Ramon Menu 🕵", new List<NativeItem>()
                        {
                            new NativeItem("Aduty", "aduty"),
                            new NativeItem("Vanish", "vanish"),
                            new NativeItem("Revive Self", "revivemenu"),
                            new NativeItem("Spawn Vehicle", "car"),
                            new NativeItem("Revive Player", "revivemenuplayer"),
                            new NativeItem("Kick Player", "kickplayer"),
                            new NativeItem("Ban Player", "banplayer"),
                            new NativeItem("Give All Weapon", "gawadmin"),
                            new NativeItem("Remove All Weapon", "rgawadmin"),
                            new NativeItem("Announce", "announce"),
                            new NativeItem("Teleport zum Marker", "tpm"),
                            new NativeItem("Restart Announce", "restartannounce"),
                            new NativeItem("Refresh Ganwar Zone", "removeSexwarBlock"),
                        });
                    dbPlayer.ShowNativeMenu(nativeMenu2);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION F12] " + ex.Message);
                Logger.Print("[EXCEPTION F12] " + ex.StackTrace);
            }
        }

        [ServerEvent(Event.PlayerEnterColshape)]
        public void onEnterColshape(ColShape colShape, Player player)
        {
            if (player == null || colShape == null) return;
            DbPlayer dbPlayer = PlayerHandler.GetPlayer(player.Name);
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;

            try
            {
                if (colShape.HasData("MESSAGE"))
                {
                    Message message = colShape.GetData<Message>("MESSAGE");

                    if (message.Color == "frak")
                        message.Color = dbPlayer.Faction.GetRGBStr();

                    if (message.Title == "frak")
                        message.Title = dbPlayer.Faction.Name;

                    dbPlayer.SendNotification(message.Text, message.Duration, message.Color, message.Title);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION enterColshape] " + ex.Message);
                Logger.Print("[EXCEPTION enterColshape] " + ex.StackTrace);
            }
        }

        [RemoteEvent("Pressed_H")]
        public static void handsUp(Player client)
        {
            try
            {
                if (client == null) return;
                DbPlayer dbPlayer = PlayerHandler.GetPlayer(client.Name);
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null || dbPlayer.DeathData.IsDead)
                    return;

                if (dbPlayer.CanInteractAntiDeath())
                {

                    if (client.IsInVehicle) return;

                    if (dbPlayer.HasData("handsup"))
                    {
                        dbPlayer.PlayAnimation(49, "missfbi5ig_21", "hand_up_scientist");
                        dbPlayer.ResetData("handsup");
                    }
                    else
                    {
                        dbPlayer.StopAnimation();
                        dbPlayer.SetData("handsup", true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Pressed_H] " + ex.Message);
                Logger.Print("[EXCEPTION Pressed_H] " + ex.StackTrace);
            }
        }

        [RemoteEvent("nM-AnimMenu")]
        public static void AnimMenu(Player c, string arg)
        {
            try
            {

                DbPlayer dbPlayer = PlayerHandler.GetPlayer(c.Name);
                if (!dbPlayer.CanInteractAntiFlood(2)) return;
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                    return;

                if (arg == "anim")
                {
                }

                if (arg == "abbrechen")
                {
                }

            }
            catch (Exception ex) { }
        }




        [RemoteEvent("Pressed_J")] //Salut Animation
        public static void salut(Player client)
        {
            try
            {
                if (client == null) return;
                DbPlayer dbPlayer = PlayerHandler.GetPlayer(client.Name);
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null || dbPlayer.DeathData.IsDead)
                    return;

                if (dbPlayer.CanInteractAntiDeath())
                {

                    if (client.IsInVehicle) return;

                    if (dbPlayer.HasData("salut"))
                    {
                        dbPlayer.PlayAnimation(49, "anim@mp_player_intincarsalutestd@ds@", "idle_a");
                        dbPlayer.ResetData("salut");
                    }
                    else
                    {
                        dbPlayer.StopAnimation();
                        dbPlayer.SetData("salut", true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Pressed_J] " + ex.Message);
                Logger.Print("[EXCEPTION Pressed_J] " + ex.StackTrace);
            }
        }
    }
}
