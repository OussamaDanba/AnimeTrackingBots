using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace CrunchyrollBot
{
    public class Show
    {
        private const string BASE_URL = "https://crunchyroll.com/";
        private bool CrunchyrollIsClip;
        private int Id, CrunchyrollDuration;
        private string Source, InternalTitle, Title, CrunchyrollURL, CrunchyrollSeriesTitle, CrunchyrollEpisodeTitle, CrunchyrollKeywords;
        private decimal InternalOffset, AKAOffset, CrunchyrollEpisodeNumber;

        public Show(int id, string source, string internalTitle, string title, decimal internalOffset, decimal AKAOffset)
        {
            Id = id;
            Source = source;
            InternalTitle = internalTitle;
            Title = title;
            InternalOffset = internalOffset;
            this.AKAOffset = AKAOffset;
        }

        public void GetShowDataAndPost(object o, DoWorkEventArgs args)
        {
            XmlDocument Feed = new XmlDocument();
            try
            {
                Feed.Load(BASE_URL + Source);
            }
            catch (System.Net.WebException)
            {
                args.Result = "Failed connect for " + Title;
                return;
            }
            
            XElement RSS = XElement.Parse(GetRawXml(Feed));
            IEnumerable<XElement> Episodes = RSS.Element("channel").Elements("item");
            foreach (XElement XElement in Episodes.Reverse())
            {
                if (DateTime.Now >= DateTime.Parse(XElement.Element("pubDate").Value).ToLocalTime())
                {
                    ParseCrunchyrollData(XElement);

                    // Early return since the episode is a preview clip
                    if (CrunchyrollIsClip)
                        return;
                }
                else
                {
                    return;
                }
            }
        }

        private void ParseCrunchyrollData(XElement XElement)
        {
            XNamespace Media = "http://search.yahoo.com/mrss/";
            XNamespace Crunchyroll = "http://www.crunchyroll.com/rss";

            // If an element does not exist it will be null
            CrunchyrollIsClip = XElement.Element(Crunchyroll + "isClip") == null ? false : true;
            CrunchyrollURL = BASE_URL + "media-" + XElement.Element(Crunchyroll + "mediaId").Value;
            CrunchyrollDuration = int.Parse(XElement.Element(Crunchyroll + "duration").Value);
            CrunchyrollEpisodeTitle = XElement.Element(Crunchyroll + "episodeTitle").Value;
            CrunchyrollSeriesTitle = XElement.Element(Crunchyroll + "seriesTitle").Value;
            CrunchyrollKeywords = XElement.Element(Media + "keywords").Value;

            // The episodeNumber does not exist in two cases which we need to account for.
            // 1: The episode is episode 0. (Usually prologue)
            // 2: The episode is actually a preview clip. (In this case we don't need to set a value
            //    since the thread will return early.)
            if (XElement.Element(Crunchyroll + "episodeNumber") == null)
                CrunchyrollEpisodeNumber = 0;
            else
                CrunchyrollEpisodeNumber = decimal.Parse(XElement.Element(Crunchyroll + "episodeNumber").Value);
        }

        private string GetRawXml(XmlDocument xmlDocument)
        {
            using (StringWriter StringWriter = new StringWriter())
            using (XmlWriter XmlWriter = XmlWriter.Create(StringWriter))
            {
                xmlDocument.WriteTo(XmlWriter);
                XmlWriter.Flush();
                return StringWriter.GetStringBuilder().ToString();
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
