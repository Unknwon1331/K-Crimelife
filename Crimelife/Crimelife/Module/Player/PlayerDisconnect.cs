using GTANetworkAPI;
using GVMP;
using System;

namespace Crimelife
{
    public class PlayerDisconnect : Script
    {
        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnect(
            Player player,
            DisconnectionType type,
            string reason)
        {
            if (player == null)
            {
                return;
            }

            string playerName =
                string.IsNullOrWhiteSpace(player.Name)
                    ? "Unbekannt"
                    : player.Name;

            try
            {
                WebhookSender.SendMessage(
                    "Spieler getrennt",
                    $"Der Spieler {playerName} hat den Server verlassen. " +
                    $"Typ: {type}, Grund: {reason}",
                    Webhooks.disconnectlogs,
                    "Disconnect"
                );
            }
            catch (Exception exception)
            {
                Logger.Print(
                    "[PlayerDisconnect Webhook] " +
                    exception.Message
                );
            }

            try
            {
                foreach (Player nearbyPlayer in
                         NAPI.Player.GetPlayersInRadiusOfPlayer(
                             50.0,
                             player))
                {
                    if (nearbyPlayer == null ||
                        nearbyPlayer.IsNull ||
                        nearbyPlayer == player)
                    {
                        continue;
                    }

                    DbPlayer nearbyDbPlayer =
                        nearbyPlayer.GetPlayer();

                    if (nearbyDbPlayer == null)
                    {
                        continue;
                    }

                    nearbyDbPlayer.SendNotification(
                        $"Der Spieler {playerName} hat den Server verlassen.",
                        4000,
                        "yellow",
                        "Disconnect"
                    );
                }
            }
            catch (Exception exception)
            {
                Logger.Print(
                    "[EXCEPTION PlayerDisconnect] " +
                    exception.Message
                );

                Logger.Print(
                    "[EXCEPTION PlayerDisconnect] " +
                    exception.StackTrace
                );
            }
        }
    }
}