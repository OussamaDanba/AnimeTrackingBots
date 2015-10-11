using System;
using System.Data.SQLite;
using RedditSharp;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;

namespace DAISUKIBot
{
    public class MainLogic
    {
        public static MainForm MainForm;
        public static SQLiteConnection CurrentDB { get; set; }
        public static WebProxy WebProxy;
        public Timer UpdateTimer = new Timer();
        private Reddit Reddit;
        public static RedditSharp.Things.Subreddit Subreddit;
        public BindingList<Show> Shows;

        public MainLogic(MainForm mainForm)
        {
            MainForm = mainForm;
            Shows = new BindingList<Show>();

            UpdateTimer = new Timer();
            // Run the TimerEvent once every second
            UpdateTimer.Interval = 1000;
            UpdateTimer.Tick += new EventHandler(TimerEvent);
        }

        private void TimerEvent(object sender, EventArgs e)
        {
            // Only update every minute
            if (DateTime.Now.Second == 0)
            {
                string SelectShowsQuery = @"
                    SELECT Id, Source, InternalTitle, Title, InternalOffset, AKAOffset, Wildcard
                    FROM Streaming WHERE Website = 'DAISUKI'";
                using (SQLiteCommand SelectShowsCommand = new SQLiteCommand(SelectShowsQuery, CurrentDB))
                using (SQLiteDataReader SelectShows = SelectShowsCommand.ExecuteReader())
                {
                    while (SelectShows.Read())
                    {
                        Show NewShow = new Show(int.Parse(SelectShows[0].ToString()), SelectShows[1].ToString(),
                            SelectShows[2].ToString(), SelectShows[3].ToString(),
                            decimal.Parse(SelectShows[4].ToString()), decimal.Parse(SelectShows[5].ToString()),
                            SelectShows[6].ToString());

                        if (!Shows.Contains(NewShow))
                        {
                            Shows.Add(NewShow);

                            BackgroundWorker bw = new BackgroundWorker();
                            bw.DoWork += new DoWorkEventHandler(NewShow.GetShowDataAndPost);

                            // Since the BackgroundWorker is created in this thread the RunWorkerCompleted
                            // is also run in this thread. This eliminates the possibility of a race condition.
                            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                            delegate (object o, RunWorkerCompletedEventArgs args)
                            {
                                if (args.Result != null)
                                {
                                    MainForm.NewError(args.Result.ToString());
                                }
                                Shows.Remove(NewShow);
                            });

                            bw.RunWorkerAsync();
                        }
                    }
                }
            }
        }

        public bool RedditSetup()
        {
            string Username = string.Empty;
            string Password = string.Empty;
            string ClientId = string.Empty;
            string ClientSecret = string.Empty;
            string RedirectURI = string.Empty;

            string RedditLoginQuery = "SELECT * FROM User LIMIT 1";
            using (SQLiteCommand RedditLoginCommand = new SQLiteCommand(RedditLoginQuery, CurrentDB))
            {
                CurrentDB.Open();
                using (SQLiteDataReader RedditLogin = RedditLoginCommand.ExecuteReader())
                {
                    if (RedditLogin.Read())
                    {
                        ClientId = RedditLogin[0].ToString();
                        ClientSecret = RedditLogin[1].ToString();
                        RedirectURI = RedditLogin[2].ToString();
                        Username = RedditLogin[3].ToString();
                        Password = RedditLogin[4].ToString();
                    }
                }
                CurrentDB.Close();
            }

            try
            {
                AuthProvider AuthProvider = new AuthProvider(ClientId, ClientSecret, RedirectURI);
                Reddit = new Reddit(AuthProvider.GetOAuthToken(Username, Password));
                Subreddit = Reddit.GetSubreddit("/r/" + MainForm.GetSubreddit());

                return true;
            }
            catch
            {
                MainForm.NewError("Failed reddit login");
                return false;
            }
        }
    }
}
