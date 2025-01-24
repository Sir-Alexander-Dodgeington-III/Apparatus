namespace P55SalesCriteriaForm
{
    partial class P55SalesCriteriaForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(P55SalesCriteriaForm));
            this.branchTextbox = new System.Windows.Forms.TextBox();
            this.promoNameTextBox = new System.Windows.Forms.TextBox();
            this.startDateTextbox = new System.Windows.Forms.TextBox();
            this.endDateTextbox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.branchLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.withDetailsCheckbox = new System.Windows.Forms.CheckBox();
            this.withTotalsCheckbox = new System.Windows.Forms.CheckBox();
            this.withBranchTotalsCheckbox = new System.Windows.Forms.CheckBox();
            this.repTotalsCheckbox = new System.Windows.Forms.CheckBox();
            this.repLabel = new System.Windows.Forms.Label();
            this.repTotalTextbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // branchTextbox
            // 
            this.branchTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.branchTextbox.Location = new System.Drawing.Point(175, 31);
            this.branchTextbox.Name = "branchTextbox";
            this.branchTextbox.Size = new System.Drawing.Size(132, 22);
            this.branchTextbox.TabIndex = 0;
            this.branchTextbox.Text = "ALL";
            this.branchTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.branchTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // promoNameTextBox
            // 
            this.promoNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.promoNameTextBox.Location = new System.Drawing.Point(175, 85);
            this.promoNameTextBox.Name = "promoNameTextBox";
            this.promoNameTextBox.Size = new System.Drawing.Size(132, 22);
            this.promoNameTextBox.TabIndex = 2;
            this.promoNameTextBox.Text = "ALL";
            this.promoNameTextBox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.promoNameTextBox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // startDateTextbox
            // 
            this.startDateTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startDateTextbox.Location = new System.Drawing.Point(175, 113);
            this.startDateTextbox.Name = "startDateTextbox";
            this.startDateTextbox.Size = new System.Drawing.Size(132, 22);
            this.startDateTextbox.TabIndex = 3;
            this.startDateTextbox.Text = "ALL";
            this.startDateTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.startDateTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // endDateTextbox
            // 
            this.endDateTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.endDateTextbox.Location = new System.Drawing.Point(175, 141);
            this.endDateTextbox.Name = "endDateTextbox";
            this.endDateTextbox.Size = new System.Drawing.Size(132, 22);
            this.endDateTextbox.TabIndex = 4;
            this.endDateTextbox.Text = "ALL";
            this.endDateTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.endDateTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // searchButton
            // 
            this.searchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchButton.Location = new System.Drawing.Point(243, 203);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(64, 64);
            this.searchButton.TabIndex = 5;
            this.searchButton.Text = "Search F5";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // branchLabel
            // 
            this.branchLabel.AutoSize = true;
            this.branchLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.branchLabel.Location = new System.Drawing.Point(26, 34);
            this.branchLabel.Name = "branchLabel";
            this.branchLabel.Size = new System.Drawing.Size(55, 16);
            this.branchLabel.TabIndex = 5;
            this.branchLabel.Text = "Branch";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(26, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "Promo Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(26, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Promo Start Date";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(26, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Promo End Date";
            // 
            // withDetailsCheckbox
            // 
            this.withDetailsCheckbox.AutoSize = true;
            this.withDetailsCheckbox.Checked = true;
            this.withDetailsCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.withDetailsCheckbox.Location = new System.Drawing.Point(12, 180);
            this.withDetailsCheckbox.Name = "withDetailsCheckbox";
            this.withDetailsCheckbox.Size = new System.Drawing.Size(58, 17);
            this.withDetailsCheckbox.TabIndex = 9;
            this.withDetailsCheckbox.Text = "Details";
            this.withDetailsCheckbox.UseVisualStyleBackColor = true;
            this.withDetailsCheckbox.Click += new System.EventHandler(this.withDetailsCheckbox_CheckedChanged);
            // 
            // withTotalsCheckbox
            // 
            this.withTotalsCheckbox.AutoSize = true;
            this.withTotalsCheckbox.Location = new System.Drawing.Point(12, 203);
            this.withTotalsCheckbox.Name = "withTotalsCheckbox";
            this.withTotalsCheckbox.Size = new System.Drawing.Size(55, 17);
            this.withTotalsCheckbox.TabIndex = 10;
            this.withTotalsCheckbox.Text = "Totals";
            this.withTotalsCheckbox.UseVisualStyleBackColor = true;
            this.withTotalsCheckbox.Click += new System.EventHandler(this.withTotalsCheckbox_CheckStateChanged);
            // 
            // withBranchTotalsCheckbox
            // 
            this.withBranchTotalsCheckbox.AutoSize = true;
            this.withBranchTotalsCheckbox.Location = new System.Drawing.Point(12, 226);
            this.withBranchTotalsCheckbox.Name = "withBranchTotalsCheckbox";
            this.withBranchTotalsCheckbox.Size = new System.Drawing.Size(92, 17);
            this.withBranchTotalsCheckbox.TabIndex = 11;
            this.withBranchTotalsCheckbox.Text = "Branch Totals";
            this.withBranchTotalsCheckbox.UseVisualStyleBackColor = true;
            this.withBranchTotalsCheckbox.Click += new System.EventHandler(this.withBranchTotalsCheckbox_Click);
            // 
            // repTotalsCheckbox
            // 
            this.repTotalsCheckbox.AutoSize = true;
            this.repTotalsCheckbox.Location = new System.Drawing.Point(12, 249);
            this.repTotalsCheckbox.Name = "repTotalsCheckbox";
            this.repTotalsCheckbox.Size = new System.Drawing.Size(78, 17);
            this.repTotalsCheckbox.TabIndex = 12;
            this.repTotalsCheckbox.Text = "Rep Totals";
            this.repTotalsCheckbox.UseVisualStyleBackColor = true;
            this.repTotalsCheckbox.Click += new System.EventHandler(this.repTotalsCheckbox_Click);
            // 
            // repLabel
            // 
            this.repLabel.AutoSize = true;
            this.repLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.repLabel.Location = new System.Drawing.Point(26, 60);
            this.repLabel.Name = "repLabel";
            this.repLabel.Size = new System.Drawing.Size(80, 16);
            this.repLabel.TabIndex = 13;
            this.repLabel.Text = "Sales Rep";
            // 
            // repTotalTextbox
            // 
            this.repTotalTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.repTotalTextbox.Location = new System.Drawing.Point(175, 59);
            this.repTotalTextbox.Name = "repTotalTextbox";
            this.repTotalTextbox.Size = new System.Drawing.Size(132, 22);
            this.repTotalTextbox.TabIndex = 1;
            this.repTotalTextbox.Text = "ALL";
            // 
            // P55SalesCriteriaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 279);
            this.Controls.Add(this.repTotalTextbox);
            this.Controls.Add(this.repLabel);
            this.Controls.Add(this.repTotalsCheckbox);
            this.Controls.Add(this.withBranchTotalsCheckbox);
            this.Controls.Add(this.withTotalsCheckbox);
            this.Controls.Add(this.withDetailsCheckbox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.branchLabel);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.endDateTextbox);
            this.Controls.Add(this.startDateTextbox);
            this.Controls.Add(this.promoNameTextBox);
            this.Controls.Add(this.branchTextbox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "P55SalesCriteriaForm";
            this.Text = "Search Criteria";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.P55SalesCriteriaForm_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox branchTextbox;
        private System.Windows.Forms.TextBox promoNameTextBox;
        private System.Windows.Forms.TextBox startDateTextbox;
        private System.Windows.Forms.TextBox endDateTextbox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.Label branchLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox withDetailsCheckbox;
        private System.Windows.Forms.CheckBox withTotalsCheckbox;
        private System.Windows.Forms.CheckBox withBranchTotalsCheckbox;
        private System.Windows.Forms.CheckBox repTotalsCheckbox;
        private System.Windows.Forms.Label repLabel;
        private System.Windows.Forms.TextBox repTotalTextbox;
    }
}