using GTANetworkAPI;
using System;
using Crimelife;

namespace Crimelife
{
    class AnticheatModule : Crimelife.Module.Module<AnticheatModule>
    {
        [RemoteEvent("server:CheatDetection")]
        public static void CheatDetection(Player player, string Detection)
        {
            try
            {

                DbPlayer dbPlayer = player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null || dbPlayer.player.IsNull || dbPlayer.DeathData.IsDead) return;// || dbPlayer.Client.Dimension != 0) return;
                Adminrank adminranks = dbPlayer.Adminrank;
                if (player.HasData("PLAYER_ADUTY") && player.GetData<bool>("PLAYER_ADUTY") == true || dbPlayer.DeathData.IsDead || dbPlayer.HasData("DisableAC") && dbPlayer.GetData<bool>("DisableAC") == true) return;
                PlayerHandler.GetAdminPlayers().ForEach((DbPlayer dbPlayer2) =>
                {
                    if (dbPlayer2.HasData("disablenc")) return;

                    Adminrank adminranks = dbPlayer2.Adminrank;

                    if (adminranks.Permission >= 91);
                        /*dbPlayer2.SendNotification($"{Detection} - {player.Name} Dimension: {player.Dimension}", 3000, "red", "Anticheat");*/
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
