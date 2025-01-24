namespace Multi_PO
{
    partial class Multi_PO
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Multi_PO));
            this.quitButton = new System.Windows.Forms.Button();
            this.goButton = new System.Windows.Forms.Button();
            this.importFile = new System.Windows.Forms.Button();
            this.defaultSupplierButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // quitButton
            // 
            this.quitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.quitButton.Location = new System.Drawing.Point(12, 473);
            this.quitButton.Name = "quitButton";
            this.quitButton.Size = new System.Drawing.Size(64, 64);
            this.quitButton.TabIndex = 0;
            this.quitButton.Text = "Quit";
            this.quitButton.UseVisualStyleBackColor = true;
            this.quitButton.Click += new System.EventHandler(this.quitButton_Click);
            // 
            // goButton
            // 
            this.goButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goButton.Location = new System.Drawing.Point(82, 473);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(64, 64);
            this.goButton.TabIndex = 1;
            this.goButton.Text = "GO";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // importFile
            // 
            this.importFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.importFile.Location = new System.Drawing.Point(152, 473);
            this.importFile.Name = "importFile";
            this.importFile.Size = new System.Drawing.Size(64, 64);
            this.importFile.TabIndex = 2;
            this.importFile.Text = "Import";
            this.importFile.UseVisualStyleBackColor = true;
            this.importFile.Click += new System.EventHandler(this.importFile_Click);
            // 
            // defaultSupplierButton
            // 
            this.defaultSupplierButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.defaultSupplierButton.Location = new System.Drawing.Point(679, 473);
            this.defaultSupplierButton.Name = "defaultSupplierButton";
            this.defaultSupplierButton.Size = new System.Drawing.Size(64, 64);
            this.defaultSupplierButton.TabIndex = 3;
            this.defaultSupplierButton.Text = "Defaults";
            this.defaultSupplierButton.UseVisualStyleBackColor = true;
            this.defaultSupplierButton.Click += new System.EventHandler(this.defaultSupplierButton_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(0, 1);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(757, 454);
            this.dataGridView1.TabIndex = 4;
            // 
            // Multi_PO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 549);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.defaultSupplierButton);
            this.Controls.Add(this.importFile);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.quitButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Multi_PO";
            this.Text = "Form2";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button quitButton;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.Button importFile;
        private System.Windows.Forms.Button defaultSupplierButton;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}