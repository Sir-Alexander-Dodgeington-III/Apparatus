namespace DeleteSpecialTerms
{
    partial class DeleteSpecialTerms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeleteSpecialTerms));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pgTextbox = new System.Windows.Forms.TextBox();
            this.rangeTextbox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.goButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.reviewButton = new System.Windows.Forms.Button();
            this.branchTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Product Group:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Range:";
            // 
            // pgTextbox
            // 
            this.pgTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pgTextbox.Location = new System.Drawing.Point(118, 58);
            this.pgTextbox.Name = "pgTextbox";
            this.pgTextbox.Size = new System.Drawing.Size(97, 21);
            this.pgTextbox.TabIndex = 2;
            this.pgTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.pgTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // rangeTextbox
            // 
            this.rangeTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rangeTextbox.Location = new System.Drawing.Point(118, 98);
            this.rangeTextbox.Name = "rangeTextbox";
            this.rangeTextbox.Size = new System.Drawing.Size(97, 21);
            this.rangeTextbox.TabIndex = 3;
            this.rangeTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.rangeTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(11, 136);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(64, 64);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // goButton
            // 
            this.goButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goButton.Location = new System.Drawing.Point(81, 136);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(64, 64);
            this.goButton.TabIndex = 5;
            this.goButton.Text = "GO";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(148, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "- OR -";
            // 
            // reviewButton
            // 
            this.reviewButton.Enabled = false;
            this.reviewButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reviewButton.Location = new System.Drawing.Point(150, 136);
            this.reviewButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.reviewButton.Name = "reviewButton";
            this.reviewButton.Size = new System.Drawing.Size(64, 64);
            this.reviewButton.TabIndex = 7;
            this.reviewButton.Text = "Review";
            this.reviewButton.UseVisualStyleBackColor = true;
            this.reviewButton.Click += new System.EventHandler(this.reviewButton_Click);
            // 
            // branchTextbox
            // 
            this.branchTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.branchTextbox.Location = new System.Drawing.Point(118, 26);
            this.branchTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.branchTextbox.Name = "branchTextbox";
            this.branchTextbox.Size = new System.Drawing.Size(97, 21);
            this.branchTextbox.TabIndex = 8;
            this.branchTextbox.Text = "ALL";
            this.branchTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.branchTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_LeaveBranch);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 27);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Branch:";
            // 
            // DeleteSpecialTerms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(231, 210);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.branchTextbox);
            this.Controls.Add(this.reviewButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.rangeTextbox);
            this.Controls.Add(this.pgTextbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DeleteSpecialTerms";
            this.Text = "Remove Special Terms";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox pgTextbox;
        private System.Windows.Forms.TextBox rangeTextbox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button reviewButton;
        private System.Windows.Forms.TextBox branchTextbox;
        private System.Windows.Forms.Label label4;
    }
}