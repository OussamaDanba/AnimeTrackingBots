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
            this.statusLabel = new System.Windows.Forms.Label();
            this.toggleStatusButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.chosenDBLabel = new System.Windows.Forms.Label();
            this.chooseDBButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.subredditTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.recentListBox = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.errorListBox = new System.Windows.Forms.ListBox();
            this.threadsListBox = new System.Windows.Forms.ListBox();
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
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.ForeColor = System.Drawing.Color.Red;
            this.statusLabel.Location = new System.Drawing.Point(12, 39);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(62, 13);
            this.statusLabel.TabIndex = 1;
            this.statusLabel.Text = "Not running";
            // 
            // toggleStatusButton
            // 
            this.toggleStatusButton.Enabled = false;
            this.toggleStatusButton.Location = new System.Drawing.Point(12, 69);
            this.toggleStatusButton.Name = "toggleStatusButton";
            this.toggleStatusButton.Size = new System.Drawing.Size(120, 25);
            this.toggleStatusButton.TabIndex = 2;
            this.toggleStatusButton.Text = "Start";
            this.toggleStatusButton.UseVisualStyleBackColor = true;
            this.toggleStatusButton.Click += new System.EventHandler(this.toggleStatusButton_Click);
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
            // chosenDBLabel
            // 
            this.chosenDBLabel.AutoSize = true;
            this.chosenDBLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chosenDBLabel.Location = new System.Drawing.Point(162, 39);
            this.chosenDBLabel.Name = "chosenDBLabel";
            this.chosenDBLabel.Size = new System.Drawing.Size(33, 13);
            this.chosenDBLabel.TabIndex = 4;
            this.chosenDBLabel.Text = "None";
            // 
            // chooseDBButton
            // 
            this.chooseDBButton.Location = new System.Drawing.Point(162, 69);
            this.chooseDBButton.Name = "chooseDBButton";
            this.chooseDBButton.Size = new System.Drawing.Size(120, 25);
            this.chooseDBButton.TabIndex = 5;
            this.chooseDBButton.Text = "Choose database";
            this.chooseDBButton.UseVisualStyleBackColor = true;
            this.chooseDBButton.Click += new System.EventHandler(this.chooseDBButton_Click);
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
            // subredditTextBox
            // 
            this.subredditTextBox.Location = new System.Drawing.Point(315, 39);
            this.subredditTextBox.MaxLength = 20;
            this.subredditTextBox.Name = "subredditTextBox";
            this.subredditTextBox.Size = new System.Drawing.Size(120, 20);
            this.subredditTextBox.TabIndex = 7;
            this.subredditTextBox.Text = "anime";
            this.subredditTextBox.TextChanged += new System.EventHandler(this.subredditTextBox_TextChanged);
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
            // recentListBox
            // 
            this.recentListBox.FormattingEnabled = true;
            this.recentListBox.Location = new System.Drawing.Point(12, 131);
            this.recentListBox.Name = "recentListBox";
            this.recentListBox.Size = new System.Drawing.Size(297, 147);
            this.recentListBox.TabIndex = 9;
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
            // errorListBox
            // 
            this.errorListBox.FormattingEnabled = true;
            this.errorListBox.Location = new System.Drawing.Point(315, 131);
            this.errorListBox.Name = "errorListBox";
            this.errorListBox.Size = new System.Drawing.Size(120, 147);
            this.errorListBox.TabIndex = 11;
            // 
            // threadsListBox
            // 
            this.threadsListBox.FormattingEnabled = true;
            this.threadsListBox.Location = new System.Drawing.Point(441, 40);
            this.threadsListBox.Name = "threadsListBox";
            this.threadsListBox.Size = new System.Drawing.Size(392, 238);
            this.threadsListBox.TabIndex = 12;
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
            this.Controls.Add(this.threadsListBox);
            this.Controls.Add(this.errorListBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.recentListBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.subredditTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chooseDBButton);
            this.Controls.Add(this.chosenDBLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.toggleStatusButton);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "CrunchyrollBot";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button toggleStatusButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label chosenDBLabel;
        private System.Windows.Forms.Button chooseDBButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox subredditTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox threadsListBox;
        public System.Windows.Forms.ListBox recentListBox;
        public System.Windows.Forms.ListBox errorListBox;
    }
}

