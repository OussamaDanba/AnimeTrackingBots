using System;
using System.Collections.Generic;
using System.Data.SQLite;
using RedditSharp;
using System.IO;
using System.Linq;

namespace TaigaBot
{
    public struct Information
    {
        public string Website;
        public string Title;
        public string TitleURL;
    };

    class Program
    {
        static SQLiteConnection CurrentDB;
        static bool SourceExists;
        static string Title, ShowType, DisplayedTitle, PostURL;
        static int Id;
        static decimal InternalOffset, AKAOffset, MyAnimeListEpisodeNumber;
        static decimal? EpisodeCount;
        static List<Information> Informations, Streamings;
        static List<string> Subreddits, Keywords;
        static Dictionary<string, bool> Websites;
        static RedditSharp.Things.Subreddit Subreddit;
        const int AmountOfColumns = 3, AmountOfRows = 13, EntriesPerTable = AmountOfColumns * AmountOfRows;
        static string SubredditName;
        static Reddit Reddit;

        static void Main(string[] args)
        {
            if (args.Length != 2)
                return;

            Title = args[0];
            if (!decimal.TryParse(args[1], out MyAnimeListEpisodeNumber))
                return;

            string DatabaseLocation;
            try
            {
                using (StreamReader Reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "data.txt"))
                {
                    DatabaseLocation = Reader.ReadLine();
                    SubredditName = Reader.ReadLine();
                }
            }
            catch (FileNotFoundException) { return; }

            if (string.IsNullOrEmpty(DatabaseLocation) || !File.Exists(DatabaseLocation))
                return;

            CurrentDB = new SQLiteConnection("Data source=" + DatabaseLocation + ";Version=3;Foreign Keys=True");
            CurrentDB.Open();

            GetDatabaseData();

            if (!ApplyOffsetAndCheckValidity() || Id == 0)
            {
                CurrentDB.Close();
                return;
            }

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
                    using (SQLiteCommand InsertEpisodeCommand = new SQLiteCommand(InsertEpisodeQuery, CurrentDB))
                    {
                        InsertEpisodeCommand.Parameters.AddWithValue("@Id", Id);
                        InsertEpisodeCommand.Parameters.AddWithValue("@EpisodeNumber", MyAnimeListEpisodeNumber);
                        InsertEpisodeCommand.ExecuteNonQuery();
                    }
                }
                catch { }

                if (PostOnReddit())
                {
                    try
                    {
                        string UpdateEpisodeQuery = @"
                                    UPDATE Episodes
                                    SET PostURL = @PostURL
                                    WHERE Id = @Id AND EpisodeNumber = @EpisodeNumber";
                        using (SQLiteCommand UpdateEpisodeCommand = new SQLiteCommand(UpdateEpisodeQuery, CurrentDB))
                        {
                            UpdateEpisodeCommand.Parameters.AddWithValue("@PostURL", PostURL);
                            UpdateEpisodeCommand.Parameters.AddWithValue("@Id", Id);
                            UpdateEpisodeCommand.Parameters.AddWithValue("@EpisodeNumber", MyAnimeListEpisodeNumber);
                            UpdateEpisodeCommand.ExecuteNonQuery();
                        }
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        string DeleteEpisodeQuery = @"
                                    DELETE FROM Episodes
                                    WHERE Id = @Id AND EpisodeNumber = @EpisodeNumber";
                        using (SQLiteCommand DeleteEpisodeCommand = new SQLiteCommand(DeleteEpisodeQuery, CurrentDB))
                        {
                            DeleteEpisodeCommand.Parameters.AddWithValue("@Id", Id);
                            DeleteEpisodeCommand.Parameters.AddWithValue("@EpisodeNumber", MyAnimeListEpisodeNumber);
                            DeleteEpisodeCommand.ExecuteNonQuery();
                        }
                    }
                    catch { }
                }
            }

            CurrentDB.Close();
        }

        static bool PostOnReddit()
        {
            try
            {
                if (!RedditSetup())
                    return false;
                Tuple<string, string> PostTuple = GeneratePost();
                RedditSharp.Things.Post Post = Subreddit.SubmitTextPost(PostTuple.Item1, PostTuple.Item2);
                PostURL = Post.Shortlink.Replace("http://", "https://");

                return true;
            }
            catch
            {
                return false;
            }
        }

        static Tuple<string, string> GeneratePost()
        {
            string PostTitle = string.Empty;
            string PostBody = string.Empty;

            // Construct PostTitle
            PostTitle += "[Spoilers] " + DisplayedTitle + " - " + ShowType + " " + MyAnimeListEpisodeNumber;

            if (MyAnimeListEpisodeNumber == EpisodeCount)
                PostTitle += " - FINAL";

            PostTitle += " [Discussion]";

            // Display alternative episode number
            if (AKAOffset != 0)
                PostBody += "**Also known as:** " + ShowType + ' ' + (MyAnimeListEpisodeNumber + AKAOffset) + "\n\n";

            if (Streamings.Any())
            {
                // Insert section divider
                PostBody += "\n";

                PostBody += "**Streaming:**  \n";
                foreach (Information Streaming in Streamings)
                {
                    PostBody += "**" + EscapeString(Streaming.Website) + ":** [" + EscapeString(Streaming.Title)
                        + "](" + ReplaceProtocol(EscapeString(Streaming.TitleURL), Streaming.Website) + ")  \n";
                }
            }

            // Display information section if it's not empty
            if (Informations.Any())
            {
                // Insert section divider
                PostBody += "\n";

                PostBody += "**Information:**  \n";
                foreach (Information Information in Informations)
                {
                    PostBody += "**" + EscapeString(Information.Website) + ":** [" + EscapeString(Information.Title)
                        + "](" + ReplaceProtocol(EscapeString(Information.TitleURL), Information.Website) + ")  \n";
                }
            }

            // Display subreddits section if it's not empty
            if (Subreddits.Any())
            {
                // Insert section divider
                PostBody += "\n";

                if (Subreddits.Count == 1)
                {
                    PostBody += "**Subreddit:** /r/" + Subreddits[0] + "\n";
                }
                else
                {
                    PostBody += "**Subreddits:**\n\n";
                    foreach (string Subreddit in Subreddits)
                        PostBody += "* /r/" + Subreddit + "\n";
                }
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

            if (Keywords.Any())
            {
                // Insert line section divider
                PostBody += "\n\n---\n\n";

                PostBody += "**Keywords:**  \n";

                foreach (string Keyword in Keywords)
                {
                    PostBody += Keyword + ", ";
                }

                PostBody = PostBody.TrimEnd(new char[] { ',', ' ' });
            }

            return Tuple.Create(PostTitle, PostBody);
        }

        // Replaces the http:// by https:// or inversed based on whether the website supports HTTPS or not
        static string ReplaceProtocol(string URL, string website)
        {
            if (Websites[website])
                return URL.Replace("http://", "https://");
            else
                return URL.Replace("https://", "http://");
        }

        // Escapes { (, ), [, ] } so that it doesn't interfere with reddit formatting
        static string EscapeString(string fullString)
        {
            fullString = fullString.Replace("(", "\\(");
            fullString = fullString.Replace(")", "\\)");
            fullString = fullString.Replace("[", "\\[");
            fullString = fullString.Replace("]", "\\]");

            return fullString;
        }

        static string GenerateTable()
        {
            string Table = "\n\n---\n\n**Previous Episodes:**";
            List<Tuple<decimal, string>> PreviousEpisodes = new List<Tuple<decimal, string>>();

            // Get data from Episodes table
            string SelectEpisodesQuery = @"
                SELECT EpisodeNumber, PostURL
                FROM Episodes WHERE Id = @Id AND EpisodeNumber < @EpisodeNumber
                ORDER BY EpisodeNumber ASC";
            using (SQLiteCommand SelectEpisodesCommand = new SQLiteCommand(SelectEpisodesQuery, CurrentDB))
            {
                SelectEpisodesCommand.Parameters.AddWithValue("@Id", Id);
                SelectEpisodesCommand.Parameters.AddWithValue("@EpisodeNumber", MyAnimeListEpisodeNumber);
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
                int AmountOfColumnsRequired = (int)Math.Ceiling(
                    (double)Math.Min(AmountOfPreviousEpisodes - TableNumber * EntriesPerTable, EntriesPerTable) / (double)AmountOfRows);

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
                            Table += "|" + ShowType + " " + PreviousEpisodes[TableNumber * EntriesPerTable + Column * AmountOfRows + Row].Item1
                                + "|[Link](" + PreviousEpisodes[TableNumber * EntriesPerTable + Column * AmountOfRows + Row].Item2 + ")";
                        }
                        catch (ArgumentOutOfRangeException) { }
                    }
                }
            }

            return Table;
        }

        static bool IsNewEpisode()
        {
            // Count the amount of rows that has the same episode number. If it is 0 the current episode
            // is a new entry. (Is there a better way of doing this?)
            string CountEpisodeQuery = @"
                SELECT COUNT(*)
                FROM Episodes WHERE Id = @Id AND EpisodeNumber = @EpisodeNumber";
            using (SQLiteCommand CountEpisodeCommand = new SQLiteCommand(CountEpisodeQuery, CurrentDB))
            {
                CountEpisodeCommand.Parameters.AddWithValue("@Id", Id);
                CountEpisodeCommand.Parameters.AddWithValue("@EpisodeNumber", MyAnimeListEpisodeNumber);
                int EpisodeCount = Convert.ToInt32(CountEpisodeCommand.ExecuteScalar());

                return EpisodeCount == 0;
            }
        }

        static void GetDatabaseData()
        {
            string SelectMyAnimeListQuery = @"
                SELECT Id, InternalOffset, AKAOffset
                FROM Streaming WHERE Website = 'MyAnimeList' AND Title = @Title";
            using (SQLiteCommand SelectMyAnimeListCommand = new SQLiteCommand(SelectMyAnimeListQuery, CurrentDB))
            {
                SelectMyAnimeListCommand.Parameters.AddWithValue("@Title", Title);
                using (SQLiteDataReader SelectMyAnimeList = SelectMyAnimeListCommand.ExecuteReader())
                {
                    if (SelectMyAnimeList.Read())
                    {
                        Id = int.Parse(SelectMyAnimeList[0].ToString());
                        InternalOffset = decimal.Parse(SelectMyAnimeList[1].ToString());
                        AKAOffset = decimal.Parse(SelectMyAnimeList[2].ToString());
                    }
                }
            }

            // Get data from Information table
            string SelectInformationQuery = @"
                SELECT Website, Title, TitleURL
                FROM Information WHERE Id = @Id AND Website = 'MyAnimeList'
                UNION ALL
                SELECT Website, Title, TitleURL
                FROM Information WHERE Id = @Id AND Website != 'MyAnimeList'";
            using (SQLiteCommand SelectInformationCommand = new SQLiteCommand(SelectInformationQuery, CurrentDB))
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
                FROM Streaming WHERE Id = @Id AND Website != 'MyAnimeList'
                ORDER BY Website ASC";
            using (SQLiteCommand SelectStreamingCommand = new SQLiteCommand(SelectStreamingQuery, CurrentDB))
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
            using (SQLiteCommand SelectSubredditsCommand = new SQLiteCommand(SelectSubredditQuery, CurrentDB))
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
            using (SQLiteCommand SelectKeywordsCommand = new SQLiteCommand(SelectKeywordsQuery, CurrentDB))
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
            using (SQLiteCommand SelectShowsCommand = new SQLiteCommand(SelectShowsQuery, CurrentDB))
            {
                SelectShowsCommand.Parameters.AddWithValue("@Id", Id);
                using (SQLiteDataReader SelectShows = SelectShowsCommand.ExecuteReader())
                {
                    if (SelectShows.Read())
                    {
                        EpisodeCount = SelectShows[0].ToString() == string.Empty ? (decimal?)null : decimal.Parse(SelectShows[0].ToString());
                        ShowType = SelectShows[1].ToString();
                        DisplayedTitle = SelectShows[2].ToString();
                        SourceExists = bool.Parse(SelectShows[3].ToString());
                    }
                }
            }

            // Get data from Websites table
            string SelectWebsiteQuery = @"
                SELECT *
                FROM Websites";
            using (SQLiteCommand SelectWebsitesCommand = new SQLiteCommand(SelectWebsiteQuery, CurrentDB))
            {
                using (SQLiteDataReader SelectWebsites = SelectWebsitesCommand.ExecuteReader())
                {
                    Websites = new Dictionary<string, bool>();
                    while (SelectWebsites.Read())
                    {
                        Websites.Add(SelectWebsites[0].ToString(), bool.Parse(SelectWebsites[1].ToString()));
                    }
                }
            }
        }

        static bool ApplyOffsetAndCheckValidity()
        {
            // There is no offset so it does not have to be applied or checked
            if (InternalOffset == 0)
            {
                return true;
            }
            else
            {
                MyAnimeListEpisodeNumber += InternalOffset;
                // The episode is actually from this season and shouldn't be skipped
                return MyAnimeListEpisodeNumber > 0;
            }
        }

        static bool RedditSetup()
        {
            string Username = string.Empty;
            string Password = string.Empty;

            string RedditLoginQuery = "SELECT * FROM User LIMIT 1";
            using (SQLiteCommand RedditLoginCommand = new SQLiteCommand(RedditLoginQuery, CurrentDB))
            {
                using (SQLiteDataReader RedditLogin = RedditLoginCommand.ExecuteReader())
                {
                    if (RedditLogin.Read())
                    {
                        Username = RedditLogin[0].ToString();
                        Password = RedditLogin[1].ToString();
                    }
                }
            }

            try
            {
                Reddit = new Reddit(Username, Password, true);
                Subreddit = Reddit.GetSubreddit("/r/" + SubredditName);

                return true;
            }
            catch { return false; }
        }
    }
}