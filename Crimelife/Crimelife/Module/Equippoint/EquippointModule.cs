using GTANetworkAPI;
using GVMP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Crimelife
{
    class EquippointModule : Crimelife.Module.Module<EquippointModule>
    {
        public static List<string> equipItems = new List<string>();
        public static List<int> alreadyEquipped = new List<int>();
        public static List<EquippointModel> equipList = new List<EquippointModel>();

        protected override bool OnLoad()
        {
            equipItems.Add("Advancedrifle");
            equipItems.Add("Gusenberg");

            equipList.Add(new EquippointModel
            {
                Name = "Arcadius",
                Position = new Vector3(-117.08, -604.55, 36.28)
            });


            foreach (EquippointModel equippointModel in equipList)
            {
                ColShape cb = NAPI.ColShape.CreateCylinderColShape(equippointModel.Position, 7.4f, 2.4f, 0);
                cb.SetData("FUNCTION_MODEL", new FunctionModel("useEquippoint"));
                cb.SetData("MESSAGE", new Message("Benutze E um die was zuholen", "Händler", "grey", 3000));

                NAPI.Ped.CreatePed(NAPI.Util.GetHashKey("u_m_o_finguru_01"), equippointModel.Position, 255, false, true, true, true, 0);
                NAPI.Blip.CreateBlip(66, equippointModel.Position, 1f, 0, "Händler " + equippointModel.Name, 255, 0.0f, true, 0, 0);


            }

            return true;
        }

        [RemoteEvent("useEquippoint")]
        public static void equipPlayer(Player c)
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
                        dbPlayer.SendNotification("Du hast deine Sachen bereits abgeholt", 3000,
                            "grey", "Händler");
                        return;
                    }

                    alreadyEquipped.Add(dbPlayer.Id);
                    dbPlayer.SendProgressbar(5000);
                    dbPlayer.disableAllPlayerActions(true);
                    dbPlayer.PlayAnimation(33, "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 8f);
                    NAPI.Task.Run(() =>
                    {
                        var r = new Random();
                        string item = equipItems[r.Next(0, equipItems.Count)];
                        dbPlayer.UpdateInventoryItems("HeavyPistol", 1, false);
                        dbPlayer.UpdateInventoryItems("Schutzweste", 25, false);
                        dbPlayer.UpdateInventoryItems("Verbandskasten", 25, false);
                        dbPlayer.UpdateInventoryItems(item, 1, false);
                        dbPlayer.StopProgressbar();
                        dbPlayer.SendNotification(
                            "Du hast deine Sachen abgeholt. (+ 1x " + item + ")", 3000, "grey",
                            "Händler");
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
