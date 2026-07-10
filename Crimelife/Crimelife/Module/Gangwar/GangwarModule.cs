using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crimelife
{
    class GangwarModule : Crimelife.Module.Module<GangwarModule>
    {
        public static List<Gangwar> GWZones = new List<Gangwar>();
        public static List<Gangwar> BlockedZones = new List<Gangwar>();
        public static Gangwar RunningGangwar = null;

        public override Type[] RequiredModules() => new Type[1]
        {
            typeof (FactionModule)
        };

        protected override bool OnLoad()
        {
            MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM gangwars");
            MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery);
            MySqlDataReader reader = mySqlResult.Reader;

            while (reader.Read())
            {
                if (reader.GetBoolean("active") == true)
                {
                    Gangwar gangwar = new Gangwar
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name"),
                        Faction = FactionModule.getFactionById(reader.GetInt32("Faction")),
                        Zone = NAPI.Util.FromJson<Vector3>(reader.GetString("Zone")),
                        Blippos = NAPI.Util.FromJson<Vector3>(reader.GetString("Blip")),
                        Range = reader.GetFloat("range"),
                        Flag1 = new Flag(NAPI.Util.FromJson<Vector3>(reader.GetString("Flag1"))),
                        Flag2 = new Flag(NAPI.Util.FromJson<Vector3>(reader.GetString("Flag2"))),
                        Flag3 = new Flag(NAPI.Util.FromJson<Vector3>(reader.GetString("Flag3"))),
                        Flag4 = new Flag(NAPI.Util.FromJson<Vector3>(reader.GetString("Flag4"))),
                        Attacker = null,
                        AttackerPoints = 0,
                        FactionPoints = 0,
                        StopDate = DateTime.Now,
                        AttackerSpawn = NAPI.Util.FromJson<Vector3>(reader.GetString("attackerspawn")),
                        DefenderSpawn = NAPI.Util.FromJson<Vector3>(reader.GetString("defenderspawn")),
                        AttackerVehicleSpawn = NAPI.Util.FromJson<Vector3>(reader.GetString("attackervehicle")),
                        AttackerVehicleRotation = reader.GetFloat("attackervehiclerotation"),
                        DefenderVehicleSpawn = NAPI.Util.FromJson<Vector3>(reader.GetString("defendervehicle")),
                        DefenderVehicleRotation = reader.GetFloat("defendervehiclerotation"),
                        AttackerParkoutPoint = NAPI.Util.FromJson<Vector3>(reader.GetString("attackerparkout")),
                        DefenderParkoutPoint = NAPI.Util.FromJson<Vector3>(reader.GetString("defenderparkout"))
                    };
                    GWZones.Add(gangwar);
                    // Console.WriteLine(gangwar.AttackerSpawn);
                    // Console.WriteLine(gangwar.Id + " " + gangwar.Name + " " + gangwar.Faction.Name);

                    ColShape c = NAPI.ColShape.CreateCylinderColShape(gangwar.Blippos, 1.4f, 1.4f, 0);
                    c.SetData("FUNCTION_MODEL", new FunctionModel("StartGangwar", gangwar.Id));
                    c.SetData("MESSAGE", new Message("Benutze E um einen Gangwar zu starten.", "GANGWAR", gangwar.Faction.GetRGBStr(), 3000));

                    ColShape colShape2 = NAPI.ColShape.CreateCylinderColShape(gangwar.AttackerParkoutPoint, 1.4f, 1.4f, uint.MaxValue);
                    colShape2.SetData("FUNCTION_MODEL", new FunctionModel("openFraktionsGarage", "5564", "frak"));
                    colShape2.SetData("MESSAGE", new Message("Benutze E um das Gangwar Menu zu öffnen.", "frak", "frak"));
                    NAPI.Marker.CreateMarker(4, gangwar.AttackerParkoutPoint.Add(new Vector3(0, 0, 1)), new Vector3(), new Vector3(), 1.0f, gangwar.Faction.RGB, false, Convert.ToUInt32(FactionModule.GangwarDimension));


                    ColShape colShape3 = NAPI.ColShape.CreateCylinderColShape(gangwar.DefenderParkoutPoint, 1.4f, 1.4f, uint.MaxValue);
                    colShape3.SetData("FUNCTION_MODEL", new FunctionModel("openFraktionsGarage", "5564", "frak"));
                    colShape3.SetData("MESSAGE", new Message("Benutze E um das Gangwar Menu zu öffnen.", "frak", "frak"));
                    NAPI.Marker.CreateMarker(4, gangwar.DefenderParkoutPoint.Add(new Vector3(0, 0, 1)), new Vector3(), new Vector3(), 1.0f, gangwar.Faction.RGB, false, Convert.ToUInt32(FactionModule.GangwarDimension));


                    NAPI.Blip.CreateBlip(543, gangwar.Zone, 1.0f, (byte)gangwar.Faction.Blip, "GW - " + gangwar.Name + " - " + gangwar.Faction.Short, 255, 0, true, 0, uint.MaxValue);
                    NAPI.Marker.CreateMarker(1, gangwar.Blippos, new Vector3(), new Vector3(), 1.0f, gangwar.Faction.RGB, false, 0);
                }
            }

            reader.Dispose();
            mySqlResult.Connection.Dispose();
            return true;
        }

        public static Gangwar FindGWById(int Id)
        {
            return GWZones.FirstOrDefault((Gangwar GWZone) => GWZone.Id == Id);
        }

        [RemoteEvent("StartGangwar")]
        public void StartGangwar(Player c, int id)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                    return;

                if (dbPlayer.Faction.Id == 0) return;

                List<NativeItem> nativeItems = new List<NativeItem>
                {
                    new NativeItem("Gebiete Informationen", "tex"),//noch machen das es geht
                    new NativeItem("Gebiet Angreifen", id.ToString())
                };
                NativeMenu nativeMenu = new NativeMenu("Gangwar", "⚔️K-Crimelife Gangwar-System", nativeItems);
                dbPlayer.ShowNativeMenu(nativeMenu);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION StartGangwar] " + ex.Message);
                Logger.Print("[EXCEPTION StartGangwar] " + ex.StackTrace);
            }
        }

        [RemoteEvent("nM-Gangwar")]
        public void Gangwar(Player c, string selection)
        {
            try
            {
                if (c == null) return;
                int id = 0;
                bool id2 = int.TryParse(selection, out id);
                if (!id2) return;
                if (id == 0) return;

                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null) return;

                if (dbPlayer.Faction.Id == 0) return;

                dbPlayer.CloseNativeMenu();

                Gangwar gw = FindGWById(id);

                if (selection == "tex")
                {
                    dbPlayer.SendNotification("Besitzer: " + gw.Faction, 7000, "orange", gw.Name);
                    dbPlayer.CloseNativeMenu();
                }

                if (gw == null) return;

                else
                {
                    if (dbPlayer.Faction == gw.Faction) return;


                    if (dbPlayer.Faction == gw.Faction) return;

                    if (gw.Faction.GetFactionPlayers().Count < 0)
                    {
                        dbPlayer.SendNotification("Es müssen mindestens 5 Personen aus der anderen Fraktion online sein.", 3000, "orange", "GANGWAR");
                        return;
                    }
                    if (BlockedZones.Contains(gw))
                    {
                        dbPlayer.SendNotification("Dieses Gebiet wurde bereits angegriffen.", 3000, "orange", "GANGWAR");
                        return;
                    }
                    if (RunningGangwar != null)
                    {
                        dbPlayer.SendNotification("Es läuft bereits ein Gangwar.", 3000, "orange", "GANGWAR");
                        return;
                    }
                    if (gw.Faction.Id == 0)
                    {
                        dbPlayer.SendNotification("Du hast das Gebiet eingenommen.", 3000, "orange", "GANGWAR");
                        GWZones.Remove(gw);
                        gw.Faction = dbPlayer.Faction;
                        GWZones.Add(gw);
                        MySqlQuery mySqlQuery = new MySqlQuery("UPDATE gangwars SET Faction = @faction WHERE Id = @id");
                        mySqlQuery.AddParameter("@id", gw.Id);
                        mySqlQuery.AddParameter("@faction", dbPlayer.Faction.Id);
                        MySqlHandler.ExecuteSync(mySqlQuery);
                        return;
                    }
                    gw.Attacker = dbPlayer.Faction;

                    dbPlayer.SetPosition(dbPlayer.Faction.Storage);

                    foreach (DbPlayer dbTarget in gw.Faction.GetFactionPlayers())
                    {

                        dbTarget.SendNotification($"Euer Gebiet {gw.Name} wird von der Fraktion {gw.Attacker.Name} angegriffen.", 16000, gw.Faction.GetRGBStr(), "GANGWAR");
                    }
                    foreach (DbPlayer dbTarget in gw.Attacker.GetFactionPlayers())
                    {
                        if (dbTarget != null && dbTarget.IsValid(true))
                            dbTarget.SendNotification($"Deine Fraktion greift das Gebiet {gw.Name} an.", 15000, gw.Attacker.GetRGBStr(), "GANGWAR");
                        dbTarget.SetData("runninggangwar", RunningGangwar);
                    }
                    Notification.SendGlobalNotification($"Die Fraktion {gw.Attacker.Name} greift das Gebiet {gw.Name} von der Fraktion {gw.Faction.Name} an.", 8000, "orange", Notification.icon.warn);
                    WebhookSender.SendMessage("Gangwar", $"Die Fraktion {gw.Attacker.Name} greift das Gebiet {gw.Name} von der Fraktion {gw.Faction.Name} an.", Webhooks.gangwar, "Gangwar");

                    BlockedZones.Add(gw);
                    RunningGangwar = gw;

                    gw.StopDate = DateTime.Now.AddMinutes(5);

                    ColShape col = NAPI.ColShape.CreateCylinderColShape(gw.Zone, gw.Range, 25f, Convert.ToUInt32(FactionModule.GangwarDimension));
                    col.SetData("GANGWAR", true);

                    Marker m = NAPI.Marker.CreateMarker(1, gw.Zone, new Vector3(), new Vector3(), gw.Range, gw.Faction.RGB, false, Convert.ToUInt32(FactionModule.GangwarDimension));
                    m.SetData("GANGWAR", true);

                    ColShape c1 = NAPI.ColShape.CreateCylinderColShape(gw.Flag1.Position.Add(new Vector3(0, 0, 1)), 1.4f, 1.4f, Convert.ToUInt32(FactionModule.GangwarDimension));
                    c1.SetData("GANGWAR_FLAG", 1);


                    ColShape c2 = NAPI.ColShape.CreateCylinderColShape(gw.Flag2.Position.Add(new Vector3(0, 0, 1)), 1.4f, 1.4f, Convert.ToUInt32(FactionModule.GangwarDimension));
                    c2.SetData("GANGWAR_FLAG", 2);


                    ColShape c3 = NAPI.ColShape.CreateCylinderColShape(gw.Flag3.Position.Add(new Vector3(0, 0, 1)), 1.4f, 1.4f, Convert.ToUInt32(FactionModule.GangwarDimension));
                    c3.SetData("GANGWAR_FLAG", 3);


                    ColShape c4 = NAPI.ColShape.CreateCylinderColShape(gw.Flag4.Position.Add(new Vector3(0, 0, 1)), 1.4f, 1.4f, Convert.ToUInt32(FactionModule.GangwarDimension));
                    c4.SetData("GANGWAR_FLAG", 4);

                    Marker Flag1 = NAPI.Marker.CreateMarker(4, gw.Flag1.Position.Add(new Vector3(0, 0, 1)), new Vector3(), new Vector3(), 1.0f, new Color(255, 255, 255), false, Convert.ToUInt32(FactionModule.GangwarDimension));
                    Marker Flag2 = NAPI.Marker.CreateMarker(4, gw.Flag2.Position.Add(new Vector3(0, 0, 1)), new Vector3(), new Vector3(), 1.0f, new Color(255, 255, 255), false, Convert.ToUInt32(FactionModule.GangwarDimension));
                    Marker Flag3 = NAPI.Marker.CreateMarker(4, gw.Flag3.Position.Add(new Vector3(0, 0, 1)), new Vector3(), new Vector3(), 1.0f, new Color(255, 255, 255), false, Convert.ToUInt32(FactionModule.GangwarDimension));
                    Marker Flag4 = NAPI.Marker.CreateMarker(4, gw.Flag4.Position.Add(new Vector3(0, 0, 1)), new Vector3(), new Vector3(), 1.0f, new Color(255, 255, 255), false, Convert.ToUInt32(FactionModule.GangwarDimension));
                    Flag1.SetData("FLAG", 1);
                    Flag2.SetData("FLAG", 2);
                    Flag3.SetData("FLAG", 3);
                    Flag4.SetData("FLAG", 4);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Gangwar] " + ex.Message);
                Logger.Print("[EXCEPTION Gangwar] " + ex.StackTrace);
            }
        }

        [ServerEvent(Event.PlayerEnterColshape)]
        public void EnterGWZone(ColShape col, Player c)
        {
            try
            {
                if (c == null || !c.Exists || col == null || !col.Exists) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null) return;
                if (RunningGangwar == null) return;
                if (dbPlayer.Faction != RunningGangwar.Faction && dbPlayer.Faction != RunningGangwar.Attacker) return;
                if (col.HasData("GANGWAR"))
                {
                    dbPlayer.SetData("IN_GANGWAR", true);
                    c.TriggerEvent("initializeGangwar", RunningGangwar.Faction.Short, RunningGangwar.Attacker.Short,
                        RunningGangwar.Faction.Id, RunningGangwar.Attacker.Id,
                        (int)(RunningGangwar.StopDate - DateTime.Now).TotalSeconds, RunningGangwar.Faction.Logo,
                        RunningGangwar.Attacker.Logo, RunningGangwar.Faction.GetRGBStr(),
                        RunningGangwar.Attacker.GetRGBStr());
                }
                else if (col.HasData("GANGWAR_FLAG"))
                {
                    int data = (int)((dynamic)col.GetData<Int32>("GANGWAR_FLAG"));
                    //dbPlayer.SendNotification(string.Concat("Du hast die Flagge ", data.ToString(), " betreten."), 3000, "orange", "GANGWAR");
                    switch (data)
                    {
                        case 1:
                            {
                                RunningGangwar.Flag1.Faction = dbPlayer.Faction.Id;
                                NAPI.Task.Run(() =>
                                {
                                    foreach (var x in NAPI.Pools.GetAllMarkers().ToList().Where(x => x.HasData("FLAG")))
                                    {
                                        if (x.GetData<int>("FLAG") == 1)
                                        {
                                            x.Color = dbPlayer.Faction.RGB;
                                        }
                                    }
                                }, 500);
                                break;
                            }
                        case 2:
                            {
                                RunningGangwar.Flag2.Faction = dbPlayer.Faction.Id;
                                NAPI.Task.Run(() =>
                                {
                                    foreach (var x in NAPI.Pools.GetAllMarkers().ToList().Where(x => x.HasData("FLAG")))
                                    {
                                        if (x.GetData<int>("FLAG") == 2)
                                        {
                                            x.Color = dbPlayer.Faction.RGB;
                                        }
                                    }
                                }, 500);
                                break;
                            }
                        case 3:
                            {
                                RunningGangwar.Flag3.Faction = dbPlayer.Faction.Id;
                                NAPI.Task.Run(() =>
                                {
                                    foreach (var x in NAPI.Pools.GetAllMarkers().ToList().Where(x => x.HasData("FLAG")))
                                    {
                                        if (x.GetData<int>("FLAG") == 3)
                                        {
                                            x.Color = dbPlayer.Faction.RGB;
                                        }
                                    }
                                }, 500);
                                break;
                            }
                        case 4:
                            {
                                RunningGangwar.Flag4.Faction = dbPlayer.Faction.Id;
                                NAPI.Task.Run(() =>
                                {
                                    foreach (var x in NAPI.Pools.GetAllMarkers().ToList().Where(x => x.HasData("FLAG")))
                                    {
                                        if (x.GetData<int>("FLAG") == 4)
                                        {
                                            x.Color = dbPlayer.Faction.RGB;
                                        }
                                    }
                                }, 500);
                                break;
                            }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION EnterGWZone] " + ex.Message);
                Logger.Print("[EXCEPTION EnterGWZone] " + ex.StackTrace);
            }
        }

        [ServerEvent(Event.PlayerExitColshape)]
        public void ExitGWZone(ColShape col, Player c)
        {
            try
            {
                if (c == null || !c.Exists || col == null || !col.Exists) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null) return;
                if (RunningGangwar == null) return;
                if (dbPlayer.Faction != RunningGangwar.Faction && dbPlayer.Faction != RunningGangwar.Attacker) return;
                if (col.HasData("GANGWAR"))
                {
                    dbPlayer.ResetData("IN_GANGWAR");
                    c.RemoveAllWeapons();
                    WeaponManager.loadWeapons(dbPlayer.player);
                    c.TriggerEvent("finishGangwar");
                }
                else if (col.HasData("GANGWAR_FLAG"))
                {
                    int data = (int)((dynamic)col.GetData<Int32>("GANGWAR_FLAG"));
                    // dbPlayer.SendNotification("Du hast die Flagge " + data + " verlassen.", 3000, "orange", "GANGWAR");

                    switch (data)
                    {
                        case 1:
                            {
                                RunningGangwar.Flag1.Faction = 0;
                                NAPI.Task.Run(() =>
                                {
                                    foreach (var x in NAPI.Pools.GetAllMarkers().ToList().Where(x => x.HasData("FLAG")))
                                    {
                                        if (x.GetData<int>("FLAG") == 1)
                                        {
                                            x.Color = new Color(255, 255, 255);
                                        }
                                    }
                                }, 500);
                                break;
                            }
                        case 2:
                            {
                                RunningGangwar.Flag2.Faction = 0;
                                NAPI.Task.Run(() =>
                                {
                                    foreach (var x in NAPI.Pools.GetAllMarkers().ToList().Where(x => x.HasData("FLAG")))
                                    {
                                        if (x.GetData<int>("FLAG") == 2)
                                        {
                                            x.Color = new Color(255, 255, 255);
                                        }
                                    }
                                }, 500);
                                break;
                            }
                        case 3:
                            {
                                RunningGangwar.Flag3.Faction = 0;
                                NAPI.Task.Run(() =>
                                {
                                    foreach (var x in NAPI.Pools.GetAllMarkers().ToList().Where(x => x.HasData("FLAG")))
                                    {
                                        if (x.GetData<int>("FLAG") == 3)
                                        {
                                            x.Color = new Color(255, 255, 255);
                                        }
                                    }
                                }, 500);
                                break;
                            }
                        case 4:
                            {
                                RunningGangwar.Flag4.Faction = 0;
                                NAPI.Task.Run(() =>
                                {
                                    foreach (var x in NAPI.Pools.GetAllMarkers().ToList().Where(x => x.HasData("FLAG")))
                                    {
                                        if (x.GetData<int>("FLAG") == 4)
                                        {
                                            x.Color = new Color(255, 255, 255);
                                        }
                                    }
                                }, 500);
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION ExitGWZone] " + ex.Message);
                Logger.Print("[EXCEPTION ExitGWZone] " + ex.StackTrace);
            }
        }


        public static void handleKill(DbPlayer killer)
        {
            try
            {
                if (killer == null || killer.player == null) return;
                if (!killer.HasData("IN_GANGWAR") || RunningGangwar == null) return;

                if (killer.Faction.Id == RunningGangwar.Faction.Id)
                {
                    RunningGangwar.FactionPoints += 3;
                }
                else if (killer.Faction.Id == RunningGangwar.Attacker.Id)
                {
                    RunningGangwar.AttackerPoints += 3;
                }

                killer.Faction.GetFactionPlayers().ForEach((DbPlayer dbPlayer) => dbPlayer.SendNotification($"+3 Punkte für das Töten eines Gegners! {killer.Name} hat {dbPlayer.Name} getötet.", 5000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name));

                RunningGangwar.Faction.GetFactionPlayers().ForEach(e =>
                {
                    if (e.player.Position.DistanceTo(RunningGangwar.Zone) < 50)
                        e.player.TriggerEvent("updateGangwarScore", RunningGangwar.FactionPoints,
                            RunningGangwar.AttackerPoints);
                });
                RunningGangwar.Attacker.GetFactionPlayers().ForEach(e =>
                {
                    if (e.player.Position.DistanceTo(RunningGangwar.Zone) < 50)
                        e.player.TriggerEvent("updateGangwarScore", RunningGangwar.FactionPoints,
                            RunningGangwar.AttackerPoints);
                });
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION handleKill] " + ex.Message);
                Logger.Print("[EXCEPTION handleKill] " + ex.StackTrace);
            }
        }

        public override void OnFiveSecUpdate()
        {
            try
            {
                if (RunningGangwar == null) return;

                if (RunningGangwar.StopDate < DateTime.Now)
                {
                    EndGangwar();
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Gangwar OnFiveSecUpdate] " + ex.Message);
                Logger.Print("[EXCEPTION Gangwar OnFiveSecUpdate] " + ex.StackTrace);
            }
        }

        public override void OnTenSecUpdate()
        {
            try
            {
                if (RunningGangwar == null) return;

                if (RunningGangwar.Flag1.Faction == RunningGangwar.Faction.Id)
                {
                    RunningGangwar.Faction.GetFactionPlayers().ForEach((DbPlayer dbPlayer) => dbPlayer.SendNotification("+1 Punkte für das Besetzen einer Flagge!", 5000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name)); //test
                    RunningGangwar.FactionPoints += 1;
                }
                else if (RunningGangwar.Flag1.Faction == RunningGangwar.Attacker.Id)
                {
                    RunningGangwar.Attacker.GetFactionPlayers().ForEach((DbPlayer dbPlayer) => dbPlayer.SendNotification("+1 Punkte für das Besetzen einer Flagge!", 5000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name)); //test

                    RunningGangwar.AttackerPoints += 1;
                }
                if (RunningGangwar.Flag2.Faction == RunningGangwar.Faction.Id)
                {
                    RunningGangwar.Faction.GetFactionPlayers().ForEach((DbPlayer dbPlayer) => dbPlayer.SendNotification("+1 Punkte für das Besetzen einer Flagge!", 5000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name)); //test

                    RunningGangwar.FactionPoints += 1;
                }
                else if (RunningGangwar.Flag2.Faction == RunningGangwar.Attacker.Id)
                {
                    RunningGangwar.Attacker.GetFactionPlayers().ForEach((DbPlayer dbPlayer) => dbPlayer.SendNotification("+1 Punkte für das Besetzen einer Flagge!", 5000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name)); //test

                    RunningGangwar.AttackerPoints += 1;
                }
                if (RunningGangwar.Flag3.Faction == RunningGangwar.Faction.Id)
                {
                    RunningGangwar.Faction.GetFactionPlayers().ForEach((DbPlayer dbPlayer) => dbPlayer.SendNotification("+1 Punkte für das Besetzen einer Flagge!", 5000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name)); //test

                    RunningGangwar.FactionPoints += 1;
                }
                else if (RunningGangwar.Flag3.Faction == RunningGangwar.Attacker.Id)
                {
                    RunningGangwar.Attacker.GetFactionPlayers().ForEach((DbPlayer dbPlayer) => dbPlayer.SendNotification("+1 Punkte für das Besetzen einer Flagge!", 5000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name)); //test

                    RunningGangwar.AttackerPoints += 1;
                }
                if (RunningGangwar.Flag4.Faction == RunningGangwar.Faction.Id)
                {
                    RunningGangwar.Faction.GetFactionPlayers().ForEach((DbPlayer dbPlayer) => dbPlayer.SendNotification("+1 Punkte für das Besetzen einer Flagge!", 5000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name)); //test

                    RunningGangwar.FactionPoints += 1;
                }
                else if (RunningGangwar.Flag4.Faction == RunningGangwar.Attacker.Id)
                {
                    RunningGangwar.Attacker.GetFactionPlayers().ForEach((DbPlayer dbPlayer) => dbPlayer.SendNotification("+1 Punkte für das Besetzen einer Flagge!", 5000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name)); //test

                    RunningGangwar.AttackerPoints += 1;
                }

                RunningGangwar.Faction.GetFactionPlayers().ForEach(e =>
                {
                    if (e.player.Position.DistanceTo(RunningGangwar.Zone) < 300)
                        e.player.TriggerEvent("updateGangwarScore", RunningGangwar.FactionPoints, RunningGangwar.AttackerPoints);
                });

                RunningGangwar.Attacker.GetFactionPlayers().ForEach(e =>
                {
                    if (e.player.Position.DistanceTo(RunningGangwar.Zone) < 300)
                        e.player.TriggerEvent("updateGangwarScore", RunningGangwar.FactionPoints, RunningGangwar.AttackerPoints);
                });
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION AddGWPoints] " + ex.Message);
                Logger.Print("[EXCEPTION AddGWPoints] " + ex.StackTrace);
            }
        }

        [RemoteEvent("client:selectKit:chooseGW")]
        public void chooseGW(Player player, int GWid)
        {
            DbPlayer dbPlayer = player.GetPlayer();
            if (GWid > 4)
            {
                dbPlayer.SendNotification("Du kannst nur Loadout 1 - 4 wählen!", 3000, "grey");
                return;
            }
            //Advancedrifle
            if (GWid == 1)
            {
                dbPlayer.player.RemoveAllWeapons();
                dbPlayer.GiveWeapon(WeaponHash.Advancedrifle, 9999);
                dbPlayer.GiveWeapon(WeaponHash.Heavypistol, 9999);
                dbPlayer.GiveWeapon(WeaponHash.Battleaxe, 9999);
                dbPlayer.SendNotification("Advancedrifle + Heavypistol + Battleaxe ", 3000, "darkgrey", "GW WAFFENKIT 1");
            }

            //Carbine
            if (GWid == 2)
            {
                dbPlayer.player.RemoveAllWeapons();
                dbPlayer.GiveWeapon(WeaponHash.Carbinerifle, 9999);
                dbPlayer.GiveWeapon(WeaponHash.Heavypistol, 9999);
                dbPlayer.GiveWeapon(WeaponHash.Battleaxe, 9999);
                dbPlayer.SendNotification("Carbinerifle + Heavypistol + Battleaxe ", 3000, "darkgrey", "GW WAFFENKIT 2");
            }

            //Gusenberg
            if (GWid == 3)
            {
                dbPlayer.player.RemoveAllWeapons();
                dbPlayer.GiveWeapon(WeaponHash.Gusenberg, 9999);
                dbPlayer.GiveWeapon(WeaponHash.Heavypistol, 9999);
                dbPlayer.GiveWeapon(WeaponHash.Battleaxe, 9999);
                dbPlayer.SendNotification("Gusenberg + Heavypistol + Battleaxe ", 3000, "darkgrey", "GW WAFFENKIT 3");
            }

            //Bullpup
            if (GWid == 4)
            {
                dbPlayer.player.RemoveAllWeapons();
                dbPlayer.GiveWeapon(WeaponHash.Bullpuprifle, 9999);
                dbPlayer.GiveWeapon(WeaponHash.Heavypistol, 9999);
                dbPlayer.GiveWeapon(WeaponHash.Battleaxe, 9999);
                dbPlayer.SendNotification("Bullpuprifle + Heavypistol + Battleaxe ", 3000, "darkgrey", "GW WAFFENKIT 4");
            }
        }
        public static void EndGangwar()
        {
            try
            {
                MySqlQuery mySqlQuery = new MySqlQuery("UPDATE gangwars SET Faction = @faction WHERE Id = @id");
                mySqlQuery.AddParameter("@id", RunningGangwar.Id);
                if (RunningGangwar.AttackerPoints > RunningGangwar.FactionPoints)
                {
                    RunningGangwar.Faction = RunningGangwar.Attacker;
                    mySqlQuery.AddParameter("@faction", RunningGangwar.Attacker.Id);
                    Notification.SendGlobalNotification(
                        $"Die Fraktion {RunningGangwar.Attacker.Name} hat den Kampf um das Gebiet {RunningGangwar.Name} gegen {RunningGangwar.Faction.Name} gewonnen.",
                        8000, "orange", Notification.icon.warn);
                    WebhookSender.SendMessage("Gangwar", $"Die Fraktion {RunningGangwar.Attacker.Name} hat den Kampf um das Gebiet {RunningGangwar.Name} gegen {RunningGangwar.Faction.Name} gewonnen.", Webhooks.gangwar, "Gangwar");

                }
                else
                {
                    mySqlQuery.AddParameter("@faction", RunningGangwar.Faction.Id);
                    Notification.SendGlobalNotification(
                        $"Die Fraktion {RunningGangwar.Faction.Name} hat den Kampf um das Gebiet {RunningGangwar.Name} gegen {RunningGangwar.Attacker.Name} verteidigt.",
                        8000, "orange", Notification.icon.warn);
                    WebhookSender.SendMessage("Gangwar", $"Die Fraktion {RunningGangwar.Faction.Name} hat den Kampf um das Gebiet {RunningGangwar.Name} gegen {RunningGangwar.Attacker.Name} verteidigt.", Webhooks.gangwar, "Gangwar");
                }


                MySqlHandler.ExecuteSync(mySqlQuery);
                RunningGangwar.Flag1.Faction = 0;
                RunningGangwar.Flag2.Faction = 0;
                RunningGangwar.Flag3.Faction = 0;
                RunningGangwar.Flag4.Faction = 0;

                RunningGangwar = null;

                foreach (DbPlayer player in PlayerHandler.GetPlayers())
                {
                    if (player.Dimension == FactionModule.GangwarDimension)
                    {
                        player.DeathData = new DeathData
                        {
                            IsDead = false,
                            DeathTime = new DateTime(0)
                        };
                        player.disableAllPlayerActions(false);
                        player.StopAnimation();
                        player.RemoveAllWeapons();
                        player.StopScreenEffect("DeathFailOut");
                        player.SetAttribute("Death", 0);
                        player.SetInvincible(false);
                        player.SetArmor(0);
                        player.TriggerEvent("finishGangwar");
                        player.SpawnPlayer(player.Faction.Spawn);
                        player.SetDimension(0);
                        WeaponManager.loadWeapons(player.player);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION EndGangwar] " + ex.Message);
                Logger.Print("[EXCEPTION EndGangwar] " + ex.StackTrace);
            }
        }
    }
}
