using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace CrunchyrollBot
{
    public partial class MainForm : Form
    {
        private SQLiteConnection currentDB;
        private Timer updateTimer = new Timer();
        private bool isRunning = false;

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
                subredditTextBox.Enabled = chooseDBButton.Enabled = true;
                statusLabel.Text = "Not running";
                statusLabel.ForeColor = Color.Red;
                updateTimer.Stop();
                toggleStatusButton.Text = "Start";
                isRunning = false;
            }
            else
            {
                subredditTextBox.Enabled = chooseDBButton.Enabled = false;
                statusLabel.Text = "Running";
                statusLabel.ForeColor = Color.Green;
                updateTimer.Start();
                toggleStatusButton.Text = "Stop";
                isRunning = true;
            }
        }

        private void subredditTextBox_TextChanged(object sender, EventArgs e)
        {
            toggleStatusButton.Enabled = !(subredditTextBox.Text == string.Empty) && (currentDB != null);
        }
    }
}
