using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Crimelife
{
    class MueleimerModule : Crimelife.Module.Module<MueleimerModule>
    {
        public static List<string> equipItems = new List<string>();
        public static List<int> alreadyEquipped = new List<int>();

        class Carwash : Script
        {
            public static Dictionary<string, Vector3> mullpoint = new Dictionary<string, Vector3>();

            [ServerEvent(Event.ResourceStart)]
            public void ResourceStart()
            {
                equipItems.Add("Carwash");
                equipItems.Add("Carwash2");
                equipItems.Add("Wuerfel");
                equipItems.Add("Geschenk");
                equipItems.Add("Batterie");
                equipItems.Add("Compactrifle");
                equipItems.Add("Pfandflasche");
                equipItems.Add("Pfandflasche");
                equipItems.Add("Pfandflasche");
                equipItems.Add("Pfandflasche");
                equipItems.Add("Pfandflasche");
                equipItems.Add("Pfandflasche");
                equipItems.Add("Gips");
                equipItems.Add("Pfandflasche");

                if (mullpoint.Count < 2)
                {
                    mullpoint.Add("Muell", new Vector3(115.98, -1049.74, 28.2));
                    mullpoint.Add("Muell2", new Vector3(172.44, -1073.81, 28.19));
                    mullpoint.Add("Muell3", new Vector3(300.14, -906.62, 28.29));
                    mullpoint.Add("Muell4", new Vector3(336.17, -1081.95, 28.45));
                    mullpoint.Add("Muell5", new Vector3(341.72, -1106.51, 28.41));
                    mullpoint.Add("Muell6", new Vector3(50.51, -1044.9, 28.59));
                    mullpoint.Add("Muell7", new Vector3(-8.86, -1036.62, 28.02));
                    mullpoint.Add("Muell8", new Vector3(-175, -1285, 30.2));
                    mullpoint.Add("Muell9", new Vector3(-149, -1294, 30.24));


                    foreach (KeyValuePair<string, Vector3> carwashpoint in mullpoint)
                    {
                        ColShape val = NAPI.ColShape.CreateCylinderColShape(carwashpoint.Value, 7.0f, 1.4f, 0);
                        val.SetData("FUNCTION_MODEL", new FunctionModel("useElefant"));
                        val.SetData("MESSAGE", new Message("Benutze E um denn Mülleimer zu durchsuchn.", "Müll", "darkgrey", 3000));

                        //                        NAPI.Blip.CreateBlip(100, carwashpoint.Value, 1.0f, 0, "Carwash", 255, 0, true, 0, 0);
                        //                        NAPI.Marker.CreateMarker(1, carwashpoint.Value, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 4.0f, new Color(67, 142, 217), false, 0);
                    }
                }
                Logger.Print("MülleimerModule geladen.");
            }


            [RemoteEvent("useElefant")]
            public static void useElefant(Player c)
            {
                try
                {
                    if (c == null) return;
                    DbPlayer dbPlayer = c.GetPlayer();
                    if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                        return;

                    try
                    {
                        if (alreadyEquipped.Contains(dbPlayer.Id))
                        {
                            dbPlayer.SendNotification("Du hast diesen Mülleimer bereits durchsucht!", 3000, "red", "");
                            return;
                        }

                        alreadyEquipped.Add(dbPlayer.Id);
                        dbPlayer.SendProgressbar(5000);
                        dbPlayer.disableAllPlayerActions(true);
                        dbPlayer.PlayAnimation(33, "amb@prop_human_bum_bin@base", "base", 8f);
                        NAPI.Task.Run(() =>
                        {
                            var r = new Random();
                            string item = equipItems[r.Next(0, equipItems.Count)];
                            dbPlayer.UpdateInventoryItems(item, 1, false);
                            dbPlayer.StopProgressbar();
                            dbPlayer.SendNotification("Du hast (+ 1x " + item + ") gefunden", 3000, "black", "");
                            dbPlayer.StopAnimation();
                            dbPlayer.disableAllPlayerActions(false);
                        }, 5000);
                    }
                    catch (Exception ex)
                    {
                        Logger.Print("[EXCEPTION useEquippoint] " + ex.Message);
                        Logger.Print("[EXCEPTION useEquippoint] " + ex.StackTrace);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION useEquippoint] " + ex.Message);
                    Logger.Print("[EXCEPTION useEquippoint] " + ex.StackTrace);
                }
            }
        }
    }
}