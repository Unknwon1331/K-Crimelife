using GTANetworkAPI;
using Crimelife.Module.FraktionsVehicleShop;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crimelife
{
    public class FahrzeugShopFrakModule : Crimelife.Module.Module<FahrzeugShopFrakModule>
    {
        public static List<FahrzeugShopFrak> autoshopList = new List<FahrzeugShopFrak>();

        protected override bool OnLoad()
        {
            using MySqlConnection con = new MySqlConnection(Configuration.connectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM fahrzeugshops_frak";
                MySqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            autoshopList.Add(new FahrzeugShopFrak
                            {
                                Id = reader.GetInt32("Id"),
                                FactionId = reader.GetInt32("FactionId"),
                                Name = reader.GetString("Name"),
                                Position = NAPI.Util.FromJson<Vector3>(reader.GetString("Position")),
                                CarSpawn = NAPI.Util.FromJson<Vector3>(reader.GetString("CarSpawn")),
                                CarSpawnRotation = reader.GetFloat("CarSpawnRotation"),
                                BuyItems = NAPI.Util.FromJson<List<BuyCarFrak>>(reader.GetString("BuyItems"))
                            });
                        }
                    }
                }
                finally
                {
                    con.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION loadFahrzeugFrakShops] " + ex.Message);
                Logger.Print("[EXCEPTION loadFahrzeugFrakShops] " + ex.StackTrace);
            }
            finally
            {
                con.Dispose();
            }

            foreach (FahrzeugShopFrak autoshop in autoshopList)
            {
                ColShape val = NAPI.ColShape.CreateCylinderColShape(autoshop.Position, 1.4f, 1.4f, 0);
                val.SetData("FUNCTION_MODEL", new FunctionModel("openFrakVehicleShop", autoshop.Name, NAPI.Util.ToJson(autoshop.BuyItems)));
                val.SetData("MESSAGE", new Message("Benutze E um den Fraktions-Autohandel zu öffnen.", autoshop.Name, "white"));

                NAPI.Marker.CreateMarker(1, autoshop.Position.Subtract(new Vector3(0, 0, 0.3)), new Vector3(), new Vector3(), 0.8f, new Color(255, 165, 0), false, 0);
            }
            return true;
        }

        [RemoteEvent("openFrakVehicleShop")]
        public static void openFrakVehicleShop(Player client, string name, string items)
        {
            try
            {
                if (client == null) return;

                DbPlayer dbPlayer = client.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null) return;
                foreach (FahrzeugShopFrak autoshop in autoshopList)
                {
                    if (dbPlayer.Faction.Id == autoshop.FactionId)
                    {


                        if (dbPlayer.Factionrank < 10)
                        {
                            dbPlayer.SendNotification("Du besitzt einen zu niedrigen Rang für diese Aktion!");
                            return;
                        }

                        List<BuyCarFrak> list = NAPI.Util.FromJson<List<BuyCarFrak>>(items);
                        List<NativeItem> list2 = new List<NativeItem>();
                        foreach (BuyCarFrak item in list)
                        {
                            list2.Add(new NativeItem(item.Vehicle_Name + " - " + item.Price + " $",
                                item.Vehicle_Name + "-" + name));
                        }

                        NativeMenu nativeMenu = new NativeMenu("Fraktionsautohandel", name, list2);
                        dbPlayer.ShowNativeMenu(nativeMenu);
                    }
                    else
                    {
                        dbPlayer.SendNotification("Du bist nicht in der Fraktion!");
                        return;

                    }


                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION openVehicleShopFrak] " + ex.Message);
                Logger.Print("[EXCEPTION openVehicleShopFrak] " + ex.StackTrace);
            }

        }

        [RemoteEvent("nM-Fraktionsautohandel")]
        public static void Autohandel(Player client, string selection)
        {
            if (client == null) return;

            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;



            try
            {
                string[] args = selection.Split("-");
                FahrzeugShopFrak autoShop = autoshopList.FirstOrDefault((FahrzeugShopFrak a) => a.Name == args[1]);
                if (autoShop == null) return;
                BuyCarFrak buyCar = autoShop.BuyItems.FirstOrDefault((BuyCarFrak i) => i.Vehicle_Name == args[0]);
                if (buyCar == null) return;
                if (dbPlayer.Money >= buyCar.Price)
                {
                    Vehicle val = NAPI.Vehicle.CreateVehicle(NAPI.Util.GetHashKey(buyCar.Vehicle_Name.ToLower()), autoShop.CarSpawn, autoShop.CarSpawnRotation, 0, 0, "", 255, false, true, client.Dimension);

                    dbPlayer.removeMoney(Convert.ToInt32(buyCar.Price));
                    client.TriggerEvent("componentServerEvent", "NativeMenu", "hide");
                    dbPlayer.SendNotification("Du hast das Fahrzeug " + buyCar.Vehicle_Name + " für " + buyCar.Price + "$ erfolgreich gekauft.", 3000, "green", "Autohandel");

                    MySqlQuery query5 = new MySqlQuery("SELECT MAX(id) as maxId FROM fraktion_vehicles");
                    MySqlResult mySqlReaderCon5 = MySqlHandler.GetQuery(query5);
                    if (!mySqlReaderCon5.Reader.HasRows) return;
                    mySqlReaderCon5.Reader.Read();
                    int fahrzeugid = mySqlReaderCon5.Reader.GetInt32("maxId") + 1;
                    string plate = dbPlayer.Faction.Short;
                    //int id = new Random().Next(10000, 99999999);
                    List<int> list = new List<int>();
                    list.Add(dbPlayer.Faction.Id);

                    MySqlQuery mySqlQuery = new MySqlQuery("INSERT INTO `fraktion_vehicles` (`Id`, `Model`, `Parked`, `FactionId`) VALUES (@id, @vehiclehash, @parked, @factionid)");
                    mySqlQuery.AddParameter("@vehiclehash", buyCar.Vehicle_Name.ToLower());
                    mySqlQuery.AddParameter("@parked", 0);
                    mySqlQuery.AddParameter("@factionid", dbPlayer.Faction.Id);
                    mySqlQuery.AddParameter("@id", fahrzeugid);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    DbVehicle dbVehicle = new DbVehicle
                    {
                        Id = fahrzeugid,
                        Keys = list,
                        Model = buyCar.Vehicle_Name.ToLower(),
                        OwnerId = dbPlayer.Id,
                        Parked = false,
                        Plate = plate,
                        PrimaryColor = dbPlayer.Faction.CustomCarColor,
                        SecondaryColor = dbPlayer.Faction.CustomCarColor,
                        PearlescentColor = dbPlayer.Faction.CustomCarColor,
                        Vehicle = val
                    };

                    val.SetSharedData("lockedStatus", true);
                    val.SetSharedData("kofferraumStatus", true);
                    val.SetSharedData("engineStatus", true);
                    val.Locked = true;

                    val.NumberPlate = plate.ToString();

                    val.SetData("vehicle", dbVehicle);

                    WebhookSender.SendMessage("Spieler kauft Fahrzeug", "Der Spieler " + dbPlayer.Name + " kauft das Fahrzeug " + dbVehicle.Model + " für " + buyCar.Price + "$. - " + dbPlayer.Faction.Name, Webhooks.autokauflogs, "Fahrzeugfraktionsshop");

                    mySqlReaderCon5.Reader.Dispose();

                    NAPI.Task.Run(() =>
                    {
                        val.Delete();
                    }, 2000);
                }
                else
                {
                    client.TriggerEvent("componentServerEvent", "NativeMenu", "hide");
                    dbPlayer.SendNotification("Du besitzt nicht genügend Geld auf der Frankbank.", 3000, "red", "Autohandel");
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Autohandel] " + ex.Message);
                Logger.Print("[EXCEPTION Autohandel] " + ex.StackTrace);
            }
        }
    }
}
