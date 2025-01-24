
namespace BranchTransfer
{
    partial class BranchTransfer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BranchTransfer));
            this.acctTextbox = new System.Windows.Forms.TextBox();
            this.startDateTextbox = new System.Windows.Forms.TextBox();
            this.endDateTextbox = new System.Windows.Forms.TextBox();
            this.acctLabel = new System.Windows.Forms.Label();
            this.startDateLabel = new System.Windows.Forms.Label();
            this.endDateLabel = new System.Windows.Forms.Label();
            this.canelButton = new System.Windows.Forms.Button();
            this.goButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // acctTextbox
            // 
            this.acctTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.acctTextbox.Location = new System.Drawing.Point(141, 34);
            this.acctTextbox.Name = "acctTextbox";
            this.acctTextbox.Size = new System.Drawing.Size(152, 21);
            this.acctTextbox.TabIndex = 0;
            this.acctTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.acctTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // startDateTextbox
            // 
            this.startDateTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startDateTextbox.Location = new System.Drawing.Point(141, 88);
            this.startDateTextbox.Name = "startDateTextbox";
            this.startDateTextbox.Size = new System.Drawing.Size(152, 21);
            this.startDateTextbox.TabIndex = 1;
            this.startDateTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.startDateTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // endDateTextbox
            // 
            this.endDateTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.endDateTextbox.Location = new System.Drawing.Point(141, 147);
            this.endDateTextbox.Name = "endDateTextbox";
            this.endDateTextbox.Size = new System.Drawing.Size(152, 21);
            this.endDateTextbox.TabIndex = 2;
            this.endDateTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.endDateTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // acctLabel
            // 
            this.acctLabel.AutoSize = true;
            this.acctLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.acctLabel.Location = new System.Drawing.Point(30, 37);
            this.acctLabel.Name = "acctLabel";
            this.acctLabel.Size = new System.Drawing.Size(61, 15);
            this.acctLabel.TabIndex = 4;
            this.acctLabel.Text = "Account:";
            // 
            // startDateLabel
            // 
            this.startDateLabel.AutoSize = true;
            this.startDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startDateLabel.Location = new System.Drawing.Point(30, 91);
            this.startDateLabel.Name = "startDateLabel";
            this.startDateLabel.Size = new System.Drawing.Size(75, 15);
            this.startDateLabel.TabIndex = 5;
            this.startDateLabel.Text = "Start Date:";
            // 
            // endDateLabel
            // 
            this.endDateLabel.AutoSize = true;
            this.endDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.endDateLabel.Location = new System.Drawing.Point(30, 150);
            this.endDateLabel.Name = "endDateLabel";
            this.endDateLabel.Size = new System.Drawing.Size(66, 15);
            this.endDateLabel.TabIndex = 6;
            this.endDateLabel.Text = "End Date";
            // 
            // canelButton
            // 
            this.canelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.canelButton.Location = new System.Drawing.Point(12, 212);
            this.canelButton.Name = "canelButton";
            this.canelButton.Size = new System.Drawing.Size(64, 64);
            this.canelButton.TabIndex = 8;
            this.canelButton.Text = "Cancel";
            this.canelButton.UseVisualStyleBackColor = true;
            this.canelButton.Click += new System.EventHandler(this.canelButton_Click);
            // 
            // goButton
            // 
            this.goButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goButton.Location = new System.Drawing.Point(82, 212);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(64, 64);
            this.goButton.TabIndex = 9;
            this.goButton.Text = "GO";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // BranchTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 288);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.canelButton);
            this.Controls.Add(this.endDateLabel);
            this.Controls.Add(this.startDateLabel);
            this.Controls.Add(this.acctLabel);
            this.Controls.Add(this.endDateTextbox);
            this.Controls.Add(this.startDateTextbox);
            this.Controls.Add(this.acctTextbox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BranchTransfer";
            this.Text = "Branch Transfers";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox acctTextbox;
        private System.Windows.Forms.TextBox startDateTextbox;
        private System.Windows.Forms.TextBox endDateTextbox;
        private System.Windows.Forms.Label acctLabel;
        private System.Windows.Forms.Label startDateLabel;
        private System.Windows.Forms.Label endDateLabel;
        private System.Windows.Forms.Button canelButton;
        private System.Windows.Forms.Button goButton;
    }
}