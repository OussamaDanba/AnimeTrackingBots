using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace CrunchyrollBot
{
    public struct Information
    {
        public string Website;
        public string Title;
        public string TitleURL;
    };

    public class Show
    {
        private const string BASE_URL = "https://crunchyroll.com/";
        private bool CrunchyrollIsClip, SourceExists;
        private int Id, CrunchyrollDuration;
        private string Source, InternalTitle, Title, CrunchyrollURL, CrunchyrollSeriesTitle, CrunchyrollEpisodeTitle, CrunchyrollKeywords,
            ShowType, DisplayedTitle, PostURL;
        private decimal InternalOffset, AKAOffset, CrunchyrollEpisodeNumber;
        private decimal? EpisodeCount;
        private List<Information> Informations, Streamings;
        private List<string> Subreddits, Keywords;
        private const int AmountOfColumns = 3, AmountOfRows = 13, EntriesPerTable = AmountOfColumns * AmountOfRows;

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

            GetDatabaseData();

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
                            string InsertEpisodeQuery = @"
                                INSERT INTO Episodes VALUES (@Id, @EpisodeNumber, '')";
                            using (SQLiteCommand InsertEpisodeCommand = new SQLiteCommand(InsertEpisodeQuery, MainLogic.CurrentDB))
                            {
                                InsertEpisodeCommand.Parameters.AddWithValue("@Id", Id);
                                InsertEpisodeCommand.Parameters.AddWithValue("@EpisodeNumber", CrunchyrollEpisodeNumber);
                                InsertEpisodeCommand.ExecuteNonQuery();
                            }
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
                                string UpdateEpisodeQuery = @"
                                    UPDATE Episodes
                                    SET PostURL = @PostURL
                                    WHERE Id = @Id AND EpisodeNumber = @EpisodeNumber";
                                using (SQLiteCommand UpdateEpisodeCommand = new SQLiteCommand(UpdateEpisodeQuery, MainLogic.CurrentDB))
                                {
                                    UpdateEpisodeCommand.Parameters.AddWithValue("@PostURL", PostURL);
                                    UpdateEpisodeCommand.Parameters.AddWithValue("@Id", Id);
                                    UpdateEpisodeCommand.Parameters.AddWithValue("@EpisodeNumber", CrunchyrollEpisodeNumber);
                                    UpdateEpisodeCommand.ExecuteNonQuery();

                                    MainLogic.MainForm.Invoke(new MethodInvoker(delegate ()
                                    {
                                        MainLogic.MainForm.RecentListBox.Items.Insert(0, (DateTime.Now.ToString("HH:mm:ss: ") +
                                            "Successful post for " + Title + " episode " + CrunchyrollEpisodeNumber + " (" + PostURL + ')'));
                                    }));
                                }
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
                                string DeleteEpisodeQuery = @"
                                    DELETE FROM Episodes
                                    WHERE Id = @Id AND EpisodeNumber = @EpisodeNumber";
                                using (SQLiteCommand DeleteEpisodeCommand = new SQLiteCommand(DeleteEpisodeQuery, MainLogic.CurrentDB))
                                {
                                    DeleteEpisodeCommand.Parameters.AddWithValue("@Id", Id);
                                    DeleteEpisodeCommand.Parameters.AddWithValue("@EpisodeNumber", CrunchyrollEpisodeNumber);
                                    DeleteEpisodeCommand.ExecuteNonQuery();
                                }
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
            try
            {
                Tuple<string, string> PostTuple = GeneratePost();
                RedditSharp.Things.Post Post = MainLogic.Subreddit.SubmitTextPost(PostTuple.Item1, PostTuple.Item2);
                PostURL = Post.Shortlink.Replace("http://", "https://");

                return true;
            }
            catch
            {
                return false;
            }
        }

        private Tuple<string, string> GeneratePost()
        {
            string PostTitle = string.Empty;
            string PostBody = string.Empty;

            // Construct PostTitle
            PostTitle += "[Spoilers] " + DisplayedTitle + " - " + ShowType + " " + CrunchyrollEpisodeNumber;

            if (CrunchyrollEpisodeNumber == EpisodeCount)
                PostTitle += " - FINAL";

            PostTitle += " [Discussion]";

            // Display alternative episode number
            if (AKAOffset != 0)
                PostBody += "**Also known as:** " + ShowType + ' ' + (CrunchyrollEpisodeNumber + AKAOffset) + "\n\n";

            // Display episode title if there is one
            if (CrunchyrollEpisodeTitle != string.Empty)
                PostBody += "**Episode title:** " + CrunchyrollEpisodeTitle + "  \n";

            // Display episode duration if it's available
            if (CrunchyrollDuration != 0)
                PostBody += "**Episode duration:** " + SecondsToMinutes(CrunchyrollDuration) + '\n';

            // Display information section if it's not empty
            if (Informations.Any())
            {
                // Insert section divider
                PostBody += "\n";

                PostBody += "**Information:**  \n";
                foreach (Information Information in Informations)
                {
                    PostBody += "**" + Regex.Escape(Information.Website) + ":** [" + Regex.Escape(Information.Title)
                        + "](" + Regex.Escape(Information.TitleURL).Replace("http://", "https://") + ")  \n";
                }
            }

            // Always show streaming section since the Crunchyroll link will always be available at this point
            // Insert section divider
            PostBody += "\n";

            PostBody += "**Streaming:**  \n";
            // Add Crunchyroll link
            PostBody += "**Crunchyroll:** [" + Regex.Escape(Title)
                    + "](" + Regex.Escape(CrunchyrollURL).Replace("http://", "https://") + ")  \n";
            foreach (Information Streaming in Streamings)
            {
                PostBody += "**" + Regex.Escape(Streaming.Website) + ":** [" + Regex.Escape(Streaming.Title)
                    + "](" + Regex.Escape(Streaming.TitleURL).Replace("http://", "https://") + ")  \n";
            }

            // Display subreddits section if it's not empty
            if (Subreddits.Any())
            {
                // Insert section divider
                PostBody += "\n";

                PostBody += "**Subreddits:**\n\n";
                foreach (string Subreddit in Subreddits)
                    PostBody += "* /r/" + Subreddit + "\n";
            }

            // Insert previous episodes table if it's not empty
            PostBody += GenerateTable();

            // Display spoiler warning when source material exists
            if (SourceExists)
            {
                // Insert line section divider
                PostBody += "\n\n---\n\n";
                
                PostBody += "**Reminder:**  \n"
                    + "Please do not discuss any plot points which haven't appeared in the anime yet. "
                    + "Try not to confirm or deny any theories, encourage people to read the source material instead. "
                    + "Minor spoilers are generally ok but should be tagged accordingly. "
                    + "Failing to comply with the rules may result in your comment being removed.";
            }

            if (CrunchyrollKeywords != string.Empty || Keywords.Any())
            {
                // Insert line section divider
                PostBody += "\n\n---\n\n";

                PostBody += "**Keywords:**  \n";
                
                // Remove duplicates from the Crunchyroll keywords
                string NewKeywords = DistinctString(CrunchyrollKeywords);

                PostBody += NewKeywords;

                if (NewKeywords != string.Empty && Keywords.Any())
                    PostBody += ", ";
                
                foreach (string Keyword in Keywords)
                {
                    PostBody += Keyword + ", ";
                }

                PostBody = PostBody.TrimEnd(new char[]{ ',', ' '});
            }

            // Display hardcoded footer message
            PostBody += "\n\n---\n\n";
            PostBody += "This post is made by a bot. Any feedback is welcome and can be sent to /u/Shadoxfix.";

            return Tuple.Create(PostTitle, PostBody);
        }

        private string GenerateTable()
        {
            string Table = "\n\n---\n\n**Previous Episodes:**";
            List<Tuple<decimal, string>> PreviousEpisodes = new List<Tuple<decimal, string>>();

            // Get data from Episodes table
            string SelectEpisodesQuery = @"
                SELECT EpisodeNumber, PostURL
                FROM Episodes WHERE Id = @Id AND EpisodeNumber < @EpisodeNumber
                ORDER BY EpisodeNumber ASC";
            using (SQLiteCommand SelectEpisodesCommand = new SQLiteCommand(SelectEpisodesQuery, MainLogic.CurrentDB))
            {
                SelectEpisodesCommand.Parameters.AddWithValue("@Id", Id);
                SelectEpisodesCommand.Parameters.AddWithValue("@EpisodeNumber", CrunchyrollEpisodeNumber);
                using (SQLiteDataReader SelectEpisodes = SelectEpisodesCommand.ExecuteReader())
                {
                    while (SelectEpisodes.Read())
                        PreviousEpisodes.Add(Tuple.Create(decimal.Parse(SelectEpisodes[0].ToString()), SelectEpisodes[1].ToString()));
                }
            }

            int AmountOfPreviousEpisodes = PreviousEpisodes.Count;

            if (AmountOfPreviousEpisodes == 0)
                return string.Empty;

            for (int TableNumber = 0; TableNumber < AmountOfPreviousEpisodes / EntriesPerTable + 1; TableNumber++)
            {
                // Calculate the amount of columns needed for the current table. Doesn't exceed the max amount of rows
                int AmountOfColumnsRequired = (int) Math.Ceiling(
                    (double) Math.Min(AmountOfPreviousEpisodes - TableNumber * EntriesPerTable, EntriesPerTable) / (double) AmountOfRows);

                // This happens when an episode is the start of a new table. This could be omitted as it wouldn't result
                // in a noticeable difference but it prevents a few whitespaces in the post source.
                if (AmountOfColumnsRequired == 0)
                    break;

                Table += "\n\n";

                // Add table header
                for (int Column = 0; Column < AmountOfColumnsRequired; Column++)
                    Table += "|**Episode**|**Reddit Link**";

                Table += "\n";

                // Add table layout specifiers
                for (int Column = 0; Column < AmountOfColumnsRequired; Column++)
                    Table += "|:--|:-:";

                // Add table information
                for (int Row = 0; Row < AmountOfRows; Row++)
                {
                    Table += "\n";

                    for (int Column = 0; Column < AmountOfColumnsRequired; Column++)
                    {
                        // This method works by not caring about the fact we're accessing
                        // indexes outside the list range. A simple try catch that does nothing
                        // means the program can continue and is completely expected.
                        try
                        {
                            Table += "|Episode " + PreviousEpisodes[TableNumber * EntriesPerTable + Column * AmountOfRows + Row].Item1
                                + "|[Link](" + PreviousEpisodes[TableNumber * EntriesPerTable + Column * AmountOfRows + Row].Item2 + ")";
                        }
                        catch (ArgumentOutOfRangeException) { }
                    }
                }
            }

            return Table;
        }

        private string SecondsToMinutes(int totalSeconds)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return minutes + " " + (minutes != 1 ? "minutes" : "minute") + " and " + seconds + " " + (seconds != 1 ? "seconds" : "second");
        }

        private string DistinctString(string fullString)
        {
            // Split the string with , as seperator
            string[] FullStringSplit = fullString.Split(',');

            // Remove space at the beginning if it exists
            for (int SplitCount = 0; SplitCount < FullStringSplit.Count(); SplitCount++)
                if (char.IsWhiteSpace(FullStringSplit[SplitCount], 0))
                    FullStringSplit[SplitCount] = FullStringSplit[SplitCount].Remove(0, 1);

            // Empty the string to start rebuilding the distinct version
            fullString = string.Empty;

            foreach (string Split in FullStringSplit.Distinct())
                fullString += Split + ", ";

            // Trim the end of the rebuilded string
            char[] CharsToTrim = { ',', ' ' };
            fullString = fullString.TrimEnd(CharsToTrim);

            return fullString;
        }

        private bool IsNewEpisode()
        {
            // Count the amount of rows that has the same episode number. If it is 0 the current episode
            // is a new entry. (Is there a better way of doing this?)
            string CountEpisodeQuery = @"
                SELECT COUNT(*)
                FROM Episodes WHERE Id = @Id AND EpisodeNumber = @EpisodeNumber";
            using (SQLiteCommand CountEpisodeCommand = new SQLiteCommand(CountEpisodeQuery, MainLogic.CurrentDB))
            {
                CountEpisodeCommand.Parameters.AddWithValue("@Id", Id);
                CountEpisodeCommand.Parameters.AddWithValue("@EpisodeNumber", CrunchyrollEpisodeNumber);
                int EpisodeCount = Convert.ToInt32(CountEpisodeCommand.ExecuteScalar());

                return EpisodeCount == 0;
            }
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
            string SelectInformationQuery = @"
                SELECT Website, Title, TitleURL
                FROM Information WHERE Id = @Id
                ORDER BY Website ASC";
            using (SQLiteCommand SelectInformationCommand = new SQLiteCommand(SelectInformationQuery, MainLogic.CurrentDB))
            {
                SelectInformationCommand.Parameters.AddWithValue("@Id", Id);
                using (SQLiteDataReader SelectInformation = SelectInformationCommand.ExecuteReader())
                {
                    Informations = new List<Information>();
                    while (SelectInformation.Read())
                    {
                        Information Information;
                        Information.Website = SelectInformation[0].ToString();
                        Information.Title = SelectInformation[1].ToString();
                        Information.TitleURL = SelectInformation[2].ToString();
                        Informations.Add(Information);
                    }
                }
            }

            // Get data from Streaming table
            // This query could be unioned with the previous one but when posting
            // to reddit there is whitespace in between so we need to be able to distinguish
            string SelectStreamingQuery = @"
                SELECT Website, Title, TitleURL
                FROM Streaming WHERE Id = @Id AND Website != 'Crunchyroll'
                ORDER BY Website ASC";
            using (SQLiteCommand SelectStreamingCommand = new SQLiteCommand(SelectStreamingQuery, MainLogic.CurrentDB))
            {
                SelectStreamingCommand.Parameters.AddWithValue("@Id", Id);
                using (SQLiteDataReader SelectStreaming = SelectStreamingCommand.ExecuteReader())
                {
                    Streamings = new List<Information>();
                    while (SelectStreaming.Read())
                    {
                        Information Streaming;
                        Streaming.Website = SelectStreaming[0].ToString();
                        Streaming.Title = SelectStreaming[1].ToString();
                        Streaming.TitleURL = SelectStreaming[2].ToString();
                        Streamings.Add(Streaming);
                    }
                }
            }

            // Get data from Subreddits table
            string SelectSubredditQuery = @"
                SELECT Subreddit
                FROM Subreddits WHERE Id = @Id
                ORDER BY Subreddit ASC";
            using (SQLiteCommand SelectSubredditsCommand = new SQLiteCommand(SelectSubredditQuery, MainLogic.CurrentDB))
            {
                SelectSubredditsCommand.Parameters.AddWithValue("@Id", Id);
                using (SQLiteDataReader SelectSubreddits = SelectSubredditsCommand.ExecuteReader())
                {
                    Subreddits = new List<string>();
                    while (SelectSubreddits.Read())
                        Subreddits.Add(SelectSubreddits[0].ToString());
                }
            }

            // Get data from Keywords table
            string SelectKeywordsQuery = @"
                SELECT Keyword
                FROM Keywords WHERE Id = @Id";
            using (SQLiteCommand SelectKeywordsCommand = new SQLiteCommand(SelectKeywordsQuery, MainLogic.CurrentDB))
            {
                SelectKeywordsCommand.Parameters.AddWithValue("@Id", Id);
                using (SQLiteDataReader SelectKeywords = SelectKeywordsCommand.ExecuteReader())
                {
                    Keywords = new List<string>();
                    while (SelectKeywords.Read())
                        Keywords.Add(SelectKeywords[0].ToString());
                }
            }

            // Get data from Shows table
            string SelectShowsQuery = @"
                SELECT EpisodeCount, ShowType, DisplayedTitle, SourceExists
                FROM Shows WHERE Id = @Id";
            using (SQLiteCommand SelectShowsCommand = new SQLiteCommand(SelectShowsQuery, MainLogic.CurrentDB))
            {
                SelectShowsCommand.Parameters.AddWithValue("@Id", Id);
                using (SQLiteDataReader SelectShows = SelectShowsCommand.ExecuteReader())
                {
                    if (SelectShows.Read())
                    {
                        EpisodeCount = SelectShows[0].ToString() == string.Empty ? (decimal?) null : decimal.Parse(SelectShows[0].ToString());
                        ShowType = SelectShows[1].ToString();
                        DisplayedTitle = SelectShows[2].ToString();
                        SourceExists = bool.Parse(SelectShows[3].ToString());
                    }
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
            CrunchyrollDuration = XElement.Element(Crunchyroll + "duration") != null ?
                int.Parse(XElement.Element(Crunchyroll + "duration").Value) : 0;
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
