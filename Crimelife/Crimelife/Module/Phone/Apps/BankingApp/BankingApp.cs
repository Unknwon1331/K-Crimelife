using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife
{
    class BankingAppOverview : Crimelife.Module.Module<BankingAppOverview>
    {
        [RemoteEvent]
        public void responseBankingAppOverview(Player player)
        {
            try
            {
                var dbPlayer = player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid()) return;
                player.TriggerEvent("requestBankAppOverview", dbPlayer.money[0], new List<BankHistory>());
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }

    public class BankAppTransfer //: SimpleApp
    {
        int bankingmaxcap = 1000000;
        int bankingmincap = 500;
        int tax = 1; //1% aktuell deaktiviert
        public BankAppTransfer() //: base("BankAppTransfer")
        {
        }

        [RemoteEvent]
        public void requestBankingCap(Player player)
        {   // Achtung - BankingCap wird auch im Player abgefragt
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            player.TriggerEvent("responseBankingCap", bankingmaxcap, bankingmincap);
        }

        [RemoteEvent]
        public void bankingAppTransfer(Player player, String toPlayer, int amount)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (amount > bankingmaxcap) { return; }
            if (amount < bankingmincap) { return; }

            dbPlayer.addMoney(amount);
            //    var bankwindow = new BankWindow();
            //    bankwindow.bankTransfer(player, amount, toPlayer);
        }

    }

}