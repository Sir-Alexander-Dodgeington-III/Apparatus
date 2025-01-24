namespace Fuse5Export
{
    partial class Fuse5Export_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fuse5Export_Form));
            this.addCustomerButton = new System.Windows.Forms.Button();
            this.removeCustomersButton = new System.Windows.Forms.Button();
            this.PGsAddButton = new System.Windows.Forms.Button();
            this.PGsRemoveButton = new System.Windows.Forms.Button();
            this.quitButton = new System.Windows.Forms.Button();
            this.ScriptButton = new System.Windows.Forms.Button();
            this.customersTextBox = new System.Windows.Forms.TextBox();
            this.customersListBox = new System.Windows.Forms.ListBox();
            this.PGTextBox = new System.Windows.Forms.TextBox();
            this.PGsListBox = new System.Windows.Forms.ListBox();
            this.finalizeButton = new System.Windows.Forms.Button();
            this.CustomersLabel = new System.Windows.Forms.Label();
            this.ProductGroupLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // addCustomerButton
            // 
            this.addCustomerButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.addCustomerButton.Location = new System.Drawing.Point(45, 219);
            this.addCustomerButton.Name = "addCustomerButton";
            this.addCustomerButton.Size = new System.Drawing.Size(75, 23);
            this.addCustomerButton.TabIndex = 3;
            this.addCustomerButton.Text = "Add";
            this.addCustomerButton.UseVisualStyleBackColor = true;
            this.addCustomerButton.Click += new System.EventHandler(this.addCustomerButton_Click);
            // 
            // removeCustomersButton
            // 
            this.removeCustomersButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.removeCustomersButton.Location = new System.Drawing.Point(126, 219);
            this.removeCustomersButton.Name = "removeCustomersButton";
            this.removeCustomersButton.Size = new System.Drawing.Size(75, 23);
            this.removeCustomersButton.TabIndex = 4;
            this.removeCustomersButton.Text = "Remove";
            this.removeCustomersButton.UseVisualStyleBackColor = true;
            this.removeCustomersButton.Click += new System.EventHandler(this.removeCustomersButton_Click);
            // 
            // PGsAddButton
            // 
            this.PGsAddButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PGsAddButton.Location = new System.Drawing.Point(207, 219);
            this.PGsAddButton.Name = "PGsAddButton";
            this.PGsAddButton.Size = new System.Drawing.Size(75, 23);
            this.PGsAddButton.TabIndex = 5;
            this.PGsAddButton.Text = "Add";
            this.PGsAddButton.UseVisualStyleBackColor = true;
            this.PGsAddButton.Click += new System.EventHandler(this.PGsAddButton_Click);
            // 
            // PGsRemoveButton
            // 
            this.PGsRemoveButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PGsRemoveButton.Location = new System.Drawing.Point(288, 219);
            this.PGsRemoveButton.Name = "PGsRemoveButton";
            this.PGsRemoveButton.Size = new System.Drawing.Size(75, 23);
            this.PGsRemoveButton.TabIndex = 6;
            this.PGsRemoveButton.Text = "Remove";
            this.PGsRemoveButton.UseVisualStyleBackColor = true;
            this.PGsRemoveButton.Click += new System.EventHandler(this.PGsRemoveButton_Click);
            // 
            // quitButton
            // 
            this.quitButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.quitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.quitButton.Location = new System.Drawing.Point(45, 278);
            this.quitButton.Name = "quitButton";
            this.quitButton.Size = new System.Drawing.Size(64, 64);
            this.quitButton.TabIndex = 9;
            this.quitButton.Text = "Quit";
            this.quitButton.UseVisualStyleBackColor = true;
            this.quitButton.Click += new System.EventHandler(this.quitButton_Click);
            // 
            // ScriptButton
            // 
            this.ScriptButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ScriptButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScriptButton.Location = new System.Drawing.Point(115, 278);
            this.ScriptButton.Name = "ScriptButton";
            this.ScriptButton.Size = new System.Drawing.Size(64, 64);
            this.ScriptButton.TabIndex = 7;
            this.ScriptButton.Text = "Script";
            this.ScriptButton.UseVisualStyleBackColor = true;
            this.ScriptButton.Click += new System.EventHandler(this.ScriptButton_Click);
            // 
            // customersTextBox
            // 
            this.customersTextBox.Location = new System.Drawing.Point(45, 66);
            this.customersTextBox.Name = "customersTextBox";
            this.customersTextBox.Size = new System.Drawing.Size(156, 20);
            this.customersTextBox.TabIndex = 1;
            this.customersTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.customersTextBox_KeyPress);
            // 
            // customersListBox
            // 
            this.customersListBox.FormattingEnabled = true;
            this.customersListBox.Location = new System.Drawing.Point(45, 92);
            this.customersListBox.Name = "customersListBox";
            this.customersListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.customersListBox.Size = new System.Drawing.Size(156, 121);
            this.customersListBox.TabIndex = 11;
            // 
            // PGTextBox
            // 
            this.PGTextBox.Location = new System.Drawing.Point(207, 66);
            this.PGTextBox.Name = "PGTextBox";
            this.PGTextBox.Size = new System.Drawing.Size(156, 20);
            this.PGTextBox.TabIndex = 2;
            this.PGTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PGTextBox_KeyPress);
            // 
            // PGsListBox
            // 
            this.PGsListBox.FormattingEnabled = true;
            this.PGsListBox.Location = new System.Drawing.Point(207, 92);
            this.PGsListBox.Name = "PGsListBox";
            this.PGsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.PGsListBox.Size = new System.Drawing.Size(156, 121);
            this.PGsListBox.TabIndex = 12;
            // 
            // finalizeButton
            // 
            this.finalizeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.finalizeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.finalizeButton.Location = new System.Drawing.Point(185, 278);
            this.finalizeButton.Name = "finalizeButton";
            this.finalizeButton.Size = new System.Drawing.Size(64, 64);
            this.finalizeButton.TabIndex = 8;
            this.finalizeButton.Text = "Finalize";
            this.finalizeButton.UseVisualStyleBackColor = true;
            this.finalizeButton.Click += new System.EventHandler(this.finalizeButton_Click);
            // 
            // CustomersLabel
            // 
            this.CustomersLabel.AutoSize = true;
            this.CustomersLabel.Location = new System.Drawing.Point(98, 50);
            this.CustomersLabel.Name = "CustomersLabel";
            this.CustomersLabel.Size = new System.Drawing.Size(56, 13);
            this.CustomersLabel.TabIndex = 11;
            this.CustomersLabel.Text = "Customers";
            // 
            // ProductGroupLabel
            // 
            this.ProductGroupLabel.AutoSize = true;
            this.ProductGroupLabel.Location = new System.Drawing.Point(265, 50);
            this.ProductGroupLabel.Name = "ProductGroupLabel";
            this.ProductGroupLabel.Size = new System.Drawing.Size(27, 13);
            this.ProductGroupLabel.TabIndex = 12;
            this.ProductGroupLabel.Text = "PGs";
            // 
            // Fuse5Export_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 354);
            this.Controls.Add(this.ProductGroupLabel);
            this.Controls.Add(this.CustomersLabel);
            this.Controls.Add(this.finalizeButton);
            this.Controls.Add(this.PGsListBox);
            this.Controls.Add(this.PGTextBox);
            this.Controls.Add(this.customersListBox);
            this.Controls.Add(this.customersTextBox);
            this.Controls.Add(this.ScriptButton);
            this.Controls.Add(this.quitButton);
            this.Controls.Add(this.PGsRemoveButton);
            this.Controls.Add(this.PGsAddButton);
            this.Controls.Add(this.removeCustomersButton);
            this.Controls.Add(this.addCustomerButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Fuse5Export_Form";
            this.Text = "Fuse5 Export";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addCustomerButton;
        private System.Windows.Forms.Button removeCustomersButton;
        private System.Windows.Forms.Button PGsAddButton;
        private System.Windows.Forms.Button PGsRemoveButton;
        private System.Windows.Forms.Button quitButton;
        private System.Windows.Forms.Button ScriptButton;
        private System.Windows.Forms.TextBox customersTextBox;
        private System.Windows.Forms.ListBox customersListBox;
        private System.Windows.Forms.TextBox PGTextBox;
        private System.Windows.Forms.ListBox PGsListBox;
        private System.Windows.Forms.Button finalizeButton;
        private System.Windows.Forms.Label CustomersLabel;
        private System.Windows.Forms.Label ProductGroupLabel;
    }
}