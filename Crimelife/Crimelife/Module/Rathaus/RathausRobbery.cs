using System;
using System.Collections.Generic;
using GTANetworkAPI;
using Crimelife.Module;

namespace Crimelife
{
	internal class RathausRob : Module<RathausRob>
	{
		public static List<Rathaus> RathausList = new List<Rathaus>();

		protected override bool OnLoad()
		{
			RathausList.Add(new Rathaus
			{
				Id = 2,
				Location = new Vector3(-545.21, -201.92, 47.41)
			});
			foreach (Rathaus rathaus in RathausList)
			{
				ColShape val = NAPI.ColShape.CreateCylinderColShape(rathaus.Location, 1.4f, 1.4f, 0u);
				val.SetData("FUNCTION_MODEL", new FunctionModel("startHacking"));
				val.SetData("MESSAGE", new Message("Benutze E um den Computer zu hacken!", "HACKING", "red"));
				NAPI.Marker.CreateMarker(1, new Vector3(-545.21, -201.92, 47.41), new Vector3(), new Vector3(), 1.0f, new Color(0, 242, 255), false, 0);

				ColShape val1 = NAPI.ColShape.CreateCylinderColShape(new Vector3(-535.46, -195.26, 47.42), 1.4f, 1.4f, 0u);
				val1.SetData("FUNCTION_MODEL", new FunctionModel("startROBBERY"));
				val1.SetData("MESSAGE", new Message("Benutze E um das Geld zu stehlen!", "ROBBERY", "red"));
			}
			return true;
		}

		[RemoteEvent("startHacking")]
		public void startHacking(Player c)
		{
			DbPlayer dbPlayer = c.GetPlayer();
			{
				try
				{
					if (c.IsInVehicle || c.HasData("HACKING"))
					{
						dbPlayer.SendNotification("Der Computer ist defekt!", 3000, "red");
						return;
					}
					if (!dbPlayer.IsFarming || !dbPlayer.DeathData.IsDead);
					{
						NAPI.Pools.GetAllPlayers().ForEach(player => c.SetData("HACKING", true));
						dbPlayer.disableAllPlayerActions(true);
						dbPlayer.SendProgressbar(11000);
						dbPlayer.IsFarming = true;
						dbPlayer.RefreshData(dbPlayer);
						dbPlayer.PlayAnimation(33, "amb@world_human_welding@male@base", "base", 8f);
						NAPI.Task.Run(delegate
						{
							dbPlayer.TriggerEvent("client:respawning");
							dbPlayer.IsFarming = false;
							dbPlayer.RefreshData(dbPlayer);
							dbPlayer.StopProgressbar();
							dbPlayer.disableAllPlayerActions(false);
							if (dbPlayer.DeathData.IsDead) { NAPI.Pools.GetAllPlayers().ForEach(player => c.SetData("HACKING", false)); return; }
							dbPlayer.SendNotification("Du hast den Computer erfolgreich gehackt und das Passwort gesichert!", 3000, "green");
							c.SetData("PASSWORT", true);
							dbPlayer.StopAnimation();
						}, 11000);
						return;
					}
				}
				catch (Exception ex)
				{
					Logger.Print("[EXCEPTION startHacking] " + ex.Message);
					Logger.Print("[EXCEPTION startHacking] " + ex.StackTrace);
				}
			}
		}

		[RemoteEvent("startROBBERY")]
		public void startROBBERY(Player c)
		{
			DbPlayer dbPlayer = c.GetPlayer();
			{
				try
				{
					if (c.IsInVehicle || c.HasData("ALREADY"))
					{
						dbPlayer.SendNotification("Du hast das Geld bereits gestohlen.", 3000, "red");
						return;
					}
					if (c.HasData("PASSWORT"))
					{
						NAPI.Pools.GetAllPlayers().ForEach(player => c.SetData("ALREADY", true));
						dbPlayer.disableAllPlayerActions(true);
						dbPlayer.SendProgressbar(11000);
						dbPlayer.IsFarming = true;
						dbPlayer.RefreshData(dbPlayer);
						dbPlayer.PlayAnimation(33, "amb@world_human_welding@male@base", "base", 8f);
						NAPI.Task.Run(delegate
						{
							dbPlayer.TriggerEvent("client:respawning");
							dbPlayer.IsFarming = false;
							dbPlayer.RefreshData(dbPlayer);
							dbPlayer.StopProgressbar();
							dbPlayer.disableAllPlayerActions(false);
							if (dbPlayer.DeathData.IsDead) { NAPI.Pools.GetAllPlayers().ForEach(player => c.SetData("ALREADY", false)); return; }
							dbPlayer.SendNotification("Du hast nun deine Beute! Sehe zu das du hier verschwindest!", 4000, "orange");
							dbPlayer.StopAnimation();
							dbPlayer.UpdateInventoryItems("Gusenberg", 34, false);
						}, 11000);
						return;
					}
				}
				catch (Exception ex)
				{
					Logger.Print("[EXCEPTION startHacking] " + ex.Message);
					Logger.Print("[EXCEPTION startHacking] " + ex.StackTrace);
				}
			}
		}
	}
}
