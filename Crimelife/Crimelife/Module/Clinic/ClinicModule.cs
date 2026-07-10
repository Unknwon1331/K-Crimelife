using GTANetworkAPI;
using System;
using System.Collections.Generic;


namespace Crimelife
{
    class ClinicModule : Crimelife.Module.Module<ClinicModule>
    {
        public static List<Clinic> clinicList = new List<Clinic>();

        protected override bool OnLoad()
        {
            clinicList.Add(new Clinic
            {
                Id = 1,
                Position = new Vector3(356.1, -596.33, 28.9)
            });

            foreach (Clinic clinic in clinicList)
            {
                ColShape p = NAPI.ColShape.CreateCylinderColShape(clinic.Position, 2.4f, 2.4f, 0);
                p.SetData("FUNCTION_MODEL", new FunctionModel("openClinic"));
                p.SetData("MESSAGE", new Message("Benutze E um die Schönheitsklinik zu betreten. (5.000$)", "[Schönheitsklinik]", "green", 3000));
                TextLabel textLabel = NAPI.TextLabel.CreateTextLabel("~w~Schönheitsklinik", new Vector3(356.1, -596.33, 29.0), 10.0f, 0.5f, 0, new Color(255, 255, 255));

                NAPI.Ped.CreatePed(NAPI.Util.GetHashKey("s_m_m_doctor_01"), new Vector3(356.1, -596.33, 28.8), 255, false, true, true, true, 0);
                NAPI.Blip.CreateBlip(468, clinic.Position, 1f, 0, "Schönheitsklinik", 255, 0, true, 0, 0);
            }

            return true;
        }

        [RemoteEvent("openClinic")]
        public void openClinic(Player player)
        {
            try
            {
                if (player == null) return;
                DbPlayer dbPlayer = player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                    return;

                if (dbPlayer.Money >= 5000)
                {                    
                    dbPlayer.SendNotification("Du hast die Schönheitsklinik betreten!", 3000, "green");                    
                    dbPlayer.removeMoney(5000);
                    dbPlayer.OpenCharacterCreator();
                    dbPlayer.SetPosition(new Vector3(402.8664, -996.4108, -99.00027));
                    dbPlayer.player.Eval("mp.players.local.setHeading(-185);");
                    
                }
                else
                {
                    dbPlayer.SendNotification("Du hast nicht genug Geld!", 3000, "red");
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION openClinic] " + ex.Message);
                Logger.Print("[EXCEPTION openClinic] " + ex.StackTrace);
            }
        }
    }
}
