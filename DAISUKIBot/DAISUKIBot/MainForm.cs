using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace DAISUKIBot
{
    public partial class MainForm : Form
    {
        private MainLogic MainLogic;
        private bool IsRunning = false;

        public MainForm()
        {
            InitializeComponent();
            MainLogic = new MainLogic(this);
            ThreadsListBox.DataSource = MainLogic.Shows;
        }

        private void ChooseDatabaseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog DatabaseDialog = new OpenFileDialog();

            // Allow filtering on .sqlite and all files.
            DatabaseDialog.Filter = "SQLite files (*.sqlite)|*.sqlite|All files (*.*)|*.*";
            DatabaseDialog.FilterIndex = 1;

            if (DatabaseDialog.ShowDialog() == DialogResult.OK)
            {
                MainLogic.CurrentDB = new SQLiteConnection("Data source=" + DatabaseDialog.FileName +
                    ";Version=3;Pooling=True;Max Pool Size=100;Foreign Keys=True");
                ChosenDatabaseLabel.Text = DatabaseDialog.SafeFileName;
                ToggleStatusButton.Enabled = !(SubredditTextBox.Text == string.Empty);
            }
            else
            {
                MainLogic.CurrentDB = null;
                ChosenDatabaseLabel.Text = "None";
                ToggleStatusButton.Enabled = false;
            }
        }

        private void ToggleStatusButton_Click(object sender, EventArgs e)
        {
            if (IsRunning && MainLogic.Shows.Count == 0)
            {
                MainLogic.CurrentDB.Close();
                MainLogic.UpdateTimer.Stop();
                IsRunning = false;
                SetUIStatus(false);
                MainLogic.WebProxy = null;
            }
            else if (IsRunning && MainLogic.Shows.Count > 0)
            {
                NewError("Not all threads have ended");
            }
            else
            {
                if (MainLogic.RedditSetup())
                {
                    MainLogic.CurrentDB.Open();
                    MainLogic.UpdateTimer.Start();
                    IsRunning = true;
                    SetUIStatus(true);
                    if (IPAddressTextBox.Text != string.Empty && PortTextBox.Text != string.Empty)
                        MainLogic.WebProxy = new System.Net.WebProxy(IPAddressTextBox.Text + ':' + PortTextBox.Text);
                }
            }
        }

        private void SetUIStatus(bool locked)
        {
            SubredditTextBox.Enabled = ChooseDatabaseButton.Enabled = !locked;
            IPAddressTextBox.Enabled = PortTextBox.Enabled = !locked;

            if (locked)
            {
                StatusLabel.Text = "Running";
                StatusLabel.ForeColor = Color.Green;
                ToggleStatusButton.Text = "Stop";
            }
            else
            {
                StatusLabel.Text = "Not running";
                StatusLabel.ForeColor = Color.Red;
                ToggleStatusButton.Text = "Start";
            }
        }

        private void SubredditTextBox_TextChanged(object sender, EventArgs e)
        {
            ToggleStatusButton.Enabled = !(SubredditTextBox.Text == string.Empty) && (MainLogic.CurrentDB != null);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Don't allow closing the program while it is running to prevent issues when the program is writing to the database.
            // Can be overridden by the user when shift is being held.
            if (IsRunning && ModifierKeys != Keys.Shift)
            {
                NewError("Stop before closing");
                e.Cancel = true;
            }
        }

        public void NewError(string errorMessage)
        {
            ErrorListBox.Items.Insert(0, (DateTime.Now.ToString("HH:mm:ss: ") + errorMessage));
        }

        public string GetSubreddit()
        {
            return SubredditTextBox.Text;
        }
    }
}
