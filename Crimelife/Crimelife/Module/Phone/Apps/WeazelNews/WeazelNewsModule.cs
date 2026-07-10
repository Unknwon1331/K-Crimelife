using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Crimelife.Module;

namespace Crimelife
{
    class NewsListApp : Module<NewsListApp>
    {

        public static List<NewsListApp.NewsFound> newsList = new List<NewsListApp.NewsFound>();
        public class NewsFound
        {
            [JsonProperty(PropertyName = "id")] public uint Id { get; }
            [JsonProperty(PropertyName = "title")] public string Title { get; }
            [JsonProperty(PropertyName = "content")] public string Content { get; }
            [JsonProperty(PropertyName = "typeId")] public int TypeId { get; }

            public NewsFound(uint id, string title, string content, int typeId)
            {
                Id = id;
                Title = title;
                Content = content;
                TypeId = typeId;
            }
        }

        [RemoteEvent("requestNews")]
        public void requestNews(Player player)
        {
            SendNewsList(player);
        }

        [RemoteEvent("addNews")]
        public void addNews(Player player, int newsType, string title, string content)
        {
            DbPlayer player2 = player.GetPlayer();
            Adminrank adminranks = player2.Adminrank;
            if (player2 != null && player2.IsValid() && adminranks.Permission >= 91)
            {
                title = title.Replace("\"", "");
                content = content.Replace("\"", "");
                title = title.Replace("\"\"", "");
                content = content.Replace("\"\"", "");
                uint id = (uint)(newsList.Count + 1);
                newsList.Add(new NewsFound(id, title + " (" + id + ")", content, newsType));
                newsList.Sort((NewsFound x, NewsFound y) => y.Id.CompareTo(x.Id));
                SendNewsList(player);
                if (newsType == 0)
                {
                    player2.SendNotification("[NEWS] Es wurde ein Wetterbericht veroeffentlicht. Check die News App!", 3000, "red", "WEAZEL NEWS");
                }
                else
                {
                    player2.SendNotification("[NEWS] Es wurde eine News veroeffentlicht. Check die News App!", 3000, "red", "WEAZEL NEWS");
                }
            }
        }


        private void SendNewsList(Player player)
        {
            DbPlayer playertest = player.GetPlayer();
            player.TriggerEvent("updateNews", NAPI.Util.ToJson((object)newsList));
            playertest.SendNotification("[TEST] Dieser Test ist erfolgreich!", 3000, "red", "ADMIN DIENST");
        }


        public void deleteNews(uint p_NewsID)
        {
            if (newsList.Exists((NewsFound n) => n.Id == p_NewsID))
            {
                NewsFound item = newsList.Find((NewsFound n) => n.Id == p_NewsID);
                newsList.Remove(item);
            }
        }

        [RemoteEvent("removeNews")]
        public void removeNews(Player player, uint p_NewsID)
        {
            deleteNews(p_NewsID);
        }
    }
}
