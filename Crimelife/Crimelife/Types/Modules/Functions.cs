using GTANetworkAPI;
using GVMP.Handlers;
using System;
using System.Collections.Generic;
using GVMP;

namespace Crimelife
{
    public static class Functions
    {
        public static void disableAllPlayerActions(this DbPlayer dbPlayer, bool val)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.ACWait();
            dbPlayer.player.TriggerEvent("disableAllPlayerActions", val);
        }

        public static void disableControlsNew(this DbPlayer dbPlayer, bool val)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.ACWait();
            dbPlayer.player.TriggerEvent("disableControlsNew", val);
        }

        public static void disableAllControls(this DbPlayer dbPlayer, bool val)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.ACWait();
            dbPlayer.player.TriggerEvent("disableAllControls", val);
        }

        public static void TriggerEvent(this DbPlayer dbPlayer, string eventName, params object[] args)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.player.TriggerEvent(eventName, args);
        }

        public static void StopAnimation(this DbPlayer dbPlayer)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.ACWait();
            dbPlayer.player.StopAnimation();
        }

        public static void SetHealth(this DbPlayer dbPlayer, int num)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.ACWait();
            dbPlayer.player.Health = num;
        }

        public static void SetArmor(this DbPlayer dbPlayer, int num)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.ACWait();
            dbPlayer.player.Armor = num;
        }

        public static void SetPosition(this DbPlayer dbPlayer, Vector3 location)
        {
            if (dbPlayer.player == null) return;
            Player client = dbPlayer.player;
            if (client == null || !client.Exists) return;
            Constants.SetPlayerACFlag(client);
            dbPlayer.ACWait();
            dbPlayer.player.Position = location;
        }

        public static void SetDimension(this DbPlayer dbPlayer, int dimension)
        {
            if (dbPlayer.player == null) return;
            Constants.SetPlayerACFlag(dbPlayer.player);
            dbPlayer.ACWait();
            dbPlayer.player.Dimension = Convert.ToUInt32(dimension);
        }

        public static int GetHealth(this DbPlayer dbPlayer)
        {
            if (dbPlayer.player == null) return 0;
            return dbPlayer.player.Health;
        }

        public static int GetArmor(this DbPlayer dbPlayer)
        {
            return dbPlayer.player.Armor;
        }

        public static Vector3 GetPosition(this DbPlayer dbPlayer)
        {
            if (dbPlayer.player == null) return new Vector3();
            return dbPlayer.player.Position;
        }

        public static int GetDimension(this DbPlayer dbPlayer)
        {
            if (dbPlayer.player == null) return 0;
            return Convert.ToInt32(dbPlayer.player.Dimension);
        }

        public static void ACWait(this DbPlayer dbPlayer)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.TriggerEvent("client:respawning");
            NAPI.Task.Run(() =>
            {
                dbPlayer.player.SetData("DisableAC", true);
            });

            NAPI.Task.Run(() =>
            {
                dbPlayer.player.SetData("DisableAC", false);
            }, 8000);
        }

        public static void SetClothes(this DbPlayer dbPlayer, int componentId, int drawableId, int textureId)
        {
            if (dbPlayer.player == null) return;
            try
            {
                dbPlayer.ACWait();

                Player c = dbPlayer.player;

                c.SetClothes(componentId, drawableId, textureId);

                c.Eval($"mp.events.callRemote('ChangeClothes', {componentId}, {drawableId}, {textureId});");
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION setClothes] " + ex.Message);
                Logger.Print("[EXCEPTION setClothes] " + ex.StackTrace);
            }
        }

        public static void SetTattoos(this DbPlayer dbPlayer, string collection, string overlay)
        {
            if (dbPlayer.player == null) return;
            try
            {
                dbPlayer.ACWait();

                Player c = dbPlayer.player;
                Decoration data = new Decoration();
                data.Collection = NAPI.Util.GetHashKey(collection);//"mpchristmas2_overlays"
                data.Overlay = NAPI.Util.GetHashKey(overlay);//"MP_Xmas2_M_Tat_005"
                dbPlayer.player.SetDecoration(data);

                /*Dictionary<int, clothingPart> clothes = new Dictionary<int, clothingPart>();

                if (c.HasSharedData("syncedClothes") && c.GetSharedData("syncedClothes") != null)
                {
                    clothes = NAPI.Util.FromJson<Dictionary<int, clothingPart>>(c.GetSharedData("syncedClothes"));
                }

                clothingPart clothing = new clothingPart();
                clothing.drawable = drawableId;
                clothing.texture = textureId;
                if (clothes.ContainsKey(componentId))
                {
                    clothes[componentId] = clothing;
                }
                else
                {
                    clothes.Add(componentId, clothing);
                }

                string syncedClothes = NAPI.Util.ToJson(clothes);
                c.SetSharedData("syncedClothes", syncedClothes);*/
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION setClothes] " + ex.Message);
                Logger.Print("[EXCEPTION setClothes] " + ex.StackTrace);
            }
        }

        public static void PlayAnimation(this DbPlayer dbPlayer, int flag, string animDict, string animName, float speed = 8f)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.ACWait();
            NAPI.Player.PlayPlayerAnimation(dbPlayer.player, flag, animDict, animName, speed);
        }

        public static void GiveWeapon(this DbPlayer dbPlayer, WeaponHash weaponHash, int ammo, bool db = false)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.ACWait();
            if (db)
            {
                WeaponManager.addWeapon(dbPlayer.player, weaponHash);
            }
            else
            {
                dbPlayer.player.GiveWeapon(weaponHash, ammo);
            }
        }

        public static void RemoveWeapon(this DbPlayer dbPlayer, WeaponHash weaponHash, bool db = false)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.ACWait();
            if (db)
            {
                WeaponManager.removeWeapon(dbPlayer.player, weaponHash);
            }
            else
            {
                dbPlayer.player.RemoveWeapon(weaponHash);
            }
        }
        public static void SetMoveInventoryItemCooldown(this DbPlayer player)
        {
            player.SetData("currentMoveCooldown", true);
            NAPI.Task.Run(() =>
            {
                player.ResetData("currentMoveCooldown");
            }, 750);
        }
        public static void RemoveAllWeapons(this DbPlayer dbPlayer, bool db = false)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.ACWait();
            Constants.SetPlayerACFlag(dbPlayer.player);
            if (db)
            {
                WeaponManager.removeAllWeapons(dbPlayer.player);
            }
            else
            {
                dbPlayer.player.RemoveAllWeapons();
            }
        }

        public static void SpawnPlayer(this DbPlayer dbPlayer, Vector3 loc)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.ACWait();
            NAPI.Player.SpawnPlayer(dbPlayer.player, loc);
        }

        public static void StartScreenEffect(this DbPlayer dbPlayer, string effectName, int duration, bool looped)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.TriggerEvent("startScreenEffect", effectName, duration, looped);
        }

        public static void StopScreenEffect(this DbPlayer dbPlayer, string effectName)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.TriggerEvent("stopScreenEffect", effectName);
        }

        public static void SetInvincible(this DbPlayer dbPlayer, bool val)
        {
            if (dbPlayer.player == null) return;
            dbPlayer.ACWait();
            dbPlayer.player.SetSharedData("PLAYER_INVINCIBLE", val);
        }

        public static HouseClass GetNearestHouseClass(this DbPlayer dbPlayer)
        {
            if (dbPlayer.player == null) return null;
            HouseClass nearestHouseClass = null;
            float nearestDistance = 999999;

            foreach (HouseClass house in HouseClassModule.houseClasses)
            {
                if (house.HouseLocation.DistanceTo(dbPlayer.player.Position) < nearestDistance)
                {
                    nearestHouseClass = house;
                    nearestDistance = house.HouseLocation.DistanceTo(dbPlayer.player.Position);
                }
            }

            if (nearestDistance > 10) return null;

            return nearestHouseClass;
        }

        public static House GetNearestHouse(this DbPlayer dbPlayer)
        {
            if (dbPlayer.player == null) return null;
            House nearestHouse = null;
            float nearestDistance = 999999;

            foreach (House house in HouseModule.houses)
            {
                if (house.Entrance.DistanceTo(dbPlayer.player.Position) < nearestDistance)
                {
                    nearestHouse = house;
                    nearestDistance = house.Entrance.DistanceTo(dbPlayer.player.Position);
                }
            }

            if (nearestDistance > 10) return null;

            return nearestHouse;
        }

        public static House GetHouse(this DbPlayer dbPlayer)
        {
            if (dbPlayer.player == null) return null;
            House house2 = null;

            foreach (House house in HouseModule.houses)
            {
                if (house.TenantsIds.Contains(dbPlayer.Id) || house.OwnerId == dbPlayer.Id)
                {
                    house2 = house;
                    break;
                }
            }

            return house2;
        }

        public static Vehicle GetNearestVehicle(this DbPlayer dbPlayer)
        {
            if (dbPlayer.player == null) return null;
            Vehicle nearestVeh = null;
            float nearestDistance = 999999;

            foreach (Vehicle veh in NAPI.Pools.GetAllVehicles())
            {
                if (veh.Position.DistanceTo(dbPlayer.player.Position) < nearestDistance)
                {
                    nearestVeh = veh;
                    nearestDistance = veh.Position.DistanceTo(dbPlayer.player.Position);
                }
            }

            if (nearestDistance > 10) return null;

            return nearestVeh;
        }

        public static House GetNearestHouseFromInterior(this DbPlayer dbPlayer)
        {
            if (dbPlayer.player == null) return null;
            HouseClass houseClass = dbPlayer.GetNearestHouseClass();
            House house = null;

            if (houseClass == null) return null;

            foreach (House house2 in HouseModule.houses)
            {
                bool InHouse = false;
                if (dbPlayer.HasData("IN_HOUSE"))
                {
                    InHouse = dbPlayer.GetData<int>("IN_HOUSE") == house.Id;
                }
                if (house2.Class.Id == houseClass.Id && (house2.OwnerId == dbPlayer.Id || house2.TenantsIds.Contains(dbPlayer.Id) || InHouse))
                {
                    house = house2;
                }
            }

            return house;
        }

        public static Vehicle GetClosestVehicle(this DbPlayer dbPlayer, float distance = 1000f)
        {
            if (dbPlayer.player == null) return null;
            Vehicle result = null;
            foreach (Vehicle allVehicle in NAPI.Pools.GetAllVehicles())
            {
                Vector3 entityPosition = NAPI.Entity.GetEntityPosition(allVehicle);
                float num = dbPlayer.player.Position.DistanceTo(entityPosition);
                if (num < distance)
                {
                    distance = num;
                    result = allVehicle;
                }
            }
            return result;
        }

        public static void ApplyCharacter(this DbPlayer dbPlayer)
        {
            if (dbPlayer.player == null) return;
            //dbPlayer.ACWait();
            Player c = dbPlayer.player;
            if (c == null) return;
            CustomizeModel customizeModel = NAPI.Util.FromJson<CustomizeModel>(dbPlayer.GetAttributeString("customization"));
            if (customizeModel == null) return;
            Dictionary<int, HeadOverlay> dictionary = new Dictionary<int, HeadOverlay>();

            if (customizeModel.customization.Appearance[9] != null)
                dictionary.Add(1, ClothingManager.CreateHeadOverlay((byte)customizeModel.customization.Appearance[1].Value, (byte)customizeModel.customization.Appearance[9].Value, 0, customizeModel.customization.Appearance[1].Opacity));

            dictionary.Add(2, ClothingManager.CreateHeadOverlay(2, (byte)customizeModel.customization.Appearance[2].Value, 0, customizeModel.customization.Appearance[2].Opacity));
            dictionary.Add(3, ClothingManager.CreateHeadOverlay(3, (byte)customizeModel.customization.Appearance[3].Value, 0, customizeModel.customization.Appearance[3].Opacity));
            dictionary.Add(4, ClothingManager.CreateHeadOverlay(4, (byte)customizeModel.customization.Appearance[4].Value, 0, customizeModel.customization.Appearance[4].Opacity));
            dictionary.Add(5, ClothingManager.CreateHeadOverlay(5, (byte)customizeModel.customization.Appearance[5].Value, 0, customizeModel.customization.Appearance[5].Opacity));
            dictionary.Add(8, ClothingManager.CreateHeadOverlay(8, (byte)customizeModel.customization.Appearance[8].Value, 0, customizeModel.customization.Appearance[8].Opacity));
            HeadBlend val = default(HeadBlend);
            val.ShapeFirst = (byte)customizeModel.customization.Parents.MotherShape;
            val.ShapeSecond = (byte)customizeModel.customization.Parents.FatherShape;
            val.ShapeThird = 0;
            val.SkinFirst = (byte)customizeModel.customization.Parents.MotherSkin;
            val.SkinSecond = (byte)customizeModel.customization.Parents.FatherSkin;
            val.SkinThird = 0;
            val.ShapeMix = customizeModel.customization.Parents.Similarity;
            val.SkinMix = customizeModel.customization.Parents.SkinSimilarity;
            val.ThirdMix = 0f;
            HeadBlend val2 = val;
            bool flag = customizeModel.customization.Gender == 0;
            c.SetCustomization(flag, val2, (byte)customizeModel.customization.EyeColor, (byte)customizeModel.customization.Hair.Color, (byte)customizeModel.customization.Hair.HighlightColor, customizeModel.customization.Features.ToArray(), dictionary, (Decoration[])(object)new Decoration[0]);

            dbPlayer.SetClothes(2, customizeModel.customization.Hair.Hair, 0);

            PlayerClothes playerClothes2 = NAPI.Util.FromJson<PlayerClothes>(dbPlayer.GetAttributeString("clothes"));
            if (playerClothes2 == null) return;

            if (playerClothes2.Hut != null)
                c.SetAccessories(0, playerClothes2.Hut.drawable, playerClothes2.Hut.texture);
            if (playerClothes2.Brille != null)
                c.SetAccessories(1, playerClothes2.Brille.drawable, playerClothes2.Brille.texture);
            if (playerClothes2.Maske != null)
                dbPlayer.SetClothes(1, playerClothes2.Maske.drawable, playerClothes2.Maske.texture);
            if (playerClothes2.Oberteil != null)
                dbPlayer.SetClothes(11, playerClothes2.Oberteil.drawable, playerClothes2.Oberteil.texture);
            if (playerClothes2.Unterteil != null)
                dbPlayer.SetClothes(8, playerClothes2.Unterteil.drawable, playerClothes2.Unterteil.texture);
            if (playerClothes2.Kette != null)
                dbPlayer.SetClothes(7, playerClothes2.Kette.drawable, playerClothes2.Kette.texture);
            if (playerClothes2.Koerper != null)
                dbPlayer.SetClothes(3, playerClothes2.Koerper.drawable, playerClothes2.Koerper.texture);
            if (playerClothes2.Hose != null)
                dbPlayer.SetClothes(4, playerClothes2.Hose.drawable, playerClothes2.Hose.texture);
            if (playerClothes2.Schuhe != null)
                dbPlayer.SetClothes(6, playerClothes2.Schuhe.drawable, playerClothes2.Schuhe.texture);
            dbPlayer.PlayerClothes = playerClothes2;
            dbPlayer.RefreshData(dbPlayer);
        }
    }
}
