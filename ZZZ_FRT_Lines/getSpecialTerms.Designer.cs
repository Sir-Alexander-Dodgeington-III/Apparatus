namespace getSpecialTerms
{
    partial class getSpecialTerms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(getSpecialTerms));
            this.cancelButton = new System.Windows.Forms.Button();
            this.searchButton = new System.Windows.Forms.Button();
            this.branchLabel = new System.Windows.Forms.Label();
            this.pgLabel = new System.Windows.Forms.Label();
            this.branchTextbox = new System.Windows.Forms.TextBox();
            this.pgTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.customerTextbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.AutoSize = true;
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(14, 142);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(2);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(68, 68);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // searchButton
            // 
            this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchButton.AutoSize = true;
            this.searchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchButton.Location = new System.Drawing.Point(86, 142);
            this.searchButton.Margin = new System.Windows.Forms.Padding(2);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(68, 68);
            this.searchButton.TabIndex = 4;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // branchLabel
            // 
            this.branchLabel.AutoSize = true;
            this.branchLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.branchLabel.Location = new System.Drawing.Point(11, 28);
            this.branchLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.branchLabel.Name = "branchLabel";
            this.branchLabel.Size = new System.Drawing.Size(59, 17);
            this.branchLabel.TabIndex = 2;
            this.branchLabel.Text = "Branch";
            // 
            // pgLabel
            // 
            this.pgLabel.AutoSize = true;
            this.pgLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pgLabel.Location = new System.Drawing.Point(11, 63);
            this.pgLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.pgLabel.Name = "pgLabel";
            this.pgLabel.Size = new System.Drawing.Size(119, 17);
            this.pgLabel.TabIndex = 3;
            this.pgLabel.Text = "Product Group:";
            // 
            // branchTextbox
            // 
            this.branchTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.branchTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.branchTextbox.Location = new System.Drawing.Point(150, 26);
            this.branchTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.branchTextbox.Name = "branchTextbox";
            this.branchTextbox.Size = new System.Drawing.Size(128, 23);
            this.branchTextbox.TabIndex = 1;
            this.branchTextbox.Text = "ALL";
            this.branchTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.branchTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // pgTextbox
            // 
            this.pgTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pgTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pgTextbox.Location = new System.Drawing.Point(150, 61);
            this.pgTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.pgTextbox.Name = "pgTextbox";
            this.pgTextbox.Size = new System.Drawing.Size(128, 23);
            this.pgTextbox.TabIndex = 2;
            this.pgTextbox.Text = "ALL";
            this.pgTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.pgTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 97);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Customer(s)";
            // 
            // customerTextbox
            // 
            this.customerTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.customerTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customerTextbox.Location = new System.Drawing.Point(150, 94);
            this.customerTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.customerTextbox.Name = "customerTextbox";
            this.customerTextbox.Size = new System.Drawing.Size(128, 23);
            this.customerTextbox.TabIndex = 3;
            this.customerTextbox.Text = "ALL";
            this.customerTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.customerTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // getSpecialTerms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 223);
            this.Controls.Add(this.customerTextbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pgTextbox);
            this.Controls.Add(this.branchTextbox);
            this.Controls.Add(this.pgLabel);
            this.Controls.Add(this.branchLabel);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.cancelButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "getSpecialTerms";
            this.Text = "Get Special Terms";
            this.Load += new System.EventHandler(this.getSpecialTerms_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.getSpecialTerms_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.Label branchLabel;
        private System.Windows.Forms.Label pgLabel;
        private System.Windows.Forms.TextBox branchTextbox;
        private System.Windows.Forms.TextBox pgTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox customerTextbox;
    }
}