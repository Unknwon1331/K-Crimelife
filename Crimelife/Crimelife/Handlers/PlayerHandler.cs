using GTANetworkAPI;
using GVMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crimelife
{
    public static class PlayerHandler
    {
        public static List<DbPlayer> GetPlayers()
        {
            List<DbPlayer> dbPlayers = new List<DbPlayer>();
            List<Player> clients = NAPI.Pools.GetAllPlayers();

            foreach (Player client in clients)
            {
                DbPlayer dbPlayer = client.GetPlayer();

                if (!IsUsablePlayer(dbPlayer))
                {
                    continue;
                }

                dbPlayers.Add(dbPlayer);
            }

            return dbPlayers;
        }

        public static List<DbPlayer> GetAdminPlayers()
        {
            return GetPlayers()
                .Where(dbPlayer =>
                    dbPlayer.Adminrank != null &&
                    dbPlayer.Adminrank.Permission > 0)
                .ToList();
        }

        public static List<DbPlayer> GetFactionPlayers(
            this Faction faction)
        {
            if (faction == null)
            {
                return new List<DbPlayer>();
            }

            return GetPlayers()
                .Where(dbPlayer =>
                    dbPlayer.Faction != null &&
                    dbPlayer.Faction.Id == faction.Id)
                .ToList();
        }

        public static List<DbPlayer> GetBusinessPlayers(
            this Business business)
        {
            if (business == null)
            {
                return new List<DbPlayer>();
            }

            return GetPlayers()
                .Where(dbPlayer =>
                    dbPlayer.Business != null &&
                    dbPlayer.Business.Id == business.Id)
                .ToList();
        }

        public static DbPlayer GetPlayer(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return GetPlayers().FirstOrDefault(
                dbPlayer =>
                    string.Equals(
                        dbPlayer.Name,
                        name,
                        StringComparison.OrdinalIgnoreCase
                    )
            );
        }

        public static DbPlayer GetPlayerbyfaction(int factionId)
        {
            return GetPlayers().FirstOrDefault(
                dbPlayer =>
                    dbPlayer.Faction != null &&
                    dbPlayer.Faction.Id == factionId
            );
        }

        public static DbPlayer GetPlayer(int id)
        {
            return GetPlayers().FirstOrDefault(
                dbPlayer => dbPlayer.Id == id
            );
        }

        private static bool IsUsablePlayer(DbPlayer dbPlayer)
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

            return dbPlayer.IsValid(true);
        }
    }
}