namespace LeftvsRight
{
    partial class LeftvsRight
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LeftvsRight));
            this.branchLabel = new System.Windows.Forms.Label();
            this.pgLabel = new System.Windows.Forms.Label();
            this.rangeLabel = new System.Windows.Forms.Label();
            this.leftLabel = new System.Windows.Forms.Label();
            this.rightLabel = new System.Windows.Forms.Label();
            this.branchTextbox = new System.Windows.Forms.TextBox();
            this.pgTextbox = new System.Windows.Forms.TextBox();
            this.rangeTextbox = new System.Windows.Forms.TextBox();
            this.leftTextbox = new System.Windows.Forms.TextBox();
            this.rightTextbox = new System.Windows.Forms.TextBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.searchButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // branchLabel
            // 
            this.branchLabel.AutoSize = true;
            this.branchLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.branchLabel.Location = new System.Drawing.Point(12, 19);
            this.branchLabel.Name = "branchLabel";
            this.branchLabel.Size = new System.Drawing.Size(56, 15);
            this.branchLabel.TabIndex = 0;
            this.branchLabel.Text = "Branch:";
            // 
            // pgLabel
            // 
            this.pgLabel.AutoSize = true;
            this.pgLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pgLabel.Location = new System.Drawing.Point(12, 45);
            this.pgLabel.Name = "pgLabel";
            this.pgLabel.Size = new System.Drawing.Size(30, 15);
            this.pgLabel.TabIndex = 1;
            this.pgLabel.Text = "PG:";
            // 
            // rangeLabel
            // 
            this.rangeLabel.AutoSize = true;
            this.rangeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rangeLabel.Location = new System.Drawing.Point(12, 71);
            this.rangeLabel.Name = "rangeLabel";
            this.rangeLabel.Size = new System.Drawing.Size(53, 15);
            this.rangeLabel.TabIndex = 2;
            this.rangeLabel.Text = "Range:";
            // 
            // leftLabel
            // 
            this.leftLabel.AutoSize = true;
            this.leftLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.leftLabel.Location = new System.Drawing.Point(12, 97);
            this.leftLabel.Name = "leftLabel";
            this.leftLabel.Size = new System.Drawing.Size(81, 15);
            this.leftLabel.TabIndex = 3;
            this.leftLabel.Text = "Define Left:";
            // 
            // rightLabel
            // 
            this.rightLabel.AutoSize = true;
            this.rightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rightLabel.Location = new System.Drawing.Point(12, 123);
            this.rightLabel.Name = "rightLabel";
            this.rightLabel.Size = new System.Drawing.Size(91, 15);
            this.rightLabel.TabIndex = 4;
            this.rightLabel.Text = "Define Right:";
            // 
            // branchTextbox
            // 
            this.branchTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.branchTextbox.Location = new System.Drawing.Point(121, 18);
            this.branchTextbox.Name = "branchTextbox";
            this.branchTextbox.Size = new System.Drawing.Size(100, 21);
            this.branchTextbox.TabIndex = 5;
            this.branchTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.branchTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // pgTextbox
            // 
            this.pgTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pgTextbox.Location = new System.Drawing.Point(121, 44);
            this.pgTextbox.Name = "pgTextbox";
            this.pgTextbox.Size = new System.Drawing.Size(100, 21);
            this.pgTextbox.TabIndex = 6;
            this.pgTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.pgTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // rangeTextbox
            // 
            this.rangeTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rangeTextbox.Location = new System.Drawing.Point(121, 70);
            this.rangeTextbox.Name = "rangeTextbox";
            this.rangeTextbox.Size = new System.Drawing.Size(100, 21);
            this.rangeTextbox.TabIndex = 7;
            this.rangeTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.rangeTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // leftTextbox
            // 
            this.leftTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.leftTextbox.Location = new System.Drawing.Point(121, 96);
            this.leftTextbox.Name = "leftTextbox";
            this.leftTextbox.Size = new System.Drawing.Size(100, 21);
            this.leftTextbox.TabIndex = 8;
            this.leftTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.leftTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // rightTextbox
            // 
            this.rightTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rightTextbox.Location = new System.Drawing.Point(121, 122);
            this.rightTextbox.Name = "rightTextbox";
            this.rightTextbox.Size = new System.Drawing.Size(100, 21);
            this.rightTextbox.TabIndex = 9;
            this.rightTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.rightTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // closeButton
            // 
            this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.Location = new System.Drawing.Point(12, 178);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(64, 64);
            this.closeButton.TabIndex = 10;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // searchButton
            // 
            this.searchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchButton.Location = new System.Drawing.Point(82, 178);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(64, 64);
            this.searchButton.TabIndex = 11;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // LeftvsRight
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 254);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.rightTextbox);
            this.Controls.Add(this.leftTextbox);
            this.Controls.Add(this.rangeTextbox);
            this.Controls.Add(this.pgTextbox);
            this.Controls.Add(this.branchTextbox);
            this.Controls.Add(this.rightLabel);
            this.Controls.Add(this.leftLabel);
            this.Controls.Add(this.rangeLabel);
            this.Controls.Add(this.pgLabel);
            this.Controls.Add(this.branchLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LeftvsRight";
            this.Text = "Right vs Left - Free Stock Search";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label branchLabel;
        private System.Windows.Forms.Label pgLabel;
        private System.Windows.Forms.Label rangeLabel;
        private System.Windows.Forms.Label leftLabel;
        private System.Windows.Forms.Label rightLabel;
        private System.Windows.Forms.TextBox branchTextbox;
        private System.Windows.Forms.TextBox pgTextbox;
        private System.Windows.Forms.TextBox rangeTextbox;
        private System.Windows.Forms.TextBox leftTextbox;
        private System.Windows.Forms.TextBox rightTextbox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button searchButton;
    }
}