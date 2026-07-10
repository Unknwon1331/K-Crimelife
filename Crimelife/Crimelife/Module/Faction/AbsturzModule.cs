using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Crimelife.Module;

namespace Crimelife
{
    class AbsturzModule : Module<AbsturzModule>
    {
        public static List<Item> itemRegisterList = new List<Item>();

        protected override bool OnLoad()
        {

            // Flugzeug Crash

            Vector3 vector3 = new Vector3(1374.25, -739.77, 67.23);
            ColShape colShape = NAPI.ColShape.CreateCylinderColShape(vector3, 1.4f, 2.4f, 0);
            NAPI.Marker.CreateMarker(36, new Vector3(-92.78, 3037.61, 34.72), new Vector3(), new Vector3(), 1f, new Color(255, 140, 0));
            colShape.SetData("FUNCTION_MODEL", new FunctionModel("startAB"));
            colShape.SetData("MESSAGE", new Message("Benutze E um die Box aufzuschweißen!", "ABSTURZ", "red", 3000));

            // Helikopter Crash

            Vector3 vector4 = new Vector3(1624.19, 3865.32, 32.49);
            ColShape colShape1 = NAPI.ColShape.CreateCylinderColShape(vector4, 1.4f, 2.4f, 0);
            NAPI.Marker.CreateMarker(36, new Vector3(1624.19, 3865.32, 32.49), new Vector3(), new Vector3(), 1f, new Color(255, 140, 0));
            colShape1.SetData("FUNCTION_MODEL", new FunctionModel("startHELI"));
            colShape1.SetData("MESSAGE", new Message("Benutze E um die Box aufzuschweißen!", "ABSTURZ", "red", 3000));
            return true;
        }


        [RemoteEvent("startAB")]
        public static void startAB(Player c)
        {
            DbPlayer dbPlayer = c.GetPlayer();
            {
                try
                {
                    if (c.IsInVehicle || (dynamic)dbPlayer.GetAttributeInt("Absturz") != 0)
                    {
                        dbPlayer.SendNotification("Diese Box wurde bereits geleert.", 3000, "red");
                        return;
                    }
                    if (!dbPlayer.IsFarming || !dbPlayer.DeathData.IsDead) ;
                    {
                        MySqlHandler.ExecuteSync(new MySqlQuery("UPDATE accounts SET Absturz = 1"));
                        dbPlayer.disableAllPlayerActions(true);
                        dbPlayer.SendProgressbar(4000);
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
                            if (dbPlayer.DeathData.IsDead)
                            {
                                MySqlHandler.ExecuteSync(new MySqlQuery("UPDATE accounts SET Absturz = 0"));
                                return;
                            }
                            dbPlayer.SendNotification("Du hast erfolgreich deine Beute erhalten. Entferne dich schnell von diesem Ort!", 3000, "lightblue");
                            dbPlayer.StopAnimation();
                            dbPlayer.UpdateInventoryItems("Advancedrifle", 30, false);
                            dbPlayer.UpdateInventoryItems("Gusenberg", 34, false);
                        }, 4000);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION openAB] " + ex.Message);
                    Logger.Print("[EXCEPTION openAB] " + ex.StackTrace);
                }
            }
        }

        [RemoteEvent("startHELI")]
        public static void startHELI(Player c)
        {
            DbPlayer dbPlayer = c.GetPlayer();
            {
                try
                {
                    if (c.IsInVehicle || (dynamic)dbPlayer.GetAttributeInt("Absturz") != 0)
                    {
                        dbPlayer.SendNotification("Diese Box wurde bereits geleert.", 3000, "red");
                        return;
                    }
                    if (!dbPlayer.IsFarming || !dbPlayer.DeathData.IsDead) ;
                    {
                        MySqlHandler.ExecuteSync(new MySqlQuery("UPDATE accounts SET Absturz = 1"));
                        dbPlayer.disableAllPlayerActions(true);
                        dbPlayer.SendProgressbar(4000);
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
                            if (dbPlayer.DeathData.IsDead)
                            {
                                MySqlHandler.ExecuteSync(new MySqlQuery("UPDATE accounts SET Absturz = 0"));
                                return;
                            }
                            dbPlayer.SendNotification("Du hast erfolgreich deine Beute erhalten. ( +15 Advancedrifle ) ( +5 Gusenberg )", 3000, "lightblue");
                            dbPlayer.StopAnimation();
                            dbPlayer.UpdateInventoryItems("Advancedrifle", 15, false);
                            dbPlayer.UpdateInventoryItems("Gusenberg", 5, false);
                        }, 4000);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION openHELI] " + ex.Message);
                    Logger.Print("[EXCEPTION openHELI] " + ex.StackTrace);
                }
            }
        }
    }
}
