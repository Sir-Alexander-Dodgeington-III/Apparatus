namespace StockLevelling_New
{
    partial class StockLevelling_New
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StockLevelling_New));
            this.closeButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
            this.checkedListBox3 = new System.Windows.Forms.CheckedListBox();
            this.pgTextBox = new System.Windows.Forms.TextBox();
            this.rangeTextBox = new System.Windows.Forms.TextBox();
            this.addPG_Button = new System.Windows.Forms.Button();
            this.addRange_Button = new System.Windows.Forms.Button();
            this.allBranchesCheckBox = new System.Windows.Forms.CheckBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.Location = new System.Drawing.Point(26, 783);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(64, 64);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // applyButton
            // 
            this.applyButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.applyButton.Location = new System.Drawing.Point(96, 783);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(64, 64);
            this.applyButton.TabIndex = 1;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(26, 37);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(200, 709);
            this.checkedListBox1.TabIndex = 2;
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            // 
            // checkedListBox2
            // 
            this.checkedListBox2.FormattingEnabled = true;
            this.checkedListBox2.Location = new System.Drawing.Point(263, 67);
            this.checkedListBox2.Name = "checkedListBox2";
            this.checkedListBox2.Size = new System.Drawing.Size(200, 679);
            this.checkedListBox2.TabIndex = 3;
            // 
            // checkedListBox3
            // 
            this.checkedListBox3.FormattingEnabled = true;
            this.checkedListBox3.Location = new System.Drawing.Point(499, 67);
            this.checkedListBox3.Name = "checkedListBox3";
            this.checkedListBox3.Size = new System.Drawing.Size(200, 679);
            this.checkedListBox3.TabIndex = 4;
            // 
            // pgTextBox
            // 
            this.pgTextBox.Location = new System.Drawing.Point(263, 37);
            this.pgTextBox.Name = "pgTextBox";
            this.pgTextBox.Size = new System.Drawing.Size(102, 20);
            this.pgTextBox.TabIndex = 5;
            this.pgTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.pgTextBox_KeyPress);
            // 
            // rangeTextBox
            // 
            this.rangeTextBox.Location = new System.Drawing.Point(499, 37);
            this.rangeTextBox.Name = "rangeTextBox";
            this.rangeTextBox.Size = new System.Drawing.Size(102, 20);
            this.rangeTextBox.TabIndex = 6;
            this.rangeTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.rangeTextBox_KeyPress);
            // 
            // addPG_Button
            // 
            this.addPG_Button.Location = new System.Drawing.Point(388, 35);
            this.addPG_Button.Name = "addPG_Button";
            this.addPG_Button.Size = new System.Drawing.Size(75, 23);
            this.addPG_Button.TabIndex = 7;
            this.addPG_Button.Text = "ADD";
            this.addPG_Button.UseVisualStyleBackColor = true;
            this.addPG_Button.Click += new System.EventHandler(this.addPG_Button_Click);
            // 
            // addRange_Button
            // 
            this.addRange_Button.Location = new System.Drawing.Point(624, 35);
            this.addRange_Button.Name = "addRange_Button";
            this.addRange_Button.Size = new System.Drawing.Size(75, 23);
            this.addRange_Button.TabIndex = 8;
            this.addRange_Button.Text = "ADD";
            this.addRange_Button.UseVisualStyleBackColor = true;
            this.addRange_Button.Click += new System.EventHandler(this.addRange_Button_Click);
            // 
            // allBranchesCheckBox
            // 
            this.allBranchesCheckBox.AutoSize = true;
            this.allBranchesCheckBox.Location = new System.Drawing.Point(26, 14);
            this.allBranchesCheckBox.Name = "allBranchesCheckBox";
            this.allBranchesCheckBox.Size = new System.Drawing.Size(118, 17);
            this.allBranchesCheckBox.TabIndex = 9;
            this.allBranchesCheckBox.Text = "Select All Branches";
            this.allBranchesCheckBox.UseVisualStyleBackColor = true;
            this.allBranchesCheckBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // removeButton
            // 
            this.removeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removeButton.Location = new System.Drawing.Point(634, 784);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(65, 64);
            this.removeButton.TabIndex = 10;
            this.removeButton.Text = "Remove Selected";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // StockLevelling_New
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 851);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.allBranchesCheckBox);
            this.Controls.Add(this.addRange_Button);
            this.Controls.Add(this.addPG_Button);
            this.Controls.Add(this.rangeTextBox);
            this.Controls.Add(this.pgTextBox);
            this.Controls.Add(this.checkedListBox3);
            this.Controls.Add(this.checkedListBox2);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.closeButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StockLevelling_New";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.CheckedListBox checkedListBox2;
        private System.Windows.Forms.CheckedListBox checkedListBox3;
        private System.Windows.Forms.TextBox pgTextBox;
        private System.Windows.Forms.TextBox rangeTextBox;
        private System.Windows.Forms.Button addPG_Button;
        private System.Windows.Forms.Button addRange_Button;
        private System.Windows.Forms.CheckBox allBranchesCheckBox;
        private System.Windows.Forms.Button removeButton;
    }
}