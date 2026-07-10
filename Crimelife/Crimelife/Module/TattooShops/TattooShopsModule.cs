using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Crimelife.Module;

namespace Crimelife
{
    class TattooShopsModule : Module<TattooShopsModule>
    {
        public static List<TattooShop> tattooshopList = new List<TattooShop>();
        public static List<Tattoos> tattoolist = new List<Tattoos>();


        protected override bool OnLoad()
        {
            MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM tattooshops");
            MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery);
            try
            {
                MySqlDataReader reader = mySqlResult.Reader;
                try
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string Name = reader.GetString("Name");
                            Vector3 Position = NAPI.Util.FromJson<Vector3>(reader.GetString("Position"));
                            tattooshopList.Add(new TattooShop
                            {
                                Id = reader.GetInt32("Id"),
                                Name = Name,
                                Position = Position
                            });

                            ColShape val = NAPI.ColShape.CreateCylinderColShape(Position, 1.4f, 2.4f, 0);
                            val.SetData("FUNCTION_MODEL", new FunctionModel("openTattooShop", Name));
                            val.SetData("MESSAGE", new Message("Benutze E um den TattooShop zu öffnen.", Name, "orange", 3000));
                            val.SetData("TattooShops", Name);

                            NAPI.Blip.CreateBlip(75, Position, 1f, 0, Name, 255, 0.0f, true, 0, 0);
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
                Logger.Print("[EXCEPTION loadClothingShops] " + ex.Message);
                Logger.Print("[EXCEPTION loadClothingShops] " + ex.StackTrace);
            }
            finally
            {
                mySqlResult.Connection.Dispose();
            }

            loadtattoosserver();
            return true;

        }

        public static List<TattooList> getTattoos(Player c)
        {
            if (c == null) return new List<TattooList>();
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return new List<TattooList>();

            List<TattooList> tattoos = new List<TattooList>();
            MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Id = @userid LIMIT 1");
            mySqlQuery.Parameters = new List<MySqlParameter>()
            {
                new MySqlParameter("@userid", dbPlayer.Id)
            };
            MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
            try
            {
                MySqlDataReader reader = mySqlReaderCon.Reader;
                try
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            tattoos = NAPI.Util.FromJson<List<TattooList>>(reader.GetString("Tattoo"));
                        }
                    }
                    else
                    {
                        mySqlQuery.Query = "INSERT INTO phone_contacts (Id) VALUES (@userid)";
                        mySqlQuery.Parameters = new List<MySqlParameter>()
                        {
                            new MySqlParameter("@userid", dbPlayer.Id)
                        };
                        MySqlHandler.ExecuteSync(mySqlQuery);
                    }
                }
                finally
                {
                    reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION getTattooList] " + ex.Message);
                Logger.Print("[EXCEPTION getTattooList] " + ex.StackTrace);
            }
            finally
            {
                mySqlReaderCon.Connection.Dispose();
            }

            return tattoos;
        }

        [ServerEvent(Event.PlayerEnterColshape)]
        public void onEnterColshape(ColShape val, Player player)
        {
            if (player == null || val == null) return;
            DbPlayer dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;

            try
            {
                if (val.HasData("TattooShops"))
                {
                    player.SetAccessories(0, -1, 0);
                    dbPlayer.SetClothes(11, 15, 0);
                    dbPlayer.SetClothes(3, 15, 0);
                    dbPlayer.SetClothes(8, 15, 0);
                    dbPlayer.SetClothes(4, 21, 0);
                    dbPlayer.SetClothes(6, 34, 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION enterColshape] " + ex.Message);
                Logger.Print("[EXCEPTION enterColshape] " + ex.StackTrace);
            }
        }

        [ServerEvent(Event.PlayerExitColshape)]
        public void onExitColshape(ColShape val, Player player)
        {
            if (player == null || val == null) return;
            DbPlayer dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;

            try
            {
                if (val.HasData("TattooShops"))
                {
                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Username = @user LIMIT 1");
                    mySqlQuery.AddParameter("@user", player.Name);

                    MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                    MySqlDataReader reader = mySqlReaderCon.Reader;
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            PlayerClothes playerClothes = NAPI.Util.FromJson<PlayerClothes>(reader.GetString("Clothes"));
                            player.SetAccessories(0, playerClothes.Hut.drawable, playerClothes.Hut.texture);
                            player.SetAccessories(1, playerClothes.Brille.drawable, playerClothes.Brille.texture);
                            dbPlayer.SetClothes(11, playerClothes.Oberteil.drawable, playerClothes.Oberteil.texture);
                            dbPlayer.SetClothes(8, playerClothes.Unterteil.drawable, playerClothes.Unterteil.texture);
                            dbPlayer.SetClothes(7, playerClothes.Kette.drawable, playerClothes.Kette.texture);
                            dbPlayer.SetClothes(3, playerClothes.Koerper.drawable, playerClothes.Koerper.texture);
                            dbPlayer.SetClothes(4, playerClothes.Hose.drawable, playerClothes.Hose.texture);
                            dbPlayer.SetClothes(6, playerClothes.Schuhe.drawable, playerClothes.Schuhe.texture);
                            List<Decoration> tattoos = NAPI.Util.FromJson<List<Decoration>>(dbPlayer.GetAttributeString("Tattoo"));
                            foreach (Decoration tattooid in tattoos)
                            {
                                player.SetDecoration(tattooid);
                            }
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION enterColshape] " + ex.Message);
                Logger.Print("[EXCEPTION enterColshape] " + ex.StackTrace);
            }
        }

        [RemoteEvent("syncTattoo")]
        public static void syncTattoo(Player client, string tattoohash)
        {
            try
            {
                Tattoos tattoo = tattoolist.FirstOrDefault(x => x.TattooHash == tattoohash);
                Decoration data = new Decoration();
                data.Collection = NAPI.Util.GetHashKey(tattoo.TattooCollection);
                data.Overlay = NAPI.Util.GetHashKey(tattoo.TattooHash);
                client.SetDecoration(data);
            }
            catch (Exception e)
            {

            }
        }

        [RemoteEvent("tattooShopBuy")]
        public static void tattooShopBuy(Player client, string tattoohash)
        {
            try
            {
                DbPlayer dbPlayer = client.GetPlayer();
                Tattoos tattoo = tattoolist.FirstOrDefault(x => x.TattooHash == tattoohash);
                Decoration data = new Decoration();
                MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Tattoo = @userId LIMIT 1");
                mySqlQuery.AddParameter("@userId", dbPlayer.Id);

                MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery);
                if (dbPlayer.Money >= tattoo.Price)
                {
                    List<TattooList> tattoos = getTattoos(client);
                    dbPlayer.removeMoney(Convert.ToInt32(tattoo.Price));
                    data.Collection = NAPI.Util.GetHashKey(tattoo.TattooCollection);
                    data.Overlay = NAPI.Util.GetHashKey(tattoo.TattooHash);
                    tattoos.Add(new TattooList
                    {
                        collection = data.Collection,
                        overlay = data.Overlay
                    });
                    var tattoogive = NAPI.Util.ToJson(tattoos);
                    dbPlayer.SetAttribute("Tattoo", tattoogive);
                    dbPlayer.SendNotification("Du hast dir erfolgreich ein Tattoo stechen lassen.", 4000, "gray");
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION openBuyTattoo] " + ex.Message);
                Logger.Print("[EXCEPTION openBuyTattoo] " + ex.StackTrace);
            }
        }
        public static void loadtattoosserver()
        {
            MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM tattoos");
            MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery);
            try
            {
                MySqlDataReader reader = mySqlResult.Reader;
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tattoolist.Add(new Tattoos(
                            reader.GetString("Name"),
                            reader.GetInt32("Price"),
                            reader.GetInt32("ZoneId"),
                            reader.GetString("TattooHash"),
                            reader.GetString("Collection"))
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION loadtattos] " + ex.Message);
                Logger.Print("[EXCEPTION loadtattos] " + ex.StackTrace);
            }
            finally
            {
                mySqlResult.Connection.Dispose();
            }
        }

        [RemoteEvent("openTattooShop")]
        public static void openTattooShop(Player client)
        {
            try
            {
                if (client == null) return;
                DbPlayer dbPlayer = client.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                    return;

                var tattoojson = NAPI.Util.ToJson(tattoolist.Where(x => x.ZoneId == 0));
                client.TriggerEvent("openWindow", new object[]
                {
                    "TattooShop","{\"tattoos\":"+tattoojson+"}"
                });
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION openTattooShop] " + ex.Message);
                Logger.Print("[EXCEPTION openTattooShop] " + ex.StackTrace);
            }
        }

        [RemoteEvent("requestTattooShopCategoryTattoos")]
        public static void Tattooladen(Player client, int idx)
        {
            Logger.Print(idx.ToString());
            if (client == null) return;
            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.player == null)
                return;
            if (idx == null) return;
            try
            {
                var tattoojson = NAPI.Util.ToJson(tattoolist.Where(x => x.ZoneId == idx));
                client.TriggerEvent("componentServerEvent", "TattooShop", "responseTattooShopCategory", tattoojson);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION nM-Kleiderladen] " + ex.Message);
                Logger.Print("[EXCEPTION nM-Kleiderladen] " + ex.StackTrace);
            }
        }

        public class Tattoos
        {
            public string Name { get; set; }
            public int Price { get; set; }
            public int ZoneId { get; set; }
            public string TattooHash { get; set; }

            public string TattooCollection { get; set; }

            public Tattoos(string name, int price, int zoneId, string tattooHash, string tattooCollection)
            {
                this.Name = name;
                this.Price = price;
                this.ZoneId = zoneId;
                this.TattooHash = tattooHash;
                this.TattooCollection = tattooCollection;
            }
        }
    }
}
