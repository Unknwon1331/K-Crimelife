using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using GVMP;

namespace Crimelife
{
    public static class NativeMenuDb
    {
        public static void ShowNativeMenu(this DbPlayer dbPlayer, NativeMenu nativeMenu)
        {
            Player client = dbPlayer.player;
            client.SetData("PLAYER_CURRENT_NATIVEMENU", nativeMenu);
            client.TriggerEvent("componentServerEvent", "NativeMenu", "showNativeMenu", NAPI.Util.ToJson(nativeMenu), 0);
        }

        public static void CloseNativeMenu(this DbPlayer dbPlayer)
        {
            dbPlayer.player.TriggerEvent("componentServerEvent", "NativeMenu", "hide");
        }
    }
}
