using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Reflection;


namespace Crimelife
{
    public class ACPModule : Crimelife.Module.Module<ACPModule>
    {
        protected override bool OnLoad()
        {

            return true;
        }


        public static void Spectate(Player player, string targetname)
        {
            DbPlayer dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;
            try
            {

                Player target = NAPI.Player.GetPlayerFromName(targetname);
                DbPlayer dbTarget = target.GetPlayer();
                if (dbTarget == null || !dbTarget.IsValid(true) || dbTarget.player == null)
                    return;

                if (dbPlayer.Adminrank.Permission >= 91)
                {
                    if (NAPI.Player.GetPlayerFromName(targetname) == null)
                    {
                        dbPlayer.SendNotification("Spieler nicht Gefunden!", 5000, "red", "Information");
                        return;
                    }
                    else
                    {
                        dbPlayer.SetData("spectatedPlayer", targetname);
                        string ped = "a_c_chop";
                        dbPlayer.player.SetSkin(NAPI.Util.GetHashKey(ped));
                        dbPlayer.player.TriggerEvent("spectatePlayer", NAPI.Player.GetPlayerFromName(targetname));
                        dbPlayer.SendNotification($"Du Spectatest nun " + targetname, 5000, "red", "Information");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        public static void unSpectate(Player player)
        {
            DbPlayer dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;
            try
            {

                if (dbPlayer.Adminrank.Permission >= 91)
                {
                    dbPlayer.ResetData("spectatedPlayer");

                    dbPlayer.player.TriggerEvent("stopSpectating");
                    dbPlayer.SendNotification($"Du Spectatest nun Nicht mehr ", 5000, "red", "Information");
                    dbPlayer.ApplyCharacter();
                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Username = @user LIMIT 1");
                    mySqlQuery.AddParameter("@user", player.Name);

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

                                    player.SetAccessories(0, playerClothes.Hut.drawable, playerClothes.Hut.texture);
                                    player.SetAccessories(1, playerClothes.Brille.drawable, playerClothes.Brille.texture);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}