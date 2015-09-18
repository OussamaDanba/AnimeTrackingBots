using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrunchyrollBot
{
    public partial class MainForm : Form
    {
        private SQLiteConnection currentDB;

        public MainForm()
        {
            InitializeComponent();
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
            }
            else
            {
                currentDB = null;
                chosenDBLabel.Text = "None";
            }
        }
    }
}
