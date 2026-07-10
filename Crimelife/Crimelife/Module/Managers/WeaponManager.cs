using GTANetworkAPI;
using GVMP.Handlers;
using Crimelife.Types;
using System.Collections.Generic;
using System.Linq;
using GVMP;
using System;

namespace Crimelife
{
    class WeaponManager : Script
    {
        public static void removeWeapon(Player client, WeaponHash weaponHash)
        {
            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;

            List<NXWeapon> playerWeapons = NAPI.Util.FromJson<List<NXWeapon>>(dbPlayer.GetAttributeString("Loadout"));

            playerWeapons.RemoveAll(w => w.Weapon == weaponHash);

            dbPlayer.Loadout = playerWeapons;
            dbPlayer.RefreshData(dbPlayer);

            dbPlayer.player.RemoveAllOwnWeaponComponent(weaponHash);
            NAPI.Player.RemovePlayerWeapon(client, weaponHash);
            dbPlayer.TriggerEvent("client:weaponSwap");
            dbPlayer.SetAttribute("Loadout", NAPI.Util.ToJson(playerWeapons));
        }

        public static void loadWeapons(Player p)
        {
            DbPlayer dbPlayer = p.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;

            Constants.SetPlayerACFlag(p);
            List<NXWeapon> playerWeapons = NAPI.Util.FromJson<List<NXWeapon>>(dbPlayer.GetAttributeString("Loadout"));

            dbPlayer.Loadout = playerWeapons;
            dbPlayer.RefreshData(dbPlayer);

            p.RemoveAllWeapons();

            foreach (NXWeapon weapon in playerWeapons)
            {
                p.GiveWeapon(weapon.Weapon, 9999);
                foreach (WeaponComponent weaponComponent in weapon.Components)
                {
                    dbPlayer.player.SetOwnWeaponComponent(weapon.Weapon, weaponComponent);
                }
            }
            NAPI.Player.SetPlayerCurrentWeapon(p, WeaponHash.Unarmed);
        }

        public static List<WeaponHash> LoadWeaponModels(Player c)
        {
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return new List<WeaponHash>();

            List<NXWeapon> playerWeapons = dbPlayer.Loadout;

            return playerWeapons.Select(x => x.Weapon).ToList();
        }

        public static void addWeapon(Player client, WeaponHash weaponHash)
        {
            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;

            dbPlayer.player.RemoveAllOwnWeaponComponent(weaponHash);
            client.GiveWeapon(weaponHash, 9999);

            List<NXWeapon> playerWeapons = NAPI.Util.FromJson<List<NXWeapon>>(dbPlayer.GetAttributeString("Loadout"));
            playerWeapons.Add(new NXWeapon
            {
                Weapon = weaponHash,
                Components = new List<WeaponComponent>()
            });

            dbPlayer.Loadout = playerWeapons;
            dbPlayer.RefreshData(dbPlayer);

            dbPlayer.SetAttribute("Loadout", NAPI.Util.ToJson(playerWeapons));
        }

        public static void addWeaponComponent(Player client, WeaponHash weaponHash, WeaponComponent weaponComponent)
        {
            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;

            dbPlayer.player.SetOwnWeaponComponent(weaponHash, weaponComponent);

            List<NXWeapon> playerWeapons = NAPI.Util.FromJson<List<NXWeapon>>(dbPlayer.GetAttributeString("Loadout"));

            var weapon = playerWeapons.FirstOrDefault(w => w.Weapon == weaponHash);
            if (weapon == null) return;

            if (!weapon.Components.Contains(weaponComponent))
                weapon.Components.Add(weaponComponent);

            dbPlayer.Loadout = playerWeapons;
            dbPlayer.RefreshData(dbPlayer);

            dbPlayer.SetAttribute("Loadout", NAPI.Util.ToJson(playerWeapons));
        }

        public static void removeWeaponComponent(Player client, WeaponHash weaponHash, WeaponComponent weaponComponent)
        {
            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;

            dbPlayer.player.RemoveOwnWeaponComponent(weaponHash, weaponComponent);

            List<NXWeapon> playerWeapons = NAPI.Util.FromJson<List<NXWeapon>>(dbPlayer.GetAttributeString("Loadout"));

            var weapon = playerWeapons.FirstOrDefault(w => w.Weapon == weaponHash);
            if (weapon == null) return;

            if (weapon.Components.Contains(weaponComponent))
                weapon.Components.Remove(weaponComponent);

            dbPlayer.Loadout = playerWeapons;
            dbPlayer.RefreshData(dbPlayer);

            dbPlayer.SetAttribute("Loadout", NAPI.Util.ToJson(playerWeapons));
        }

        public static void removeAllWeapons(Player client)
        {
            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;

            dbPlayer.player.ResetAllOwnWeaponComponents();
            client.RemoveAllWeapons();

            dbPlayer.Loadout.Clear();
            dbPlayer.RefreshData(dbPlayer);

            dbPlayer.SetAttribute("Loadout", NAPI.Util.ToJson(new List<WeaponHash>()));
        }
    }
}