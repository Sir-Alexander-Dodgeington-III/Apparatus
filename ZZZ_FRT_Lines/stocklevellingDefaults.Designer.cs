namespace ZZZ_FRT_Lines
{
    partial class stocklevellingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(stocklevellingForm));
            this.pgListBox = new System.Windows.Forms.ListBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.rangeListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pgTextBox = new System.Windows.Forms.TextBox();
            this.rngTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // pgListBox
            // 
            this.pgListBox.FormattingEnabled = true;
            this.pgListBox.Location = new System.Drawing.Point(12, 52);
            this.pgListBox.Name = "pgListBox";
            this.pgListBox.Size = new System.Drawing.Size(157, 563);
            this.pgListBox.TabIndex = 0;
            // 
            // closeButton
            // 
            this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.Location = new System.Drawing.Point(12, 634);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(64, 64);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // rangeListBox
            // 
            this.rangeListBox.FormattingEnabled = true;
            this.rangeListBox.Location = new System.Drawing.Point(203, 52);
            this.rangeListBox.Name = "rangeListBox";
            this.rangeListBox.Size = new System.Drawing.Size(161, 563);
            this.rangeListBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Branches not in PG default";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(211, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(153, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Branches not in Range Default";
            // 
            // pgTextBox
            // 
            this.pgTextBox.Location = new System.Drawing.Point(96, 634);
            this.pgTextBox.Name = "pgTextBox";
            this.pgTextBox.Size = new System.Drawing.Size(268, 20);
            this.pgTextBox.TabIndex = 5;
            this.pgTextBox.Visible = false;
            // 
            // rngTextBox
            // 
            this.rngTextBox.Location = new System.Drawing.Point(96, 660);
            this.rngTextBox.Name = "rngTextBox";
            this.rngTextBox.Size = new System.Drawing.Size(268, 20);
            this.rngTextBox.TabIndex = 6;
            this.rngTextBox.Visible = false;
            // 
            // stocklevellingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 713);
            this.Controls.Add(this.rngTextBox);
            this.Controls.Add(this.pgTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rangeListBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.pgListBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "stocklevellingForm";
            this.Text = "Stock Levelling Defaults";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox pgListBox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ListBox rangeListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox pgTextBox;
        private System.Windows.Forms.TextBox rngTextBox;
    }
}