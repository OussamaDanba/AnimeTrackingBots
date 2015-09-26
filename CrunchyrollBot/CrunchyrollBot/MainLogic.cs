using System;
using System.Data.SQLite;
using RedditSharp;
using System.Windows.Forms;

namespace CrunchyrollBot
{
    public class MainLogic
    {
        private MainForm mainForm;
        public SQLiteConnection currentDB { get; set; }
        public Timer updateTimer = new Timer();
        private Reddit reddit;

        public MainLogic(MainForm mainForm)
        {
            this.mainForm = mainForm;
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

            currentDB.Open();
            SQLiteDataReader redditLogin = new SQLiteCommand(
                "SELECT * FROM User LIMIT 1", currentDB).ExecuteReader();

            if (redditLogin.Read())
            {
                username = redditLogin[0].ToString();
                password = redditLogin[1].ToString();
            }
            currentDB.Close();

            try
            {
                reddit = new Reddit(username, password, true);
                reddit.GetSubreddit("/r/" + mainForm.getSubreddit());

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
