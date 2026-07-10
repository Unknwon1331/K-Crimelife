using GTANetworkAPI;
using Microsoft.VisualBasic;
using System;

namespace Crimelife
{
    public class AntiAimbotModule : Crimelife.Module.Module<AntiAimbotModule>
    {
        [RemoteEvent]
        public void AntiAimbot(Player player, ulong bone, float damage, ulong weapon)
        {
            DbPlayer dbPlayer = player.GetPlayer();
            // Console.WriteLine($"Bone: {bone} Damage: {damage} Weapon: {weapon} Count: {dbPlayer.AimbotCount} LastBone: {dbPlayer.AimbotLastBone}");

            if (bone == dbPlayer.AimbotLastBone)
            {
                if (dbPlayer.AimbotCount >= 3)
                {
                    foreach (DbPlayer dbplayer in PlayerHandler.GetAdminPlayers())
                    {
                        /*dbplayer.SendNotification($"{player.GetPlayer().Name} used wahrscheinlich 0:0 (Aimbot)");*/
                    }
                }
                else
                {
                    dbPlayer.AimbotCount = dbPlayer.AimbotCount += 1;
                }
            }
            else
            {
                dbPlayer.AimbotLastBone = bone;
                dbPlayer.AimbotCount = 1;
            }
        }
    }
}