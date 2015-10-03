using System;
using System.Data.SQLite;
using RedditSharp;
using System.ComponentModel;
using System.Windows.Forms;

namespace CrunchyrollBot
{
    public class MainLogic
    {
        private MainForm mainForm;
        public SQLiteConnection currentDB { get; set; }
        public Timer updateTimer = new Timer();
        private Reddit reddit;
        private RedditSharp.Things.Subreddit subreddit;
        public BindingList<Show> shows;

        public MainLogic(MainForm mainForm)
        {
            this.mainForm = mainForm;
            shows = new BindingList<Show>();

            updateTimer = new Timer();
            // Run the TimerEvent once every second
            updateTimer.Interval = 1000;
            updateTimer.Tick += new EventHandler(TimerEvent);
        }

        private void TimerEvent(object sender, EventArgs e)
        {
            // Only update every minute
            if (DateTime.Now.Second == 0)
            {
                // Empty for now
            }
        }

        public bool redditSetup()
        {
            string username = string.Empty;
            string password = string.Empty;
            string clientID = string.Empty;
            string clientSecret = string.Empty;
            string redirectURI = string.Empty;

            currentDB.Open();
            SQLiteDataReader redditLogin = new SQLiteCommand(
                "SELECT * FROM User LIMIT 1", currentDB).ExecuteReader();

            if (redditLogin.Read())
            {
                clientID = redditLogin[0].ToString();
                clientSecret = redditLogin[1].ToString();
                redirectURI = redditLogin[2].ToString();
                username = redditLogin[3].ToString();
                password = redditLogin[4].ToString();
            }
            currentDB.Close();

            try
            {
                AuthProvider authProvider = new AuthProvider(clientID, clientSecret, redirectURI);
                reddit = new Reddit(authProvider.GetOAuthToken(username, password));
                subreddit = reddit.GetSubreddit("/r/" + mainForm.getSubreddit());

                return true;
            }
            catch
            {
                mainForm.errorListBox.Items.Add("Failed reddit login");
                return false;
            }
        }
    }
}
