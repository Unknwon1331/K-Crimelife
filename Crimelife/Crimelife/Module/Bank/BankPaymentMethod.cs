/*using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using GTANetworkAPI;
using Crimelife;

namespace Crimelie
{
        public class BankPaymentMethod : Crimelife.Module.Module<BankPaymentMethod>
    {
        private class ShowEvent //: Event
        {

            [JsonProperty(PropertyName = "price")]
            private int Price { get; }


            public ShowEvent(DbPlayer dbPlayer, int price)
            {
                Price = price;

            }
        }

        public BankPaymentMethod()
        {
        }

        public override Func<DbPlayer, int, bool> Show()
        {
            return (player, price) => OnShow(new ShowEvent(player, price));
        }

        private bool OnShow(ShowEvent showEvent)
        {
            throw new NotImplementedException();
        }

        [RemoteEvent]
        public void selectPaymentMethod(Player Player, string method)
        {
            NAPI.Task.Run(() =>
            {

                var iPlayer = Player.GetPlayer();
                if (iPlayer == null || !iPlayer.IsValid()) return;

                iPlayer.SetData("selected", method);
                iPlayer.TriggerEvent("Moneywindownocursor");
            });
        
        }
    }
}*/