namespace QueryBuilder
{
    partial class QueryBuilder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryBuilder));
            this.SelectTextBox = new System.Windows.Forms.TextBox();
            this.SelectInputTextBox = new System.Windows.Forms.TextBox();
            this.FromTextBox = new System.Windows.Forms.TextBox();
            this.FromComboBox = new System.Windows.Forms.ComboBox();
            this.GroupByTextBox = new System.Windows.Forms.TextBox();
            this.GroupByInputText = new System.Windows.Forms.TextBox();
            this.GroupByCheckBox = new System.Windows.Forms.CheckBox();
            this.QuitButton = new System.Windows.Forms.Button();
            this.SearchButton = new System.Windows.Forms.Button();
            this.OrderByCheckBox = new System.Windows.Forms.CheckBox();
            this.OrderByTextBox = new System.Windows.Forms.TextBox();
            this.OrderByInputTextBox = new System.Windows.Forms.TextBox();
            this.WhereTextBox = new System.Windows.Forms.TextBox();
            this.WhereInputText = new System.Windows.Forms.TextBox();
            this.WhereCheckBox = new System.Windows.Forms.CheckBox();
            this.QueryString = new System.Windows.Forms.TextBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ColumnNamesComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // SelectTextBox
            // 
            this.SelectTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectTextBox.Location = new System.Drawing.Point(12, 12);
            this.SelectTextBox.Name = "SelectTextBox";
            this.SelectTextBox.ReadOnly = true;
            this.SelectTextBox.Size = new System.Drawing.Size(70, 20);
            this.SelectTextBox.TabIndex = 99;
            this.SelectTextBox.Text = "SELECT";
            // 
            // SelectInputTextBox
            // 
            this.SelectInputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectInputTextBox.Location = new System.Drawing.Point(88, 12);
            this.SelectInputTextBox.Name = "SelectInputTextBox";
            this.SelectInputTextBox.Size = new System.Drawing.Size(700, 20);
            this.SelectInputTextBox.TabIndex = 1;
            // 
            // FromTextBox
            // 
            this.FromTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FromTextBox.Location = new System.Drawing.Point(12, 38);
            this.FromTextBox.Name = "FromTextBox";
            this.FromTextBox.ReadOnly = true;
            this.FromTextBox.Size = new System.Drawing.Size(70, 20);
            this.FromTextBox.TabIndex = 100;
            this.FromTextBox.Text = "FROM";
            // 
            // FromComboBox
            // 
            this.FromComboBox.DropDownHeight = 200;
            this.FromComboBox.FormattingEnabled = true;
            this.FromComboBox.IntegralHeight = false;
            this.FromComboBox.Location = new System.Drawing.Point(88, 38);
            this.FromComboBox.Name = "FromComboBox";
            this.FromComboBox.Size = new System.Drawing.Size(221, 21);
            this.FromComboBox.TabIndex = 2;
            this.FromComboBox.TextChanged += new System.EventHandler(this.FromComboBox_TextChanged);
            // 
            // GroupByTextBox
            // 
            this.GroupByTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupByTextBox.Location = new System.Drawing.Point(12, 136);
            this.GroupByTextBox.Name = "GroupByTextBox";
            this.GroupByTextBox.ReadOnly = true;
            this.GroupByTextBox.Size = new System.Drawing.Size(70, 20);
            this.GroupByTextBox.TabIndex = 101;
            this.GroupByTextBox.Text = "GROUP BY";
            this.GroupByTextBox.Visible = false;
            // 
            // GroupByInputText
            // 
            this.GroupByInputText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupByInputText.Location = new System.Drawing.Point(88, 136);
            this.GroupByInputText.Name = "GroupByInputText";
            this.GroupByInputText.Size = new System.Drawing.Size(700, 20);
            this.GroupByInputText.TabIndex = 6;
            this.GroupByInputText.Visible = false;
            // 
            // GroupByCheckBox
            // 
            this.GroupByCheckBox.AutoSize = true;
            this.GroupByCheckBox.Location = new System.Drawing.Point(12, 113);
            this.GroupByCheckBox.Name = "GroupByCheckBox";
            this.GroupByCheckBox.Size = new System.Drawing.Size(70, 17);
            this.GroupByCheckBox.TabIndex = 5;
            this.GroupByCheckBox.Text = "Group By";
            this.GroupByCheckBox.UseVisualStyleBackColor = true;
            this.GroupByCheckBox.Visible = false;
            this.GroupByCheckBox.CheckedChanged += new System.EventHandler(this.GroupByCheckBox_CheckedChanged);
            // 
            // QuitButton
            // 
            this.QuitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.QuitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QuitButton.Location = new System.Drawing.Point(12, 255);
            this.QuitButton.Name = "QuitButton";
            this.QuitButton.Size = new System.Drawing.Size(64, 64);
            this.QuitButton.TabIndex = 10;
            this.QuitButton.Text = "Quit";
            this.QuitButton.UseVisualStyleBackColor = true;
            this.QuitButton.Click += new System.EventHandler(this.QuitButton_Click);
            // 
            // SearchButton
            // 
            this.SearchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SearchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchButton.Location = new System.Drawing.Point(82, 255);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(64, 64);
            this.SearchButton.TabIndex = 9;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // OrderByCheckBox
            // 
            this.OrderByCheckBox.AutoSize = true;
            this.OrderByCheckBox.Location = new System.Drawing.Point(12, 162);
            this.OrderByCheckBox.Name = "OrderByCheckBox";
            this.OrderByCheckBox.Size = new System.Drawing.Size(67, 17);
            this.OrderByCheckBox.TabIndex = 7;
            this.OrderByCheckBox.Text = "Order By";
            this.OrderByCheckBox.UseVisualStyleBackColor = true;
            this.OrderByCheckBox.Visible = false;
            this.OrderByCheckBox.CheckedChanged += new System.EventHandler(this.OrderByCheckBox_CheckedChanged);
            // 
            // OrderByTextBox
            // 
            this.OrderByTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrderByTextBox.Location = new System.Drawing.Point(12, 185);
            this.OrderByTextBox.Name = "OrderByTextBox";
            this.OrderByTextBox.ReadOnly = true;
            this.OrderByTextBox.Size = new System.Drawing.Size(70, 20);
            this.OrderByTextBox.TabIndex = 103;
            this.OrderByTextBox.Text = "ORDER BY";
            this.OrderByTextBox.Visible = false;
            // 
            // OrderByInputTextBox
            // 
            this.OrderByInputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OrderByInputTextBox.Location = new System.Drawing.Point(88, 185);
            this.OrderByInputTextBox.Name = "OrderByInputTextBox";
            this.OrderByInputTextBox.Size = new System.Drawing.Size(700, 20);
            this.OrderByInputTextBox.TabIndex = 8;
            this.OrderByInputTextBox.Visible = false;
            // 
            // WhereTextBox
            // 
            this.WhereTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WhereTextBox.Location = new System.Drawing.Point(12, 87);
            this.WhereTextBox.Name = "WhereTextBox";
            this.WhereTextBox.ReadOnly = true;
            this.WhereTextBox.Size = new System.Drawing.Size(70, 20);
            this.WhereTextBox.TabIndex = 105;
            this.WhereTextBox.Text = "WHERE";
            this.WhereTextBox.Visible = false;
            // 
            // WhereInputText
            // 
            this.WhereInputText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WhereInputText.Location = new System.Drawing.Point(88, 87);
            this.WhereInputText.Name = "WhereInputText";
            this.WhereInputText.Size = new System.Drawing.Size(700, 20);
            this.WhereInputText.TabIndex = 4;
            this.WhereInputText.Visible = false;
            // 
            // WhereCheckBox
            // 
            this.WhereCheckBox.AutoSize = true;
            this.WhereCheckBox.Location = new System.Drawing.Point(12, 64);
            this.WhereCheckBox.Name = "WhereCheckBox";
            this.WhereCheckBox.Size = new System.Drawing.Size(58, 17);
            this.WhereCheckBox.TabIndex = 3;
            this.WhereCheckBox.Text = "Where";
            this.WhereCheckBox.UseVisualStyleBackColor = true;
            this.WhereCheckBox.CheckedChanged += new System.EventHandler(this.WhereCheckBox_CheckedChanged);
            // 
            // QueryString
            // 
            this.QueryString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.QueryString.Location = new System.Drawing.Point(12, 223);
            this.QueryString.Name = "QueryString";
            this.QueryString.Size = new System.Drawing.Size(776, 20);
            this.QueryString.TabIndex = 106;
            this.QueryString.Visible = false;
            // 
            // saveButton
            // 
            this.saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.Location = new System.Drawing.Point(152, 255);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(64, 64);
            this.saveButton.TabIndex = 107;
            this.saveButton.Text = "Save Query";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(222, 255);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(64, 64);
            this.button2.TabIndex = 108;
            this.button2.Text = "Recover saved Query";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.recoverQuery_Click);
            // 
            // ColumnNamesComboBox
            // 
            this.ColumnNamesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ColumnNamesComboBox.Enabled = false;
            this.ColumnNamesComboBox.FormattingEnabled = true;
            this.ColumnNamesComboBox.Location = new System.Drawing.Point(338, 39);
            this.ColumnNamesComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.ColumnNamesComboBox.Name = "ColumnNamesComboBox";
            this.ColumnNamesComboBox.Size = new System.Drawing.Size(174, 21);
            this.ColumnNamesComboBox.TabIndex = 109;
            this.ColumnNamesComboBox.SelectedIndexChanged += new System.EventHandler(this.ColumnNamesComboBox_SelectedIndexChanged);
            // 
            // QueryBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 331);
            this.Controls.Add(this.ColumnNamesComboBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.QueryString);
            this.Controls.Add(this.WhereCheckBox);
            this.Controls.Add(this.WhereInputText);
            this.Controls.Add(this.WhereTextBox);
            this.Controls.Add(this.OrderByInputTextBox);
            this.Controls.Add(this.OrderByTextBox);
            this.Controls.Add(this.OrderByCheckBox);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.QuitButton);
            this.Controls.Add(this.GroupByCheckBox);
            this.Controls.Add(this.GroupByInputText);
            this.Controls.Add(this.GroupByTextBox);
            this.Controls.Add(this.FromComboBox);
            this.Controls.Add(this.FromTextBox);
            this.Controls.Add(this.SelectInputTextBox);
            this.Controls.Add(this.SelectTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "QueryBuilder";
            this.Text = "Query Builder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SelectTextBox;
        private System.Windows.Forms.TextBox SelectInputTextBox;
        private System.Windows.Forms.TextBox FromTextBox;
        private System.Windows.Forms.ComboBox FromComboBox;
        private System.Windows.Forms.TextBox GroupByTextBox;
        private System.Windows.Forms.TextBox GroupByInputText;
        private System.Windows.Forms.CheckBox GroupByCheckBox;
        private System.Windows.Forms.Button QuitButton;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.CheckBox OrderByCheckBox;
        private System.Windows.Forms.TextBox OrderByTextBox;
        private System.Windows.Forms.TextBox OrderByInputTextBox;
        private System.Windows.Forms.TextBox WhereTextBox;
        private System.Windows.Forms.TextBox WhereInputText;
        private System.Windows.Forms.CheckBox WhereCheckBox;
        private System.Windows.Forms.TextBox QueryString;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox ColumnNamesComboBox;
    }
}