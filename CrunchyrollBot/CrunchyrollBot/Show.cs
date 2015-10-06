using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace CrunchyrollBot
{
    public struct Information
    {
        public string Website;
        public string Title;
        public string BaseURL;
    };

    public class Show
    {
        private const string BASE_URL = "https://crunchyroll.com/";
        private bool CrunchyrollIsClip, SourceExists;
        private int Id, CrunchyrollDuration;
        private string Source, InternalTitle, Title, CrunchyrollURL, CrunchyrollSeriesTitle, CrunchyrollEpisodeTitle, CrunchyrollKeywords,
            ShowType, DisplayedTitle, PostURL;
        private decimal InternalOffset, AKAOffset, CrunchyrollEpisodeNumber, EpisodeCount;
        private List<Information> Informations, Streamings;
        private List<string> Subreddits, Keywords;

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
            catch
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

                    // Skip this episode if it is a preview clip or if the result is not for the current show
                    if (CrunchyrollIsClip || CrunchyrollSeriesTitle != InternalTitle)
                        continue;

                    GetDatabaseData();

                    if (!ApplyOffsetAndCheckValidity())
                        continue;

                    if (IsNewEpisode())
                    {
                        // How does posting to reddit work?:
                        // 1 - Insert the episode into the database without PostURL so that other bots won't post in the meantime
                        // 2 - Post to reddit
                        // 3 - If the post failed remove the entry from the database. If it succeeded update PostURL with the URL
                        try
                        {
                            SQLiteCommand InsertEpisodeCommand = new SQLiteCommand(@"
                                INSERT INTO Episodes VALUES (@Id, @EpisodeNumber, '')
                                ", MainLogic.CurrentDB);
                            InsertEpisodeCommand.Parameters.AddWithValue("@Id", Id);
                            InsertEpisodeCommand.Parameters.AddWithValue("@EpisodeNumber", CrunchyrollEpisodeNumber);
                            InsertEpisodeCommand.ExecuteNonQuery();
                        }
                        catch
                        {
                            MainLogic.MainForm.Invoke(new MethodInvoker(delegate ()
                            {
                                MainLogic.MainForm.ErrorListBox.Items.Insert(0, (DateTime.Now.ToString("HH:mm:ss: ") + 
                                    "Failed insert in database for " + Title + " episode " + CrunchyrollEpisodeNumber));
                            }));
                            continue;
                        }

                        if (PostOnReddit())
                        {
                            try
                            {
                                SQLiteCommand UpdateEpisodeCommand = new SQLiteCommand(@"
                                UPDATE Episodes
                                SET PostURL = @PostURL
                                WHERE Id = @Id AND EpisodeNumber = @EpisodeNumber
                                ", MainLogic.CurrentDB);
                                UpdateEpisodeCommand.Parameters.AddWithValue("@PostURL", PostURL);
                                UpdateEpisodeCommand.Parameters.AddWithValue("@Id", Id);
                                UpdateEpisodeCommand.Parameters.AddWithValue("@EpisodeNumber", CrunchyrollEpisodeNumber);
                                UpdateEpisodeCommand.ExecuteNonQuery();
                            }
                            catch
                            {
                                MainLogic.MainForm.Invoke(new MethodInvoker(delegate ()
                                {
                                    MainLogic.MainForm.ErrorListBox.Items.Insert(0, (DateTime.Now.ToString("HH:mm:ss: ") +
                                        "!ALERT! Failed update in database for " + Title + " episode " + CrunchyrollEpisodeNumber));
                                }));
                                continue;
                            }
                        }
                        else
                        {
                            try
                            {
                                SQLiteCommand DeleteEpisodeCommand = new SQLiteCommand(@"
                                DELETE FROM Episodes
                                WHERE Id = @Id AND EpisodeNumber = @EpisodeNumber
                                ", MainLogic.CurrentDB);
                                DeleteEpisodeCommand.Parameters.AddWithValue("@Id", Id);
                                DeleteEpisodeCommand.Parameters.AddWithValue("@EpisodeNumber", CrunchyrollEpisodeNumber);
                                DeleteEpisodeCommand.ExecuteNonQuery();
                            }
                            catch
                            {
                                MainLogic.MainForm.Invoke(new MethodInvoker(delegate ()
                                {
                                    MainLogic.MainForm.ErrorListBox.Items.Insert(0, (DateTime.Now.ToString("HH:mm:ss: ") +
                                        "!ALERT! Failed delete in database for " + Title + " episode " + CrunchyrollEpisodeNumber));
                                }));
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        private bool PostOnReddit()
        {
            PostURL = "test";
            return true;
        }

        private bool IsNewEpisode()
        {
            // Count the amount of rows that has the same episode number. If it is 0 the current episode
            // is a new entry. (Is there a better way of doing this?)
            SQLiteCommand CountEpisodeCommand = new SQLiteCommand(@"
                    SELECT COUNT(*)
                    FROM Episodes WHERE Id = @Id AND EpisodeNumber = @EpisodeNumber
                    ", MainLogic.CurrentDB);
            CountEpisodeCommand.Parameters.AddWithValue("@Id", Id);
            CountEpisodeCommand.Parameters.AddWithValue("@EpisodeNumber", CrunchyrollEpisodeNumber);
            int EpisodeCount = Convert.ToInt32(CountEpisodeCommand.ExecuteScalar());

            return EpisodeCount == 0;
        }

        private bool ApplyOffsetAndCheckValidity()
        {
            // There is no offset so it does not have to be applied
            if (InternalOffset == 0)
            {
                return true;
            }
            else
            {
                CrunchyrollEpisodeNumber += InternalOffset;
                // The episode was actually an episode from a different season so it needs to be skipped
                return CrunchyrollEpisodeNumber > 0;
            }
        }

        private void GetDatabaseData()
        {
            // Get data from Information table
            SQLiteCommand SelectInformationCommand = new SQLiteCommand(@"
                    SELECT Website, Title, BaseURL
                    FROM Information WHERE Id = @Id
                    ORDER BY Website ASC
                    ", MainLogic.CurrentDB);
            SelectInformationCommand.Parameters.AddWithValue("@Id", Id);
            SQLiteDataReader SelectInformation = SelectInformationCommand.ExecuteReader();

            Informations = new List<Information>();
            while (SelectInformation.Read())
            {
                Information Information;
                Information.Website = SelectInformation[0].ToString();
                Information.Title = SelectInformation[1].ToString();
                Information.BaseURL = SelectInformation[2].ToString();
                Informations.Add(Information);
            }

            // Get data from Streaming table
            // This query could be unioned with the previous one but when posting
            // to reddit there is whitespace in between so we need to be able to distinguish
            SQLiteCommand SelectStreamingCommand = new SQLiteCommand(@"
                    SELECT Website, Title, BaseURL
                    FROM Streaming WHERE Id = @Id AND Website != 'Crunchyroll'
                    ORDER BY Website ASC
                    ", MainLogic.CurrentDB);
            SelectStreamingCommand.Parameters.AddWithValue("@Id", Id);
            SQLiteDataReader SelectStreaming = SelectStreamingCommand.ExecuteReader();

            Streamings = new List<Information>();
            while (SelectStreaming.Read())
            {
                Information Streaming;
                Streaming.Website = SelectStreaming[0].ToString();
                Streaming.Title = SelectStreaming[1].ToString();
                Streaming.BaseURL = SelectStreaming[2].ToString();
                Streamings.Add(Streaming);
            }

            // Get data from Subreddits table
            SQLiteCommand SelectSubredditsCommand = new SQLiteCommand(@"
                    SELECT Subreddit
                    FROM Subreddits WHERE Id = @Id
                    ORDER BY Subreddit ASC
                    ", MainLogic.CurrentDB);
            SelectSubredditsCommand.Parameters.AddWithValue("@Id", Id);
            SQLiteDataReader SelectSubreddits = SelectSubredditsCommand.ExecuteReader();

            Subreddits = new List<string>();
            while (SelectSubreddits.Read())
                Subreddits.Add(SelectSubreddits[0].ToString());

            // Get data from Keywords table
            SQLiteCommand SelectKeywordsCommand = new SQLiteCommand(@"
                    SELECT Keyword
                    FROM Keywords WHERE Id = @Id
                    ", MainLogic.CurrentDB);
            SelectKeywordsCommand.Parameters.AddWithValue("@Id", Id);
            SQLiteDataReader SelectKeywords = SelectKeywordsCommand.ExecuteReader();

            Keywords = new List<string>();
            while (SelectKeywords.Read())
                Keywords.Add(SelectKeywords[0].ToString());

            // Get data from Shows table
            SQLiteCommand SelectShowsCommand = new SQLiteCommand(@"
                    SELECT EpisodeCount, ShowType, DisplayedTitle, SourceExists
                    FROM Shows WHERE Id = @Id
                    ", MainLogic.CurrentDB);
            SelectShowsCommand.Parameters.AddWithValue("@Id", Id);
            SQLiteDataReader SelectShows = SelectShowsCommand.ExecuteReader();

            if (SelectShows.Read())
            {
                EpisodeCount = decimal.Parse(SelectShows[0].ToString());
                ShowType = SelectShows[1].ToString();
                DisplayedTitle = SelectShows[2].ToString();
                SourceExists = bool.Parse(SelectShows[3].ToString());
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
            CrunchyrollKeywords = XElement.Element(Media + "keywords") != null ?
                XElement.Element(Media + "keywords").Value : string.Empty;

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
