using GTANetworkAPI;
using GVMP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife
{
    class PlayerDisconnect : Script
    {
		[ServerEvent(Event.PlayerDisconnected)]
		public void OnPlayerDisconnect(Player c, DisconnectionType type, string reason)
		{
			WebhookSender.SendMessage("Offlineflucht", "Der Spieler " + c.Name + " hat sich ausgeloggt.", Webhooks.disconnectlogs, "Offlineflucht");		
			lock (c)
			{
				NAPI.Player.GetPlayersInRadiusOfPlayer(50.0, c).ForEach(delegate (Player player)
				{
					DbPlayer player3 = player.GetPlayer();
					if (player3 != null)
					{
						player3.SendNotification("Der Spieler " + c.Name + " hat sich ausgeloggt.", 4000, "yellow", "Offlineflucht");
					}
				});
			}
		}
	}
}
