using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace Crimelife
{
	class MarkedPlaceApp : Script
	{

		public static List<OfferModel> myOffers = new List<OfferModel>();
		public static List<OfferModel> Offers = new List<OfferModel>();

		[RemoteEvent("deleteOffer")]
		public void deleteOffer(Player p)
		{
			DbPlayer dbPlayer = p.GetPlayer();
			Offers.Remove(Offers.Find((OfferModel offer) => offer.phone == p));
			dbPlayer.SendNotification("Du hast dein Angebot gelöscht!", 3000, "red", "GEBAY");
			requestMarketPlaceOffers(p, 0);
			requestMyOffers(p);
		}

		[RemoteEvent("requestMarketplaceCategories")]
		public void requestMarketplaceCategories(Player p)
		{
			try
			{
				if (p.Dimension == 8888)
					return;
				p.TriggerEvent("componentServerEvent", new object[3]
				{
					"MarketplaceApp",
					"responseMarketPlaceCategories",
					"[{\"id\":\"1\",\"name\":\"Angebote\",\"icon_path\":\"house.png\"}]"
				});


			}
			catch (Exception ex)
			{
				Logger.Print(ex.ToString());
			}
		}

		[RemoteEvent("requestMyOffers")]
		public void requestMyOffers(Player c)
		{
			c.TriggerEvent("componentServerEvent", new object[3]
			{
				"MarketplaceApp",
				"responseMyOffers",
				JsonConvert.SerializeObject(myOffers)
			});
		}

		[RemoteEvent("requestMarketPlaceOffers")]
		public void requestMarketPlaceOffers(Player p, int categoryId)
		{
			try
			{
				if (categoryId == 1)
				{
					p.TriggerEvent("componentServerEvent", new object[3]
					{
						"MarketplaceApp",
						"responseMarketPlaceOffers",
						JsonConvert.SerializeObject(Offers)
					});
				}
			}
			catch (Exception e)
			{
				Logger.Print(e.ToString());
			}
		}

		[RemoteEvent("addOffer")]
		public void addOffer(Player c, int id, string name, int price, string desc, bool search)
		{
			DbPlayer dbPlayer1 = c.GetPlayer();
			if (id == 1)
			{
				Offers.Add(new OfferModel(1, name, search, c, price.ToString(), desc));
			}


			foreach (Player p in NAPI.Pools.GetAllPlayers())
			{
				DbPlayer dbPlayer = p.GetPlayer();
				dbPlayer.SendNotification("Es ist eine neue Gebay-Anzeige aufgetaucht!", 3000, "green", "GEBAY");
			}
			requestMarketPlaceOffers(c, id);
			dbPlayer1.SendNotification("Erfolgreich erstellt!", 3000, "red", "GEBAY");
		}
	}
}
