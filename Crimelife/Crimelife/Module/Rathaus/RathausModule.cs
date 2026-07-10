using System;
using System.Collections.Generic;
using System.Data.Common;
using GTANetworkAPI;
using Crimelife.Module;

namespace Crimelife
{
	internal class RathausModule : Module<RathausModule>
	{
		public static List<Rathaus> RathausList = new List<Rathaus>();

		protected override bool OnLoad()
		{
			RathausList.Add(new Rathaus
			{
				Id = 1,
				Location = new Vector3(-551.15, -192.27, 38.22)
			});
			foreach (Rathaus rathaus in RathausList)
			{
				ColShape val = NAPI.ColShape.CreateCylinderColShape(rathaus.Location, 1.4f, 1.4f, 0u);
				val.SetData("FUNCTION_MODEL", new FunctionModel("openRathaus"));
				val.SetData("MESSAGE", new Message("Benutze E um mit dem Rathaus zu interagieren.", "RATHAUS", "green"));
				NAPI.Marker.CreateMarker(1, rathaus.Location.Subtract(new Vector3(0f, 0f, 1f)), new Vector3(), new Vector3(), 1f, new Color(0, 242, 255), false, 0u);
				NAPI.Blip.CreateBlip(419, rathaus.Location, 1f, (byte)0, "Rathaus", byte.MaxValue, 0f, true, (short)0, 0u);
			}
			return true;
		}

		[RemoteEvent("openRathaus")]
		public void openRathaus(Player c)
		{
			try
			{
				if (!(c == null))
				{
					DbPlayer player = c.GetPlayer();
					if (player != null && player.IsValid(ignorelogin: true) && !(player.player == null))
					{
						NativeMenu nativeMenu = new NativeMenu("Rathaus", "", new List<NativeItem>
					{
						new NativeItem("Namensänderung - 10.000.000$", "changename")
					});
						player.ShowNativeMenu(nativeMenu);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Print("[EXCEPTION openRathaus] " + ex.Message);
				Logger.Print("[EXCEPTION openRathaus] " + ex.StackTrace);
			}
		}

		[RemoteEvent("nM-Rathaus")]
		public void Rathaus(Player c)
		{
			try
			{
				if (!(c == null))
				{
					DbPlayer player = c.GetPlayer();
					if (player != null && player.IsValid(ignorelogin: true) && !(player.player == null))
					{
						player.CloseNativeMenu();
						player.OpenTextInputBox(new TextInputBoxObject
						{
							Title = "Namensänderungen",
							Message = "Gebe bitte deinen neuen Namen ein, im Anschluss wird dir der Betrag in Höhe von 10.000.000$ vom Staat entfernt. Beispiel: Max_Mustermann",
							Callback = "changeName",
							CloseCallback = ""
						});
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Print("[EXCEPTION nM-Rathaus] " + ex.Message);
				Logger.Print("[EXCEPTION nM-Rathaus] " + ex.StackTrace);
			}
		}

		[RemoteEvent("changeName")]
		public void changeName(Player c, string username)
		{
			//SyncThread.Process(username);

			try
			{
				if (c == null)
				{
					return;
				}
				DbPlayer player = c.GetPlayer();
				if (player == null || !player.IsValid(ignorelogin: true) || player.player == null)
				{
					return;
				}
				if (player.Money >= 10000000)
				{
					bool flag = false;
					foreach (char c2 in username)
					{
						if (c2 == '_')
						{
							flag = true;
						}
					}
					if (username.Length > 16)
					{
						flag = false;
					}
					if (username != username.RemoveSpecialCharacters())
					{
						flag = false;
					}
					if (flag)
					{
						MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Username = @username");
						mySqlQuery.AddParameter("@username", username);
						MySqlResult query = MySqlHandler.GetQuery(mySqlQuery);
						if (((DbDataReader)query.Reader).HasRows)
						{
							while (((DbDataReader)query.Reader).Read())
							{
								int @int = query.Reader.GetInt32("Adminrank");
								if (c.IsInVehicle)
								{
									return;
								}
								if (@int > 0)
								{
									player.BanPlayer();
									return;
								}
							}
						}
						flag = !((DbDataReader)query.Reader).HasRows;
						query.Reader.Dispose();
						query.Connection.Dispose();
					}
					if (!flag)
					{
						c.SendNotification("Der kann nur aus einem Vor- und Nachnamen bestehen, nur 16 Zeichen lang sein und ihn darf es noch nicht geben.", true);
						player.OpenTextInputBox(new TextInputBoxObject
						{
							Title = "Name ändern",
							Message = "Gebe bitte deinen neuen Namen ein, im Anschluss wird dir der Betrag in Höhe von 10.000.000$ vom Staat entfernt. Beispiel: Max_Mustermann",
							Callback = "changeName",
							CloseCallback = ""
						});
						return;
					}
					WebhookSender.SendMessage("Spieler benennt sich um", "Der Spieler " + player.Name + " ID " + player.Id + " nennt sich zu " + username + " um.", Webhooks.rathauslogs, "Rathaus");
					player.SendNotification("Name geändert!", 3000, "red", "RATHAUS");
					player.removeMoney(1000000);
					player.Name = username;
					player.RefreshData(player);
					player.SetAttribute("Username", username);
					player.player.Name = (username);
				}
				else
				{
					player.SendNotification("Du hast nicht genug Geld!", 3000, "red", "RATHAUS");
				}
			}
			catch (Exception ex)
			{
				Logger.Print("[EXCEPTION changeName] " + ex.Message);
				Logger.Print("[EXCEPTION changeName] " + ex.StackTrace);
			}
		}
	}
}
