using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using GVMP;

namespace Crimelife
{
    class PhoneModule : Crimelife.Module.Module<PhoneModule>
    {
	[RemoteEvent("Keks")]
	public static void PhoneOpen(Player c, bool state)
	{
		try
		{
			if ((Entity)(object)c == (Entity)null)
			{
				return;
			}
			DbPlayer player = c.GetPlayer();
			if (player == null || !player.IsValid(ignorelogin: true) || (Entity)(object)player.player == (Entity)null || !player.CanInteractAntiDeath() || player.DeathData.IsDead)
			{
				return;
			}

			if (state)
			{
			    EntityType entity = EntityType.Player;
                c.TriggerEvent("setAttachments", entity, "HoldHandyFix");
				c.TriggerEvent("createphoneobject");

				if (!c.IsInVehicle)
				{
					NAPI.Player.PlayPlayerAnimation(c, 49, "amb@world_human_stand_mobile@male@text@base", "base", 8f);
                    c.TriggerEvent("setAttachments", entity, "HoldHandyFix");
				    c.TriggerEvent("createphoneobject");
				  //  NAPI.Object.CreateObject(1407197773, player.Position, new Vector3(), 255, 0);

                    }
				c.TriggerEvent("hatNudeln", new object[1] { state });
				requestApps(player.player);
				}
			else
			{
				c.TriggerEvent("hatNudeln", new object[1] { false });
                c.TriggerEvent("destroyphoneobject");
				if (!c.IsInVehicle)
				{
					c.StopAnimation();
                    c.TriggerEvent("destroyphoneobject");
                }
			}
		}
		catch (Exception ex)
		{
			Logger.Print("[EXCEPTION PhoneOpen] " + ex.Message);
			Logger.Print("[EXCEPTION PhoneOpen] " + ex.StackTrace);
		}
	}

		[RemoteEvent("requestApps")]
		public static void requestApps(Player c)
		{
			try
			{
				if ((Entity)(object)c == (Entity)null)
				{
					return;
				}
				DbPlayer player = c.GetPlayer();
				if (player != null && player.IsValid(ignorelogin: true) && !((Entity)(object)player.player == (Entity)null))
				{
					string text = "{\"id\":\"TeamApp\",\"name\":\"Team\",\"icon\":\"TeamApp.png\"}, ";
					string text2 = "{\"id\":\"BusinessApp\",\"name\":\"Business\",\"icon\":\"BusinessApp.png\"}, ";
					if (player.Faction.Id == 0)
					{
						text = "";
					}
					if (player.Business.Id == 0)
					{
						text2 = "";
					}
					if (SettingsApp.isFlugmodus(player.player))
                    {
						c.TriggerEvent("componentServerEvent", new object[3]
					{
					"HomeApp",
					"responseApps",
					"[" + text + text2 +"{\"id\":\"ContactsApp\",\"name\":\"Kontakte\",\"icon\": \"ContactsApp.png\"}, {\"id\":\"GpsApp\",\"name\":\"GPS\",\"icon\": \"GpsApp.png\"}, {\"id\":\"CalculatorApp\",\"name\":\"Rechner\",\"icon\": \"CalculatorApp.png\"}, {\"id\":\"ProfileApp\",\"name\":\"Profil\",\"icon\": \"ProfilApp.png\"}, {\"id\":\"SettingsApp\",\"name\":\"Settings\",\"icon\": \"SettingsApp.png\"}, {\"id\":\"NewsApp\",\"name\":\"NewsApp\",\"icon\": \"NewsApp.png\"}, {\"id\":\"BankingApp\",\"name\":\"Banking\",\"icon\": \"BankingApp.png\"}]"
                    });
					}
 else
                    {
						c.TriggerEvent("componentServerEvent", new object[3]
					{
					"HomeApp",
					"responseApps",
					"[" + text + text2 +"{\"id\":\"FunkApp\",\"name\":\"Funkgerät\",\"icon\": \"FunkApp.png\"}, {\"id\":\"ContactsApp\",\"name\":\"Kontakte\",\"icon\": \"ContactsApp.png\"}, {\"id\":\"GpsApp\",\"name\":\"GPS\",\"icon\": \"GpsApp.png\"}, {\"id\":\"TelefonApp\",\"name\":\"Telefon\",\"icon\": \"TelefonApp.png\"}, {\"id\":\"MessengerApp\",\"name\":\"SMS\",\"icon\": \"MessengerApp.png\"},  {\"id\":\"CalculatorApp\",\"name\":\"Rechner\",\"icon\": \"CalculatorApp.png\"}, {\"id\":\"ProfileApp\",\"name\":\"Profil\",\"icon\": \"ProfilApp.png\"}, {\"id\":\"SettingsApp\",\"name\":\"Settings\",\"icon\": \"SettingsApp.png\"}, {\"id\":\"LifeInvaderApp\",\"name\":\"Lifeinvader\",\"icon\": \"LifeinvaderApp.png\"}, {\"id\":\"NewsApp\",\"name\":\"NewsApp\",\"icon\": \"NewsApp.png\"}, {\"id\":\"BankingApp\",\"name\":\"Banking\",\"icon\": \"BankingApp.png\"}]"
                    });
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Print("[EXCEPTION requestApps] " + ex.Message);
				Logger.Print("[EXCEPTION requestApps] " + ex.StackTrace);
			}
		}

        
    }
}
