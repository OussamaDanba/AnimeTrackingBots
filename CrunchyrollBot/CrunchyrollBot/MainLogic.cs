using System;
using System.Data.SQLite;
using RedditSharp;
using System.ComponentModel;
using System.Windows.Forms;

namespace CrunchyrollBot
{
    public class MainLogic
    {
        private MainForm MainForm;
        public SQLiteConnection CurrentDB { get; set; }
        public Timer UpdateTimer = new Timer();
        private Reddit Reddit;
        private RedditSharp.Things.Subreddit Subreddit;
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
                // Empty for now
            }
        }

        public bool RedditSetup()
        {
            string Username = string.Empty;
            string Password = string.Empty;
            string ClientId = string.Empty;
            string ClientSecret = string.Empty;
            string RedirectURI = string.Empty;

            CurrentDB.Open();
            SQLiteDataReader RedditLogin = new SQLiteCommand(
                "SELECT * FROM User LIMIT 1", CurrentDB).ExecuteReader();

            if (RedditLogin.Read())
            {
                ClientId = RedditLogin[0].ToString();
                ClientSecret = RedditLogin[1].ToString();
                RedirectURI = RedditLogin[2].ToString();
                Username = RedditLogin[3].ToString();
                Password = RedditLogin[4].ToString();
            }
            CurrentDB.Close();

            try
            {
                AuthProvider AuthProvider = new AuthProvider(ClientId, ClientSecret, RedirectURI);
                Reddit = new Reddit(AuthProvider.GetOAuthToken(Username, Password));
                Subreddit = Reddit.GetSubreddit("/r/" + MainForm.GetSubreddit());

                return true;
            }
            catch
            {
                MainForm.ErrorListBox.Items.Add("Failed reddit login");
                return false;
            }
        }
    }
}
