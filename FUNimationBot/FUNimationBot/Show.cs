using Newtonsoft.Json;
using System.ComponentModel;
using System.Net;

namespace FUNimationBot
{
    public class Show
    {
        private Episodes Episodes;
        private int Id;
        private string Source, InternalTitle, Title, Wildcard;
        private decimal InternalOffset, AKAOffset;

        public Show(int id, string source, string internalTitle, string title, decimal internalOffset, decimal AKAOffset, string wildcard)
        {
            Id = id;
            Source = source;
            InternalTitle = internalTitle;
            Title = title;
            InternalOffset = internalOffset;
            this.AKAOffset = AKAOffset;
            // The Wildcard for the FUNimationBot is the base URL for streaming links.
            Wildcard = wildcard;
        }

        public void GetShowDataAndPost(object o, DoWorkEventArgs args)
        {
            string JSON = string.Empty;
            using (WebClient WebClient = new WebClient())
            {
                WebClient.Headers.Add("Cache-Control", "no-cache");
                if (MainLogic.WebProxy != null)
                    WebClient.Proxy = MainLogic.WebProxy;

                try
                {
                    JSON = WebClient.DownloadString("https://www.funimation.com/feeds/ps/videos?ut=FunimationSubscriptionUser&show_id="
                    + Source + "&limit=10000");
                }
                catch (WebException)
                {
                    args.Result = "Failed connect for " + Title;
                    return;
                }

                if (JSON == string.Empty)
                    return;
            }

            try
            {
                Episodes = JsonConvert.DeserializeObject<Episodes>(JSON);
            }
            catch (JsonException)
            {
                args.Result = "Failed JSON deserialization for " + Title;
                return;
            }

            foreach (Episode Episode in Episodes.EpisodesList)
            {

            }
        }

        public override string ToString()
        {
            return Title;
        }

        public override bool Equals(object obj)
        {
            var Show = obj as Show;

            if (Show == null)
            {
                return false;
            }

            return Id == Show.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}