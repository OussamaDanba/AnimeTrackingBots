namespace CrunchyrollBot
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.ToggleStatusButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ChosenDatabaseLabel = new System.Windows.Forms.Label();
            this.ChooseDatabaseButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SubredditTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.RecentListBox = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ErrorListBox = new System.Windows.Forms.ListBox();
            this.ThreadsListBox = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Status:";
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.ForeColor = System.Drawing.Color.Red;
            this.StatusLabel.Location = new System.Drawing.Point(12, 39);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(62, 13);
            this.StatusLabel.TabIndex = 1;
            this.StatusLabel.Text = "Not running";
            // 
            // ToggleStatusButton
            // 
            this.ToggleStatusButton.Enabled = false;
            this.ToggleStatusButton.Location = new System.Drawing.Point(12, 69);
            this.ToggleStatusButton.Name = "ToggleStatusButton";
            this.ToggleStatusButton.Size = new System.Drawing.Size(120, 25);
            this.ToggleStatusButton.TabIndex = 2;
            this.ToggleStatusButton.Text = "Start";
            this.ToggleStatusButton.UseVisualStyleBackColor = true;
            this.ToggleStatusButton.Click += new System.EventHandler(this.ToggleStatusButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(162, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Current database:";
            // 
            // ChosenDatabaseLabel
            // 
            this.ChosenDatabaseLabel.AutoSize = true;
            this.ChosenDatabaseLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ChosenDatabaseLabel.Location = new System.Drawing.Point(162, 39);
            this.ChosenDatabaseLabel.Name = "ChosenDatabaseLabel";
            this.ChosenDatabaseLabel.Size = new System.Drawing.Size(33, 13);
            this.ChosenDatabaseLabel.TabIndex = 4;
            this.ChosenDatabaseLabel.Text = "None";
            // 
            // ChooseDatabaseButton
            // 
            this.ChooseDatabaseButton.Location = new System.Drawing.Point(162, 69);
            this.ChooseDatabaseButton.Name = "ChooseDatabaseButton";
            this.ChooseDatabaseButton.Size = new System.Drawing.Size(120, 25);
            this.ChooseDatabaseButton.TabIndex = 5;
            this.ChooseDatabaseButton.Text = "Choose database";
            this.ChooseDatabaseButton.UseVisualStyleBackColor = true;
            this.ChooseDatabaseButton.Click += new System.EventHandler(this.ChooseDatabaseButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(312, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Subreddit:";
            // 
            // SubredditTextBox
            // 
            this.SubredditTextBox.Location = new System.Drawing.Point(315, 39);
            this.SubredditTextBox.MaxLength = 20;
            this.SubredditTextBox.Name = "SubredditTextBox";
            this.SubredditTextBox.Size = new System.Drawing.Size(120, 20);
            this.SubredditTextBox.TabIndex = 7;
            this.SubredditTextBox.Text = "anime";
            this.SubredditTextBox.TextChanged += new System.EventHandler(this.SubredditTextBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Recent:";
            // 
            // RecentListBox
            // 
            this.RecentListBox.FormattingEnabled = true;
            this.RecentListBox.Location = new System.Drawing.Point(12, 131);
            this.RecentListBox.Name = "RecentListBox";
            this.RecentListBox.Size = new System.Drawing.Size(297, 147);
            this.RecentListBox.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(312, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 16);
            this.label5.TabIndex = 10;
            this.label5.Text = "Errors:";
            // 
            // ErrorListBox
            // 
            this.ErrorListBox.FormattingEnabled = true;
            this.ErrorListBox.Location = new System.Drawing.Point(315, 131);
            this.ErrorListBox.Name = "ErrorListBox";
            this.ErrorListBox.Size = new System.Drawing.Size(120, 147);
            this.ErrorListBox.TabIndex = 11;
            // 
            // ThreadsListBox
            // 
            this.ThreadsListBox.FormattingEnabled = true;
            this.ThreadsListBox.Location = new System.Drawing.Point(441, 40);
            this.ThreadsListBox.Name = "ThreadsListBox";
            this.ThreadsListBox.Size = new System.Drawing.Size(392, 238);
            this.ThreadsListBox.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(438, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 16);
            this.label6.TabIndex = 13;
            this.label6.Text = "Active threads:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(845, 290);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ThreadsListBox);
            this.Controls.Add(this.ErrorListBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.RecentListBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SubredditTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ChooseDatabaseButton);
            this.Controls.Add(this.ChosenDatabaseLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ToggleStatusButton);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "CrunchyrollBot";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Button ToggleStatusButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label ChosenDatabaseLabel;
        private System.Windows.Forms.Button ChooseDatabaseButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox SubredditTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox ThreadsListBox;
        public System.Windows.Forms.ListBox RecentListBox;
        public System.Windows.Forms.ListBox ErrorListBox;
    }
}

