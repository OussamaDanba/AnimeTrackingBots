using System;
using System.Data.SQLite;
using System.Drawing;
using RedditSharp;
using System.Windows.Forms;

namespace CrunchyrollBot
{
    public partial class MainForm : Form
    {
        private SQLiteConnection currentDB;
        private Timer updateTimer = new Timer();
        private bool isRunning = false;
        private Reddit reddit;

        public MainForm()
        {
            InitializeComponent();
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

        private void chooseDBButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog DBDialog = new OpenFileDialog();

            // Allow filtering on .sqlite and all files.
            DBDialog.Filter = "SQLite files (*.sqlite)|*.sqlite|All files (*.*)|*.*";
            DBDialog.FilterIndex = 1;

            if (DBDialog.ShowDialog() == DialogResult.OK)
            {
                currentDB = new SQLiteConnection("Data source=" + DBDialog.FileName);
                chosenDBLabel.Text = DBDialog.SafeFileName;
                toggleStatusButton.Enabled = !(subredditTextBox.Text == string.Empty);
            }
            else
            {
                currentDB = null;
                chosenDBLabel.Text = "None";
                toggleStatusButton.Enabled = false;
            }
        }

        private void toggleStatusButton_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                updateTimer.Stop();
                isRunning = false;
                setUIStatus(false);
            }
            else
            {
                if(redditSetup())
                {
                    updateTimer.Start();
                    isRunning = true;
                    setUIStatus(true);
                }
            }
        }

        private void setUIStatus(bool locked)
        {
            subredditTextBox.Enabled = chooseDBButton.Enabled = !locked;
            if (locked)
            {
                statusLabel.Text = "Running";
                statusLabel.ForeColor = Color.Green;
                toggleStatusButton.Text = "Stop";
            }
            else
            {
                statusLabel.Text = "Not running";
                statusLabel.ForeColor = Color.Red;
                toggleStatusButton.Text = "Start";
            }
        }

        private void subredditTextBox_TextChanged(object sender, EventArgs e)
        {
            toggleStatusButton.Enabled = !(subredditTextBox.Text == string.Empty) && (currentDB != null);
        }

        private bool redditSetup()
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
                reddit.GetSubreddit("/r/" + subredditTextBox.Text);
                
                return true;
            }
            catch
            {
                errorListBox.Items.Add("Failed reddit login");
                return false;
            }
        }
    }
}
