namespace TimesToZero_Form
{
    partial class timesToZero
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(timesToZero));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.PGTextbox = new System.Windows.Forms.TextBox();
            this.partNumberTextbox = new System.Windows.Forms.TextBox();
            this.branchTextbox = new System.Windows.Forms.TextBox();
            this.startDateTextbox = new System.Windows.Forms.TextBox();
            this.endDateTextbox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(29, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "PG";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(29, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Part";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(29, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Branch";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(29, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Start Date";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(29, 205);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "End Date";
            // 
            // PGTextbox
            // 
            this.PGTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PGTextbox.Location = new System.Drawing.Point(115, 26);
            this.PGTextbox.Name = "PGTextbox";
            this.PGTextbox.Size = new System.Drawing.Size(100, 22);
            this.PGTextbox.TabIndex = 5;
            this.PGTextbox.Text = "ALL";
            this.PGTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.PGTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.PGTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // partNumberTextbox
            // 
            this.partNumberTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.partNumberTextbox.Location = new System.Drawing.Point(115, 70);
            this.partNumberTextbox.Name = "partNumberTextbox";
            this.partNumberTextbox.Size = new System.Drawing.Size(100, 22);
            this.partNumberTextbox.TabIndex = 6;
            this.partNumberTextbox.Text = "ALL";
            this.partNumberTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.partNumberTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.partNumberTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // branchTextbox
            // 
            this.branchTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.branchTextbox.Location = new System.Drawing.Point(115, 114);
            this.branchTextbox.Name = "branchTextbox";
            this.branchTextbox.Size = new System.Drawing.Size(100, 22);
            this.branchTextbox.TabIndex = 7;
            this.branchTextbox.Text = "ALL";
            this.branchTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.branchTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.branchTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // startDateTextbox
            // 
            this.startDateTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startDateTextbox.Location = new System.Drawing.Point(115, 158);
            this.startDateTextbox.Name = "startDateTextbox";
            this.startDateTextbox.Size = new System.Drawing.Size(100, 22);
            this.startDateTextbox.TabIndex = 8;
            this.startDateTextbox.Text = "ALL";
            this.startDateTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.startDateTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.startDateTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // endDateTextbox
            // 
            this.endDateTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.endDateTextbox.Location = new System.Drawing.Point(115, 202);
            this.endDateTextbox.Name = "endDateTextbox";
            this.endDateTextbox.Size = new System.Drawing.Size(100, 22);
            this.endDateTextbox.TabIndex = 9;
            this.endDateTextbox.Text = "ALL";
            this.endDateTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.endDateTextbox.Enter += new System.EventHandler(this.colorActiveTextbox_Enter);
            this.endDateTextbox.Leave += new System.EventHandler(this.colorActiveTextbox_Leave);
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(12, 254);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(64, 64);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Quit";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(82, 254);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(64, 64);
            this.okButton.TabIndex = 11;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // timesToZero
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 334);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.endDateTextbox);
            this.Controls.Add(this.startDateTextbox);
            this.Controls.Add(this.branchTextbox);
            this.Controls.Add(this.partNumberTextbox);
            this.Controls.Add(this.PGTextbox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "timesToZero";
            this.Text = "Times To Zero";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox PGTextbox;
        private System.Windows.Forms.TextBox partNumberTextbox;
        private System.Windows.Forms.TextBox branchTextbox;
        private System.Windows.Forms.TextBox startDateTextbox;
        private System.Windows.Forms.TextBox endDateTextbox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}