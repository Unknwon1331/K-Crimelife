using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace Crimelife
{
    public class TattooModel
    {

        public string name
        {
            get;
            set;
        }

        public string category
        {
            get;
            set;
        }


        public string collection
        {
            get;
            set;
        }

        public string overlay
        {
            get;
            set;
        }

        public static void setTattoos(Player c, TattooModel tattoo)
        {
            try
            {
                MySqlQuery mySqlQuery = new MySqlQuery("UPDATE accounts SET Tattoo = @val WHERE Username = @username");
                mySqlQuery.AddParameter("@username", c.Name);
                mySqlQuery.AddParameter("@val", NAPI.Util.ToJson(tattoo));
                MySqlHandler.ExecuteSync(mySqlQuery);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION setClothes] " + ex.Message);
                Logger.Print("[EXCEPTION setClothes] " + ex.StackTrace);
            }
        }


        public TattooModel(string name, string category, string collection, string overlay)
        {
            this.name = name;
            this.category = category;
            this.collection = collection;
            this.overlay = overlay;
        }

    }
}
