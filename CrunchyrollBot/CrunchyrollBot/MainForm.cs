using System;
using System.Data.SQLite;
using System.Drawing;
using RedditSharp;
using System.Windows.Forms;

namespace CrunchyrollBot
{
    public partial class MainForm : Form
    {
        private MainLogic mainLogic;
        private bool isRunning = false;

        public MainForm()
        {
            InitializeComponent();
            mainLogic = new MainLogic(this);
        }

        private void chooseDBButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog DBDialog = new OpenFileDialog();

            // Allow filtering on .sqlite and all files.
            DBDialog.Filter = "SQLite files (*.sqlite)|*.sqlite|All files (*.*)|*.*";
            DBDialog.FilterIndex = 1;

            if (DBDialog.ShowDialog() == DialogResult.OK)
            {
                mainLogic.currentDB = new SQLiteConnection("Data source=" + DBDialog.FileName);
                chosenDBLabel.Text = DBDialog.SafeFileName;
                toggleStatusButton.Enabled = !(subredditTextBox.Text == string.Empty);
            }
            else
            {
                mainLogic.currentDB = null;
                chosenDBLabel.Text = "None";
                toggleStatusButton.Enabled = false;
            }
        }

        private void toggleStatusButton_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                mainLogic.currentDB.Close();
                mainLogic.updateTimer.Stop();
                isRunning = false;
                setUIStatus(false);
            }
            else
            {
                if(mainLogic.redditSetup())
                {
                    mainLogic.currentDB.Open();
                    mainLogic.updateTimer.Start();
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
            toggleStatusButton.Enabled = !(subredditTextBox.Text == string.Empty) && (mainLogic.currentDB != null);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Don't allow closing the program while it is running to prevent issues when the program is writing to the database.
            // Can be overridden by the user when shift is being held.
            if(isRunning && ModifierKeys != Keys.Shift)
            {
                errorListBox.Items.Add("Stop before closing");
                e.Cancel = true;
            }
        }

        public string getSubreddit()
        {
            return subredditTextBox.Text;
        }
    }
}
