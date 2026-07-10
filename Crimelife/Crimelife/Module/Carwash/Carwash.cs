using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Crimelife;
using GTANetworkAPI;
using Org.BouncyCastle.Crypto.Engines;

namespace Carwash
{
    class Carwash : Script
    {
        public static Dictionary<string, Vector3> washpoints = new Dictionary<string, Vector3>();

        [ServerEvent(Event.ResourceStart)]
        public void ResourceStart()
        {
            if (washpoints.Count < 2)
            {
                washpoints.Add("Carwash", new Vector3(24.038015, -1392.0083, 28.2323));
                washpoints.Add("Carwash2", new Vector3(-700.1143, -933.36584, 17.9138));
                /*
                  washpoints.Add("Carwash2", new Vector3(-700.1143, -933.36584, 17.9138));
                  washpoints.Add("Carwash2", new Vector3(-700.1143, -933.36584, 17.9138));
                  washpoints.Add("Carwash2", new Vector3(-700.1143, -933.36584, 17.9138));
                  washpoints.Add("Carwash2", new Vector3(-700.1143, -933.36584, 17.9138));
                  washpoints.Add("Carwash2", new Vector3(-700.1143, -933.36584, 17.9138));
                  washpoints.Add("Carwash2", new Vector3(-700.1143, -933.36584, 17.9138));
                  washpoints.Add("Carwash2", new Vector3(-700.1143, -933.36584, 17.9138));
                  washpoints.Add("Carwash2", new Vector3(-700.1143, -933.36584, 17.9138));
                 */

                foreach (KeyValuePair<string, Vector3> carwashpoint in washpoints)
                {
                    ColShape val = NAPI.ColShape.CreateCylinderColShape(carwashpoint.Value, 7.0f, 1.4f, 0);
                    val.SetData("FUNCTION_MODEL", new FunctionModel("useCarwash"));
                    val.SetData("MESSAGE", new Message("Benutze E um dein Auto zu Waschen.", "Carwash", "orange", 3000));

                    NAPI.Blip.CreateBlip(100, carwashpoint.Value, 1.0f, 0, "Carwash", 255, 0, true, 0, 0);
                    NAPI.Marker.CreateMarker(1, carwashpoint.Value, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 4.0f, new Color(67, 142, 217), false, 0);
                }
            }
            Logger.Print("Carwash geladen.");

        }





        [RemoteEvent("useCarwash")]
        public static void CleanPlayerCar(Player p)
        {
            try
            {
                DbPlayer dbPlayer = PlayerHandler.GetPlayer(p.Name);

                Vehicle veh = p.Vehicle;
                DbVehicle dbVehicle = veh.GetVehicle();

                if (dbVehicle.Vehicle.EngineStatus = true)
                {
                    dbPlayer.SendNotification("Herzlich Willkommen in der Waschstraße!", 5000, "yellow", "Carwash");
                }
                dbVehicle.Vehicle.SetSharedData("engineStatus", false);
                dbVehicle.Vehicle.EngineStatus = false;
                dbPlayer.SendNotification("Motor ausgeschaltet!", 5000, "red");
                dbPlayer.SendProgressbar(5000);
                NAPI.Task.Run((() =>
                {
                    p.TriggerEvent("autoCar");
                    dbPlayer.StopProgressbar();

                }), 5000);

                NAPI.Task.Run((() =>
                {
                    p.TriggerEvent("closeBrowser");
                    dbPlayer.StopProgressbar();

                }), 8000);

                p.TriggerEvent("closeBrowser");
                dbPlayer.SendNotification("Dein Fahrzeug ist nun Sauber!", 5000, "yellow", "Carwash");

            }
            catch (Exception ex) { Logger.Print(ex.Message); }
        }
    }
}
