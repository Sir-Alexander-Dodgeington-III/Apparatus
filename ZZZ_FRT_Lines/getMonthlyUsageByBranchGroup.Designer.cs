namespace getMonthlyUsageByBranchGroup
{
    partial class getMonthlyUsageByBranchGroup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(getMonthlyUsageByBranchGroup));
            this.yearTextbox = new System.Windows.Forms.TextBox();
            this.branchTextbox = new System.Windows.Forms.TextBox();
            this.pgTextbox = new System.Windows.Forms.TextBox();
            this.rangeTextbox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.searchButton = new System.Windows.Forms.Button();
            this.yearLabel = new System.Windows.Forms.Label();
            this.branchLabel = new System.Windows.Forms.Label();
            this.pgLabel = new System.Windows.Forms.Label();
            this.rangeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // yearTextbox
            // 
            this.yearTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yearTextbox.Location = new System.Drawing.Point(134, 25);
            this.yearTextbox.Name = "yearTextbox";
            this.yearTextbox.Size = new System.Drawing.Size(100, 21);
            this.yearTextbox.TabIndex = 0;
            this.yearTextbox.Text = "ALL";
            this.yearTextbox.Enter += new System.EventHandler(this.textBox_Enter);
            this.yearTextbox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // branchTextbox
            // 
            this.branchTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.branchTextbox.Location = new System.Drawing.Point(134, 52);
            this.branchTextbox.Name = "branchTextbox";
            this.branchTextbox.Size = new System.Drawing.Size(100, 21);
            this.branchTextbox.TabIndex = 1;
            this.branchTextbox.Text = "ALL";
            this.branchTextbox.Enter += new System.EventHandler(this.textBox_Enter);
            this.branchTextbox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // pgTextbox
            // 
            this.pgTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pgTextbox.Location = new System.Drawing.Point(134, 79);
            this.pgTextbox.Name = "pgTextbox";
            this.pgTextbox.Size = new System.Drawing.Size(100, 21);
            this.pgTextbox.TabIndex = 2;
            this.pgTextbox.Text = "ALL";
            this.pgTextbox.Enter += new System.EventHandler(this.textBox_Enter);
            this.pgTextbox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // rangeTextbox
            // 
            this.rangeTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rangeTextbox.Location = new System.Drawing.Point(134, 106);
            this.rangeTextbox.Name = "rangeTextbox";
            this.rangeTextbox.Size = new System.Drawing.Size(100, 21);
            this.rangeTextbox.TabIndex = 3;
            this.rangeTextbox.Text = "ALL";
            this.rangeTextbox.Enter += new System.EventHandler(this.textBox_Enter);
            this.rangeTextbox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(25, 143);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(64, 64);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // searchButton
            // 
            this.searchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchButton.Location = new System.Drawing.Point(95, 143);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(64, 64);
            this.searchButton.TabIndex = 5;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // yearLabel
            // 
            this.yearLabel.AutoSize = true;
            this.yearLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yearLabel.Location = new System.Drawing.Point(22, 28);
            this.yearLabel.Name = "yearLabel";
            this.yearLabel.Size = new System.Drawing.Size(40, 15);
            this.yearLabel.TabIndex = 6;
            this.yearLabel.Text = "Year:";
            // 
            // branchLabel
            // 
            this.branchLabel.AutoSize = true;
            this.branchLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.branchLabel.Location = new System.Drawing.Point(22, 55);
            this.branchLabel.Name = "branchLabel";
            this.branchLabel.Size = new System.Drawing.Size(99, 15);
            this.branchLabel.TabIndex = 7;
            this.branchLabel.Text = "Branch Group:";
            // 
            // pgLabel
            // 
            this.pgLabel.AutoSize = true;
            this.pgLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pgLabel.Location = new System.Drawing.Point(22, 82);
            this.pgLabel.Name = "pgLabel";
            this.pgLabel.Size = new System.Drawing.Size(103, 15);
            this.pgLabel.TabIndex = 8;
            this.pgLabel.Text = "Product Group:";
            // 
            // rangeLabel
            // 
            this.rangeLabel.AutoSize = true;
            this.rangeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rangeLabel.Location = new System.Drawing.Point(22, 109);
            this.rangeLabel.Name = "rangeLabel";
            this.rangeLabel.Size = new System.Drawing.Size(53, 15);
            this.rangeLabel.TabIndex = 9;
            this.rangeLabel.Text = "Range:";
            // 
            // getMonthlyUsageByBranchGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 219);
            this.Controls.Add(this.rangeLabel);
            this.Controls.Add(this.pgLabel);
            this.Controls.Add(this.branchLabel);
            this.Controls.Add(this.yearLabel);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.rangeTextbox);
            this.Controls.Add(this.pgTextbox);
            this.Controls.Add(this.branchTextbox);
            this.Controls.Add(this.yearTextbox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "getMonthlyUsageByBranchGroup";
            this.Text = "Get Monthly Usage By Branch";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox yearTextbox;
        private System.Windows.Forms.TextBox branchTextbox;
        private System.Windows.Forms.TextBox pgTextbox;
        private System.Windows.Forms.TextBox rangeTextbox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.Label yearLabel;
        private System.Windows.Forms.Label branchLabel;
        private System.Windows.Forms.Label pgLabel;
        private System.Windows.Forms.Label rangeLabel;
    }
}