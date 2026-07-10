using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkAPI;
using MySql.Data.MySqlClient;

namespace Crimelife
{
    class SettingsApp : Crimelife.Module.Module<SettingsApp>
    {

        List<Ringtone> ringtones = new List<Ringtone>()
        {
            new Ringtone(1, "K-Crimelife", "2"),
            new Ringtone(2, "La Cosa Nostra", "https://cdn.discordapp.com/attachments/1085640874626650254/1085647811766386729/Judgement_NEw_Clean_2.mp3"),
            new Ringtone(3, "Lost MC", "https://cdn.discordapp.com/attachments/1085640874626650254/1085647811766386729/Judgement_NEw_Clean_2.mp3"),
};

        List<Wallpaper> wallpapers = new List<Wallpaper>()
        {
            new Wallpaper(1, "K-Crimelife", "https://cdn.discordapp.com/attachments/791769614152892436/1084421087439376385/kwallpaper.png"),
            new Wallpaper(2, "La Cosa Nostra", "https://cdn.discordapp.com/attachments/791769614152892436/1084408297177747566/lcnwallpaper.png"),
            new Wallpaper(3, "Lost MC", "https://cdn.discordapp.com/attachments/791769614152892436/1084415144446140426/lostmcwallpaper.png"),
            new Wallpaper(4, "LSPD", "https://cdn.discordapp.com/attachments/791769614152892436/1084425694542573648/lspdwallpaper.png"),
            new Wallpaper(5, "Marabunta Grande", "https://cdn.discordapp.com/attachments/791769614152892436/1084397937251065866/mg13wallpaper.png"),
            new Wallpaper(6, "Sicario Cartel", "https://cdn.discordapp.com/attachments/956072301005266945/1085732917117997157/SicarioWallpaper.png"),
            new Wallpaper(7, "Yakuza", "https://cdn.discordapp.com/attachments/791769614152892436/1084411448333172747/yakuzawallpaper3.png"),
            new Wallpaper(8, "Triaden Mafia", "https://cdn.discordapp.com/attachments/791769614152892436/1084404138466627594/triadenwallpaper5.png"),
            new Wallpaper(9, "Los Santos Vagos", "https://cdn.discordapp.com/attachments/791769614152892436/1084394897274056774/vagoswallpaper.png"),
            new Wallpaper(10, "Front Yard Ballas", "https://cdn.discordapp.com/attachments/791769614152892436/1084400571735359498/ballaswallpaper2.png"),
            new Wallpaper(11, "Grove Street Families", "https://cdn.discordapp.com/attachments/791769614152892436/1084407634771320912/grovewallpaper5.png"),
            new Wallpaper(12, "Hxxver Crips", "https://cdn.discordapp.com/attachments/791769614152892436/1084427083721216010/hoover2.png"),
            new Wallpaper(13, "Organisazija", "https://cdn.discordapp.com/attachments/956072301005266945/1085737444621684766/Screenshot_366.png"),
            new Wallpaper(14, "IAA", "https://cdn.discordapp.com/attachments/791769614152892436/1084427990869479424/iaawallpaper.png"),           
            new Wallpaper(20, "La Cosa Nostra Gif", "https://cdn.discordapp.com/attachments/956072301005266945/1085744777250754560/oie_dXsGFj98Q2Zq.gif"),
            new Wallpaper(21, "Hxxver Crips 2", "https://cdn.discordapp.com/attachments/956072301005266945/1085737447016644618/Screenshot_358.png"),
            new Wallpaper(22, "Lost MC 2", "https://cdn.discordapp.com/attachments/956072301005266945/1085737446702075934/Screenshot_359.png"),
            new Wallpaper(23, "Front Yard Ballas", "https://cdn.discordapp.com/attachments/956072301005266945/1085737446446207026/Screenshot_360.png"),
            new Wallpaper(24, "Midnight Club", "https://cdn.discordapp.com/attachments/956072301005266945/1085737446165200987/Screenshot_361.png"),
            new Wallpaper(25, "Los Santos Vagos 2", "https://cdn.discordapp.com/attachments/956072301005266945/1085737445846437988/Screenshot_362.png"),
            new Wallpaper(26, "Los Santos Vagos 3", "https://cdn.discordapp.com/attachments/956072301005266945/1027419659437363240/639-f1e59e7d.png"),
            new Wallpaper(27, "Triaden Mafia 2", "https://cdn.discordapp.com/attachments/956072301005266945/1085737445846437988/Screenshot_362.png"),
            new Wallpaper(28, "LSPD 2", "https://cdn.discordapp.com/attachments/956072301005266945/1085737445154373693/Screenshot_364.png"),
            new Wallpaper(29, "FIB", "https://cdn.discordapp.com/attachments/956072301005266945/1085737444894330900/Screenshot_365.png"),           
};


        [RemoteEvent("requestWallpaperList")]
        public void requestWallpaperList(Player c)
        {

            try
            {
                if (c == null) return;
                c.TriggerEvent("componentServerEvent", "SettingsEditWallpaperApp", "responseWallpaperList", NAPI.Util.ToJson(wallpapers));
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION requestWallpaperList] " + ex.Message);
                Logger.Print("[EXCEPTION requestWallpaperList] " + ex.StackTrace);
            }
        }

        [RemoteEvent("requestPhoneWallpaper")]
        public void requestPhoneWallpaper(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                    return;

                checkUserSettingsTable(c);
                MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1");
                mySqlQuery.AddParameter("@userid", dbPlayer.Id);
                MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                try
                {
                    MySqlDataReader reader = mySqlReaderCon.Reader;
                    try
                    {
                        if (!reader.HasRows)
                        {
                            mySqlQuery.Parameters.Clear();
                            mySqlQuery.Query = "INSERT INTO phone_settings (Id) VALUES (@userid)";
                            mySqlQuery.AddParameter("@userid", dbPlayer.Id);
                            MySqlHandler.ExecuteSync(mySqlQuery);
                        }
                        else
                        {
                            reader.Read();
                            Wallpaper wallpaper = wallpapers.FirstOrDefault((Wallpaper wall) => wall.Id == reader.GetInt32("Wallpaper"));

                            if (wallpaper != null)
                                c.TriggerEvent("componentServerEvent", "HomeApp", "responsePhoneWallpaper", wallpaper.File);
                        }
                    }
                    finally
                    {
                        reader.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION requestPhoneWallpaper] " + ex.Message);
                    Logger.Print("[EXCEPTION requestPhoneWallpaper] " + ex.StackTrace);
                }
                finally
                {
                    mySqlReaderCon.Connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION requestPhoneWallpaper] " + ex.Message);
                Logger.Print("[EXCEPTION requestPhoneWallpaper] " + ex.StackTrace);
            }

        }


        [RemoteEvent("saveWallpaper")]
        public void saveWallpaper(Player c, int id)
        {
            if (c == null) return;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;

            checkUserSettingsTable(c);

            try
            {
                MySqlQuery mySqlQuery = new MySqlQuery("UPDATE phone_settings SET Wallpaper = @val WHERE Id = @userid");
                mySqlQuery.AddParameter("@userid", dbPlayer.Id);
                mySqlQuery.AddParameter("@val", id);
                MySqlHandler.ExecuteSync(mySqlQuery);
                Wallpaper wallpaper = wallpapers.FirstOrDefault();
                dbPlayer.SendNotification($"Wallpaper {wallpaper.Name} gespeichert.", 3000, "");
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION saveWallpaper] " + ex.Message);
                Logger.Print("[EXCEPTION saveWallpaper] " + ex.StackTrace);
            }
        }


        [RemoteEvent("requestRingtoneList")]
        public void requestRingtoneList(Player c)
        {

            try
            {
                if (c == null) return;
                c.TriggerEvent("componentServerEvent", "SettingsEditRingtonesApp", "responseRingtoneList", NAPI.Util.ToJson(ringtones));
                Logger.Print("RequestRingtoneList " + NAPI.Util.ToJson(ringtones));
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION requestWallpaperList] " + ex.Message);
                Logger.Print("[EXCEPTION requestWallpaperList] " + ex.StackTrace);
            }
        }

        [RemoteEvent("setCurrentRingtone")]
        public void setCurrentRingtone(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                    return;

                checkUserSettingsTable(c);
                MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1");
                mySqlQuery.AddParameter("@userid", dbPlayer.Id);
                MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                try
                {
                    MySqlDataReader reader = mySqlReaderCon.Reader;
                    try
                    {
                        if (!reader.HasRows)
                        {
                            mySqlQuery.Parameters.Clear();
                            mySqlQuery.Query = "INSERT INTO phone_settings (Id) VALUES (@userid)";
                            mySqlQuery.AddParameter("@userid", dbPlayer.Id);
                            MySqlHandler.ExecuteSync(mySqlQuery);
                        }
                        else
                        {
                            reader.Read();
                            Ringtone ringtone = ringtones.FirstOrDefault((Ringtone ringtone1) => ringtone1.Id == reader.GetInt32("Ringtone"));

                            if (ringtone != null)
                                c.TriggerEvent("componentServerEvent", "HomeApp", "setCurrentRingtone", ringtone.File);
                        }
                    }
                    finally
                    {
                        reader.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION requestPhoneWallpaper] " + ex.Message);
                    Logger.Print("[EXCEPTION requestPhoneWallpaper] " + ex.StackTrace);
                }
                finally
                {
                    mySqlReaderCon.Connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION requestPhoneWallpaper] " + ex.Message);
                Logger.Print("[EXCEPTION requestPhoneWallpaper] " + ex.StackTrace);
            }

        }


        [RemoteEvent("saveRingtones")]
        public void saveRingtones(Player c, int id)
        {
            if (c == null) return;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;

            checkUserSettingsTable(c);

            try
            {
                MySqlQuery mySqlQuery = new MySqlQuery("UPDATE phone_settings SET Wallpaper = @val WHERE Id = @userid");
                mySqlQuery.AddParameter("@userid", dbPlayer.Id);
                mySqlQuery.AddParameter("@val", id);
                MySqlHandler.ExecuteSync(mySqlQuery);
                Ringtone wallpaper = ringtones.FirstOrDefault();


                dbPlayer.TriggerEvent("RingtoneFile", wallpaper.File);
                dbPlayer.TriggerEvent("setActiveRingtone", wallpaper.Id);
                dbPlayer.SendNotification($"Wallpaper gespeichert.", 3000, "");
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION saveWallpaper] " + ex.Message);
                Logger.Print("[EXCEPTION saveWallpaper] " + ex.StackTrace);
            }
        }

        public static void checkUserSettingsTable(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                    return;

                MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1");
                mySqlQuery.Parameters = new List<MySqlParameter>()
                {
                    new MySqlParameter("@userid", dbPlayer.Id)
                };
                MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                try
                {
                    MySqlDataReader reader = mySqlReaderCon.Reader;
                    if (!reader.HasRows)
                    {
                        reader.Dispose();
                        mySqlQuery.Query = "INSERT INTO phone_settings (Id) VALUES (@userid)";
                        mySqlQuery.Parameters = new List<MySqlParameter>()
                        {
                            new MySqlParameter("@userid", dbPlayer.Id)
                        };
                        MySqlHandler.ExecuteSync(mySqlQuery);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION checkUserSettingsTable] " + ex.Message);
                    Logger.Print("[EXCEPTION checkUserSettingsTable] " + ex.StackTrace);
                }
                finally
                {
                    mySqlReaderCon.Connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION checkUserSettingsTable] " + ex.Message);
                Logger.Print("[EXCEPTION checkUserSettingsTable] " + ex.StackTrace);
            }
        }

        public static bool isFlugmodus(Player c)
        {
            if (c == null) return false;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return false;

            checkUserSettingsTable(c);
            MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1");
            mySqlQuery.Parameters = new List<MySqlParameter>()
            {
                new MySqlParameter("@userid", dbPlayer.Id)
            };
            MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
            try
            {
                mySqlQuery.Query = "SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1";
                mySqlQuery.Parameters = new List<MySqlParameter>()
                {
                    new MySqlParameter("@userid", dbPlayer.Id)
                };
                MySqlHandler.ExecuteSync(mySqlQuery);
                MySqlDataReader reader = mySqlReaderCon.Reader;
                try
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return reader.GetInt32("Flugmodus") == 1;
                        }
                    }
                }
                finally
                {
                    mySqlReaderCon.Reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION isFlugmodus] " + ex.Message);
                Logger.Print("[EXCEPTION isFlugmodus] " + ex.StackTrace);
            }
            finally
            {
                mySqlReaderCon.Connection.Dispose();
            }
            return false;
        }

        public static bool blockCalls(Player c)
        {
            if (c == null) return false;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return false;

            checkUserSettingsTable(c);
            MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1");
            mySqlQuery.Parameters = new List<MySqlParameter>()
            {
                new MySqlParameter("@userid", dbPlayer.Id)
            };
            MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
            try
            {
                mySqlQuery.Query = "SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1";
                mySqlQuery.Parameters = new List<MySqlParameter>()
                {
                    new MySqlParameter("@userid", dbPlayer.Id)
                };
                MySqlHandler.ExecuteSync(mySqlQuery);
                MySqlDataReader reader = mySqlReaderCon.Reader;
                try
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return reader.GetInt32("blockCalls") == 1;
                        }
                    }
                }
                finally
                {
                    mySqlReaderCon.Reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION blockCalls] " + ex.Message);
                Logger.Print("[EXCEPTION blockCalls] " + ex.StackTrace);
            }
            finally
            {
                mySqlReaderCon.Connection.Dispose();
            }
            return false;
        }


        [RemoteEvent("requestPhoneSettings")]
        public void requestPhoneSettings(Player c)
        {
            if (c == null) return;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;

            MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1");
            mySqlQuery.AddParameter("@userid", dbPlayer.Id);
            MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
            try
            {
                MySqlDataReader reader = mySqlReaderCon.Reader;
                try
                {
                    if (!reader.HasRows)
                    {
                        mySqlQuery.Query = "INSERT INTO phone_settings (Id) VALUES (@userid)";
                        mySqlQuery.Parameters = new List<MySqlParameter>()
                        {
                            new MySqlParameter("@userid", dbPlayer.Id)
                        };
                        MySqlHandler.ExecuteSync(mySqlQuery);
                    }
                    else
                    {
                        reader.Read();
                        string boolstring = "true";
                        if (reader.GetInt32("Flugmodus") == 0)
                            boolstring = "false";

                        string boolstring2 = "true";
                        if (reader.GetInt32("blockCalls") == 0)
                            boolstring2 = "false";


                        c.TriggerEvent("componentServerEvent", "SettingsApp", "responsePhoneSettings", boolstring, boolstring2, boolstring2);
                    }
                }
                finally
                {
                    reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION requestPhoneSettings] " + ex.Message);
                Logger.Print("[EXCEPTION requestPhoneSettings] " + ex.StackTrace);
            }
            finally
            {
                mySqlReaderCon.Connection.Dispose();
            }
        }

        [RemoteEvent("savePhoneSettings")]
        public void savePhoneSettings(Player c, bool flugmodusStatus, bool lautlosStatus, bool anrufAblehnen)
        {
            if (c == null) return;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;

            checkUserSettingsTable(c);
            try
            {
                if (Convert.ToInt32(flugmodusStatus) == 1)
                {
                    dbPlayer.SetSharedData("FUNK_CHANNEL", 0);
                    dbPlayer.SetSharedData("FUNK_TALKING", false);
                    // dbPlayer.SendNotification("flugmodus an.", 3000, "");
                    //Logger.Print("Funk Reset");
                }
                MySqlQuery mySqlQuery = new MySqlQuery("UPDATE phone_settings SET Flugmodus = @val, blockCalls = @val2 WHERE Id = @userid");
                mySqlQuery.Parameters = new List<MySqlParameter>()
                {
                    new MySqlParameter("@userid", dbPlayer.Id),
                    new MySqlParameter("@val", Convert.ToInt32(flugmodusStatus)),
                    new MySqlParameter("@val2", Convert.ToInt32(anrufAblehnen))
                };
                MySqlHandler.ExecuteSync(mySqlQuery);
                dbPlayer.SendNotification("Settings gespeichert.", 3000, "");
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION savePhoneSettings] " + ex.Message);
                Logger.Print("[EXCEPTION savePhoneSettings] " + ex.StackTrace);
            }
        }
    }
}
