using GTANetworkAPI;
using GVMP;
using System;

namespace Crimelife
{
    public static class PlayerDb
    {
        public static DbPlayer GetPlayer(this Player player)
        {
            if (player == null)
            {
                return null;
            }

            if (player.IsNull)
            {
                return null;
            }

            if (!player.HasData("player"))
            {
                return null;
            }

            return player.GetData<DbPlayer>("player");
        }

        public static bool CanInteractAntiDeath(
            this DbPlayer dbPlayer)
        {
            if (!IsValidReference(dbPlayer))
            {
                return false;
            }

            return dbPlayer.LastDeath.AddSeconds(3) <= DateTime.Now;
        }

        public static bool CanInteractAntiFlood(
            this DbPlayer dbPlayer,
            int seconds = 3)
        {
            if (!IsValidReference(dbPlayer))
            {
                return false;
            }

            seconds = Math.Max(1, Math.Min(seconds, 60));

            if (dbPlayer.LastInteracted.AddSeconds(seconds) >
                DateTime.Now)
            {
                return false;
            }

            dbPlayer.LastInteracted = DateTime.Now;
            return true;
        }

        public static bool CanPressE(this DbPlayer dbPlayer)
        {
            if (!IsValidReference(dbPlayer))
            {
                return false;
            }

            if (dbPlayer.LastEInteract.AddSeconds(3) >
                DateTime.Now)
            {
                return false;
            }

            dbPlayer.LastEInteract = DateTime.Now;
            return true;
        }

        public static bool IsValid(
            this DbPlayer dbPlayer,
            bool ignoreLogin = false)
        {
            if (!IsValidReference(dbPlayer))
            {
                return false;
            }

            if (!NAPI.Pools.GetAllPlayers().Contains(dbPlayer.player))
            {
                return false;
            }

            return ignoreLogin ||
                   dbPlayer.AccountStatus == AccountStatus.LoggedIn;
        }

        private static bool IsValidReference(DbPlayer dbPlayer)
        {
            if (dbPlayer == null)
            {
                return false;
            }

            if (dbPlayer.player == null)
            {
                return false;
            }

            if (dbPlayer.player.IsNull)
            {
                return false;
            }

            return true;
        }
    }
}