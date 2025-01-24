namespace StockLevelling
{
    partial class StockLevelling
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StockLevelling));
            this.branchLabel = new System.Windows.Forms.Label();
            this.branchComboBox = new System.Windows.Forms.ComboBox();
            this.PG_ListBox_Available = new System.Windows.Forms.ListBox();
            this.PG_ListBox_Current = new System.Windows.Forms.ListBox();
            this.availableLabel = new System.Windows.Forms.Label();
            this.currentPGsLabel = new System.Windows.Forms.Label();
            this.Range_ListBox_Available = new System.Windows.Forms.ListBox();
            this.rangeListBox_Current = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.currentRangesLabel = new System.Windows.Forms.Label();
            this.addPG_Button = new System.Windows.Forms.Button();
            this.removePG_Button = new System.Windows.Forms.Button();
            this.addRangeButton = new System.Windows.Forms.Button();
            this.removeRangeButton = new System.Windows.Forms.Button();
            this.quitButton = new System.Windows.Forms.Button();
            this.updateButton = new System.Windows.Forms.Button();
            this.splitTextBox = new System.Windows.Forms.TextBox();
            this.splitTextBox2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // branchLabel
            // 
            this.branchLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.branchLabel.AutoSize = true;
            this.branchLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.branchLabel.Location = new System.Drawing.Point(28, 44);
            this.branchLabel.Name = "branchLabel";
            this.branchLabel.Size = new System.Drawing.Size(47, 13);
            this.branchLabel.TabIndex = 0;
            this.branchLabel.Text = "Branch";
            // 
            // branchComboBox
            // 
            this.branchComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.branchComboBox.FormattingEnabled = true;
            this.branchComboBox.Location = new System.Drawing.Point(81, 41);
            this.branchComboBox.Name = "branchComboBox";
            this.branchComboBox.Size = new System.Drawing.Size(133, 21);
            this.branchComboBox.TabIndex = 1;
            // 
            // PG_ListBox_Available
            // 
            this.PG_ListBox_Available.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PG_ListBox_Available.FormattingEnabled = true;
            this.PG_ListBox_Available.Location = new System.Drawing.Point(31, 99);
            this.PG_ListBox_Available.Name = "PG_ListBox_Available";
            this.PG_ListBox_Available.Size = new System.Drawing.Size(207, 199);
            this.PG_ListBox_Available.TabIndex = 100;
            this.PG_ListBox_Available.Click += new System.EventHandler(this.PG_ListBox_Available_Click);
            // 
            // PG_ListBox_Current
            // 
            this.PG_ListBox_Current.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PG_ListBox_Current.FormattingEnabled = true;
            this.PG_ListBox_Current.Location = new System.Drawing.Point(327, 99);
            this.PG_ListBox_Current.Name = "PG_ListBox_Current";
            this.PG_ListBox_Current.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PG_ListBox_Current.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.PG_ListBox_Current.Size = new System.Drawing.Size(211, 199);
            this.PG_ListBox_Current.TabIndex = 3;
            this.PG_ListBox_Current.Click += new System.EventHandler(this.PG_ListBox_Current_Click);
            // 
            // availableLabel
            // 
            this.availableLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.availableLabel.AutoSize = true;
            this.availableLabel.Location = new System.Drawing.Point(90, 83);
            this.availableLabel.Name = "availableLabel";
            this.availableLabel.Size = new System.Drawing.Size(75, 13);
            this.availableLabel.TabIndex = 4;
            this.availableLabel.Text = "Available PG\'s";
            // 
            // currentPGsLabel
            // 
            this.currentPGsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.currentPGsLabel.AutoSize = true;
            this.currentPGsLabel.Location = new System.Drawing.Point(391, 83);
            this.currentPGsLabel.Name = "currentPGsLabel";
            this.currentPGsLabel.Size = new System.Drawing.Size(66, 13);
            this.currentPGsLabel.TabIndex = 5;
            this.currentPGsLabel.Text = "Current PG\'s";
            // 
            // Range_ListBox_Available
            // 
            this.Range_ListBox_Available.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Range_ListBox_Available.FormattingEnabled = true;
            this.Range_ListBox_Available.Location = new System.Drawing.Point(31, 340);
            this.Range_ListBox_Available.Name = "Range_ListBox_Available";
            this.Range_ListBox_Available.Size = new System.Drawing.Size(207, 199);
            this.Range_ListBox_Available.TabIndex = 101;
            this.Range_ListBox_Available.Click += new System.EventHandler(this.Range_ListBox_Available_Click);
            // 
            // rangeListBox_Current
            // 
            this.rangeListBox_Current.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rangeListBox_Current.FormattingEnabled = true;
            this.rangeListBox_Current.Location = new System.Drawing.Point(327, 340);
            this.rangeListBox_Current.Name = "rangeListBox_Current";
            this.rangeListBox_Current.Size = new System.Drawing.Size(211, 199);
            this.rangeListBox_Current.TabIndex = 7;
            this.rangeListBox_Current.Click += new System.EventHandler(this.rangeListBox_Current_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(90, 324);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Available Ranges";
            // 
            // currentRangesLabel
            // 
            this.currentRangesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.currentRangesLabel.AutoSize = true;
            this.currentRangesLabel.Location = new System.Drawing.Point(391, 324);
            this.currentRangesLabel.Name = "currentRangesLabel";
            this.currentRangesLabel.Size = new System.Drawing.Size(81, 13);
            this.currentRangesLabel.TabIndex = 9;
            this.currentRangesLabel.Text = "Current Ranges";
            // 
            // addPG_Button
            // 
            this.addPG_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addPG_Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.addPG_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addPG_Button.Location = new System.Drawing.Point(244, 165);
            this.addPG_Button.Name = "addPG_Button";
            this.addPG_Button.Size = new System.Drawing.Size(77, 23);
            this.addPG_Button.TabIndex = 10;
            this.addPG_Button.Text = "Add";
            this.addPG_Button.UseVisualStyleBackColor = true;
            this.addPG_Button.Click += new System.EventHandler(this.addPG_Button_Click);
            // 
            // removePG_Button
            // 
            this.removePG_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.removePG_Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.removePG_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removePG_Button.Location = new System.Drawing.Point(244, 194);
            this.removePG_Button.Name = "removePG_Button";
            this.removePG_Button.Size = new System.Drawing.Size(77, 23);
            this.removePG_Button.TabIndex = 11;
            this.removePG_Button.Text = "Remove";
            this.removePG_Button.UseVisualStyleBackColor = true;
            this.removePG_Button.Click += new System.EventHandler(this.removePG_Button_Click);
            // 
            // addRangeButton
            // 
            this.addRangeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addRangeButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.addRangeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addRangeButton.Location = new System.Drawing.Point(244, 405);
            this.addRangeButton.Name = "addRangeButton";
            this.addRangeButton.Size = new System.Drawing.Size(77, 23);
            this.addRangeButton.TabIndex = 12;
            this.addRangeButton.Text = "Add";
            this.addRangeButton.UseVisualStyleBackColor = true;
            this.addRangeButton.Click += new System.EventHandler(this.addRangeButton_Click);
            // 
            // removeRangeButton
            // 
            this.removeRangeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.removeRangeButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.removeRangeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removeRangeButton.Location = new System.Drawing.Point(244, 434);
            this.removeRangeButton.Name = "removeRangeButton";
            this.removeRangeButton.Size = new System.Drawing.Size(77, 23);
            this.removeRangeButton.TabIndex = 13;
            this.removeRangeButton.Text = "Remove";
            this.removeRangeButton.UseVisualStyleBackColor = true;
            this.removeRangeButton.Click += new System.EventHandler(this.removeRangeButton_Click);
            // 
            // quitButton
            // 
            this.quitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.quitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.quitButton.Location = new System.Drawing.Point(31, 558);
            this.quitButton.Name = "quitButton";
            this.quitButton.Size = new System.Drawing.Size(64, 64);
            this.quitButton.TabIndex = 14;
            this.quitButton.Text = "Quit";
            this.quitButton.UseVisualStyleBackColor = true;
            this.quitButton.Click += new System.EventHandler(this.quitButton_Click);
            // 
            // updateButton
            // 
            this.updateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.updateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateButton.Location = new System.Drawing.Point(101, 558);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(64, 64);
            this.updateButton.TabIndex = 15;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // splitTextBox
            // 
            this.splitTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitTextBox.Location = new System.Drawing.Point(219, 15);
            this.splitTextBox.Name = "splitTextBox";
            this.splitTextBox.Size = new System.Drawing.Size(335, 20);
            this.splitTextBox.TabIndex = 16;
            this.splitTextBox.Visible = false;
            this.splitTextBox.WordWrap = false;
            // 
            // splitTextBox2
            // 
            this.splitTextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitTextBox2.Location = new System.Drawing.Point(219, 41);
            this.splitTextBox2.Name = "splitTextBox2";
            this.splitTextBox2.Size = new System.Drawing.Size(335, 20);
            this.splitTextBox2.TabIndex = 17;
            this.splitTextBox2.Visible = false;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(171, 559);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 64);
            this.button1.TabIndex = 102;
            this.button1.Text = "Check Defaults";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // StockLevelling
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 634);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.splitTextBox2);
            this.Controls.Add(this.splitTextBox);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.quitButton);
            this.Controls.Add(this.removeRangeButton);
            this.Controls.Add(this.addRangeButton);
            this.Controls.Add(this.removePG_Button);
            this.Controls.Add(this.addPG_Button);
            this.Controls.Add(this.currentRangesLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rangeListBox_Current);
            this.Controls.Add(this.Range_ListBox_Available);
            this.Controls.Add(this.currentPGsLabel);
            this.Controls.Add(this.availableLabel);
            this.Controls.Add(this.PG_ListBox_Current);
            this.Controls.Add(this.PG_ListBox_Available);
            this.Controls.Add(this.branchComboBox);
            this.Controls.Add(this.branchLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StockLevelling";
            this.Text = "Stock Levelling";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label branchLabel;
        private System.Windows.Forms.ComboBox branchComboBox;
        private System.Windows.Forms.ListBox PG_ListBox_Available;
        private System.Windows.Forms.ListBox PG_ListBox_Current;
        private System.Windows.Forms.Label availableLabel;
        private System.Windows.Forms.Label currentPGsLabel;
        private System.Windows.Forms.ListBox Range_ListBox_Available;
        private System.Windows.Forms.ListBox rangeListBox_Current;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label currentRangesLabel;
        private System.Windows.Forms.Button addPG_Button;
        private System.Windows.Forms.Button removePG_Button;
        private System.Windows.Forms.Button addRangeButton;
        private System.Windows.Forms.Button removeRangeButton;
        private System.Windows.Forms.Button quitButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.TextBox splitTextBox;
        private System.Windows.Forms.TextBox splitTextBox2;
        private System.Windows.Forms.Button button1;
    }
}