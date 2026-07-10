using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Crimelife;
using GTANetworkAPI;


namespace Werkstatt
{
    class Werkstatt : Script
    {
        public static Dictionary<string, Vector3> werkpoints = new Dictionary<string, Vector3>();

        [ServerEvent(Event.ResourceStart)]
        public void ResourceStart()
        {
            if (werkpoints.Count < 2)
            {
                werkpoints.Add("Werkstatt", new Vector3(-198.34, -1382.63, 31));
                //werkpoints.Add("Carwash2", new Vector3(-700.1143, -933.36584, 17.9138));

                foreach (KeyValuePair<string, Vector3> werkpoints in werkpoints)
                {
                    ColShape val = NAPI.ColShape.CreateCylinderColShape(werkpoints.Value, 7.0f, 1.4f, 0);
                    val.SetData("FUNCTION_MODEL", new FunctionModel("useRepair"));
                    val.SetData("MESSAGE", new Message("Benutze E um dein Auto zu Reparieren", "Werkstatt", "grey", 3000));

                    NAPI.Blip.CreateBlip(402, werkpoints.Value, 1.0f, 0, "Werkstatt", 255, 0, true, 0, 0);
                    NAPI.Marker.CreateMarker(1, werkpoints.Value, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 4.0f, new Color(74, 74, 74), false, 0);
                }
            }
            Logger.Print("Werkstatt geladen.");
        }





        [RemoteEvent("useRepair")]
        public static void RepairPlayerCar(Player p)
        {
            try
            {
                DbPlayer dbPlayer = PlayerHandler.GetPlayer(p.Name);

                Vehicle veh = p.Vehicle;
                DbVehicle dbVehicle = veh.GetVehicle();
                dbVehicle.Vehicle.SetSharedData("engineStatus", false);
                dbPlayer.SendProgressbar(5000);
                NAPI.Task.Run((() =>
                {
                    //p.TriggerEvent("autoCar");
                    dbPlayer.StopProgressbar();
                    dbPlayer.SendNotification("Dein Fahrzeug wurde repariert", 5000, "grey", "Werkstatt");
                    if (dbPlayer.player.IsInVehicle)
                    {
                        dbPlayer.player.Vehicle.Repair();
                    }
                }), 5000);
            }
            catch (Exception ex) { Logger.Print(ex.Message); }
        }
    }
}
