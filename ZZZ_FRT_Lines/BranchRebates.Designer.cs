namespace BranchRebates
{
    partial class BranchRebates_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BranchRebates_Form));
            this.branchTextBox = new System.Windows.Forms.TextBox();
            this.acctTextBox = new System.Windows.Forms.TextBox();
            this.pgTextBox = new System.Windows.Forms.TextBox();
            this.StartDateTextBox = new System.Windows.Forms.TextBox();
            this.EndDateTextBox = new System.Windows.Forms.TextBox();
            this.BranchLabel = new System.Windows.Forms.Label();
            this.AcctLabel = new System.Windows.Forms.Label();
            this.PGLabel = new System.Windows.Forms.Label();
            this.StartDateLabel = new System.Windows.Forms.Label();
            this.EndDateLabel = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.quitButton = new System.Windows.Forms.Button();
            this.SearchButton = new System.Windows.Forms.Button();
            this.SaveAsButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // branchTextBox
            // 
            this.branchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.branchTextBox.Location = new System.Drawing.Point(59, 35);
            this.branchTextBox.Name = "branchTextBox";
            this.branchTextBox.Size = new System.Drawing.Size(100, 20);
            this.branchTextBox.TabIndex = 0;
            // 
            // acctTextBox
            // 
            this.acctTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.acctTextBox.Location = new System.Drawing.Point(227, 35);
            this.acctTextBox.Name = "acctTextBox";
            this.acctTextBox.Size = new System.Drawing.Size(100, 20);
            this.acctTextBox.TabIndex = 1;
            // 
            // pgTextBox
            // 
            this.pgTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgTextBox.Location = new System.Drawing.Point(435, 35);
            this.pgTextBox.Name = "pgTextBox";
            this.pgTextBox.Size = new System.Drawing.Size(100, 20);
            this.pgTextBox.TabIndex = 2;
            // 
            // StartDateTextBox
            // 
            this.StartDateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StartDateTextBox.Location = new System.Drawing.Point(622, 35);
            this.StartDateTextBox.Name = "StartDateTextBox";
            this.StartDateTextBox.Size = new System.Drawing.Size(100, 20);
            this.StartDateTextBox.TabIndex = 3;
            // 
            // EndDateTextBox
            // 
            this.EndDateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EndDateTextBox.Location = new System.Drawing.Point(807, 35);
            this.EndDateTextBox.Name = "EndDateTextBox";
            this.EndDateTextBox.Size = new System.Drawing.Size(100, 20);
            this.EndDateTextBox.TabIndex = 4;
            // 
            // BranchLabel
            // 
            this.BranchLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BranchLabel.AutoSize = true;
            this.BranchLabel.Location = new System.Drawing.Point(12, 38);
            this.BranchLabel.Name = "BranchLabel";
            this.BranchLabel.Size = new System.Drawing.Size(41, 13);
            this.BranchLabel.TabIndex = 5;
            this.BranchLabel.Text = "Branch";
            // 
            // AcctLabel
            // 
            this.AcctLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AcctLabel.AutoSize = true;
            this.AcctLabel.Location = new System.Drawing.Point(192, 38);
            this.AcctLabel.Name = "AcctLabel";
            this.AcctLabel.Size = new System.Drawing.Size(29, 13);
            this.AcctLabel.TabIndex = 6;
            this.AcctLabel.Text = "Acct";
            // 
            // PGLabel
            // 
            this.PGLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PGLabel.AutoSize = true;
            this.PGLabel.Location = new System.Drawing.Point(353, 38);
            this.PGLabel.Name = "PGLabel";
            this.PGLabel.Size = new System.Drawing.Size(76, 13);
            this.PGLabel.TabIndex = 7;
            this.PGLabel.Text = "Product Group";
            // 
            // StartDateLabel
            // 
            this.StartDateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StartDateLabel.AutoSize = true;
            this.StartDateLabel.Location = new System.Drawing.Point(561, 38);
            this.StartDateLabel.Name = "StartDateLabel";
            this.StartDateLabel.Size = new System.Drawing.Size(55, 13);
            this.StartDateLabel.TabIndex = 8;
            this.StartDateLabel.Text = "Start Date";
            // 
            // EndDateLabel
            // 
            this.EndDateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EndDateLabel.AutoSize = true;
            this.EndDateLabel.Location = new System.Drawing.Point(749, 38);
            this.EndDateLabel.Name = "EndDateLabel";
            this.EndDateLabel.Size = new System.Drawing.Size(52, 13);
            this.EndDateLabel.TabIndex = 9;
            this.EndDateLabel.Text = "End Date";
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 86);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1186, 459);
            this.dataGridView1.TabIndex = 10;
            // 
            // quitButton
            // 
            this.quitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.quitButton.Location = new System.Drawing.Point(12, 555);
            this.quitButton.Name = "quitButton";
            this.quitButton.Size = new System.Drawing.Size(64, 64);
            this.quitButton.TabIndex = 11;
            this.quitButton.Text = "Quit";
            this.quitButton.UseVisualStyleBackColor = true;
            this.quitButton.Click += new System.EventHandler(this.quitButton_Click);
            // 
            // SearchButton
            // 
            this.SearchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SearchButton.Location = new System.Drawing.Point(82, 555);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(64, 64);
            this.SearchButton.TabIndex = 12;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // SaveAsButton
            // 
            this.SaveAsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SaveAsButton.Location = new System.Drawing.Point(152, 555);
            this.SaveAsButton.Name = "SaveAsButton";
            this.SaveAsButton.Size = new System.Drawing.Size(64, 64);
            this.SaveAsButton.TabIndex = 13;
            this.SaveAsButton.Text = "Save as .CSV";
            this.SaveAsButton.UseVisualStyleBackColor = true;
            this.SaveAsButton.Click += new System.EventHandler(this.SaveAsButton_Click);
            // 
            // BranchRebates_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1210, 631);
            this.Controls.Add(this.SaveAsButton);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.quitButton);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.EndDateLabel);
            this.Controls.Add(this.StartDateLabel);
            this.Controls.Add(this.PGLabel);
            this.Controls.Add(this.AcctLabel);
            this.Controls.Add(this.BranchLabel);
            this.Controls.Add(this.EndDateTextBox);
            this.Controls.Add(this.StartDateTextBox);
            this.Controls.Add(this.pgTextBox);
            this.Controls.Add(this.acctTextBox);
            this.Controls.Add(this.branchTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BranchRebates_Form";
            this.Text = "Branch Rebates";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox branchTextBox;
        private System.Windows.Forms.TextBox acctTextBox;
        private System.Windows.Forms.TextBox pgTextBox;
        private System.Windows.Forms.TextBox StartDateTextBox;
        private System.Windows.Forms.TextBox EndDateTextBox;
        private System.Windows.Forms.Label BranchLabel;
        private System.Windows.Forms.Label AcctLabel;
        private System.Windows.Forms.Label PGLabel;
        private System.Windows.Forms.Label StartDateLabel;
        private System.Windows.Forms.Label EndDateLabel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button quitButton;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Button SaveAsButton;
    }
}