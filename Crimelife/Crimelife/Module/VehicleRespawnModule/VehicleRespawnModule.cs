/*using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife.Module.VehicleRespawnModule
{
    class VehicleRespawnModule : Crimelife.Module.Module<VehicleRespawnModule>
    {
        protected override bool OnLoad()
        {




            MySqlQuery query = new MySqlQuery("SELECT * FROM vehicles WHERE Parked = 0 AND Impound = 0");
            MySqlResult query2 = MySqlHandler.GetQuery(query);
            try
            {
                MySqlDataReader reader = query2.Reader;
                try
                {
                    if ((reader).HasRows)
                    {
                        while ((reader).Read())
                        {
      


                                    NAPI.Pools.GetAllVehicles().FindAll((Vehicle veh) => veh.GetVehicle() != null && veh.GetVehicle().Id == reader.GetInt32("Id")).ForEach(delegate (Vehicle veh)
                                    {
                                        ((Entity)veh).Delete();
                                    });

                                    if (!reader.GetString("Location").StartsWith("{"))
                                    {
                                    }
                                    else
                                    {



                                        Vehicle vehicle1 = NAPI.Vehicle.CreateVehicle(NAPI.Util.GetHashKey(reader.GetString("Vehiclehash")), NAPI.Util.FromJson<Vector3>(reader.GetString("Location")), reader.GetFloat("Rotation"), 0, 0, "", 255, false, false, 0);
                                        Dictionary<int, int> dictionary = new Dictionary<int, int>();
                                        string str = reader.GetString("Tuning");
                                        if ((str == null ? false : str != "[]"))
                                        {
                                            dictionary = NAPI.Util.FromJson<Dictionary<int, int>>(str);
                                        }
                                        DbVehicle dbVehicle = new DbVehicle()
                                        {
                                           
                                            Id = reader.GetInt32("Id"),
                                            Keys = NAPI.Util.FromJson<List<int>>(reader.GetString("Carkeys")),
                                            Model = reader.GetString("Vehiclehash"),
                                            OwnerId = reader.GetInt32("OwnerId"),
                                            Parked = Convert.ToBoolean(reader.GetInt32("Parked")),
                                            Plate = reader.GetString("Plate"),
                                            PrimaryColor = reader.GetInt32("PrimaryColor"),
                                            SecondaryColor = reader.GetInt32("SecondaryColor"),
                                            PearlescentColor = reader.GetInt32("PearlescentColor"),
                                            Impound = reader.GetBoolean("Impound"),
                                            fuel = reader.GetDouble("fuel"),

                                            WindowTint = reader.GetInt32("WindowTint"),
                                            Tuning = dictionary,
                                            Vehicle = vehicle1,
                                            InvRows = reader.GetInt32("InvRows"),
                                            Distance = reader.GetDouble("Distance"),
                                        };
                                        foreach (KeyValuePair<int, int> keyValuePair in dictionary)
                                        {
                                            vehicle1.SetMod(keyValuePair.Key, keyValuePair.Value);
                                        }
                                        vehicle1.Neons = (bool)Convert.ToBoolean((dynamic)dbVehicle.GetAttributeInt("Neons"));
                                        vehicle1.NeonColor = (Color)NAPI.Util.FromJson<Color>((dynamic)dbVehicle.GetAttributeString("NeonColor"));
                                        vehicle1.SetSharedData("headlightColor", (dynamic)dbVehicle.GetAttributeInt("HeadlightColor"));
                                        vehicle1.PrimaryColor = dbVehicle.PrimaryColor;
                                        vehicle1.SecondaryColor = dbVehicle.SecondaryColor;
                                        vehicle1.PearlescentColor = dbVehicle.PearlescentColor;
                                        vehicle1.WindowTint = dbVehicle.WindowTint;
                                        vehicle1.NumberPlate = dbVehicle.Plate;
                                        vehicle1.SetData("vehicle", dbVehicle);
                                        vehicle1.SetSharedData("lockedStatus", true);
                                        vehicle1.SetSharedData("kofferraumStatus", true);
                                        vehicle1.SetSharedData("engineStatus", false);
                                        vehicle1.Locked = true;
                                        vehicle1.EngineStatus = false;
                                        vehicle1.GetVehicle().UpdateVehicleFuel(reader.GetDouble("fuel"), false);
                                vehicle1.GetVehicle().fuel = reader.GetDouble("fuel");

                                Console.WriteLine("VEHICLE SPAWNED AFTER RESTART: " + vehicle1.NumberPlate);


                                    }

                                
                            
                        }
                    }
                }
                finally
                {
                    reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION respawnvehicles] " + ex.Message);
                Logger.Print("[EXCEPTION respawnvehicles] " + ex.StackTrace);
            }
            finally
            {
                query2.Connection.Dispose();
            }

            return true;

        }
    }
    
}
*/