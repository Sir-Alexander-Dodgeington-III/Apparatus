using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BranchRebates
{
    public partial class BranchRebates_Form : Form
    {

        public DataTable table = new DataTable();

        public BranchRebates_Form()
        {
            InitializeComponent();
            this.CenterToScreen();
        }


        private void SearchButton_Click(object sender, EventArgs e)
        {
            table.Clear();

            pgTextBox.Text =pgTextBox.Text.ToUpper();
            branchTextBox.Text = branchTextBox.Text.ToUpper();

            if (String.IsNullOrEmpty(acctTextBox.Text))
            {
                MessageBox.Show("The Account search criteria can not be left empty!");
                return;
            }


            if (String.IsNullOrEmpty(StartDateTextBox.Text))
            {
                MessageBox.Show("The Start Date search criteria can not be left empty!");
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            string Branch = "'" + branchTextBox.Text + "'";
            string PG = "'" + pgTextBox.Text + "'";
            string Acct = "'" + acctTextBox.Text + "'";
            string StartDate = "'" + StartDateTextBox.Text + "'";
            string EndDate = "'" + EndDateTextBox.Text + "'";

            if (String.IsNullOrEmpty(EndDateTextBox.Text))
            {
                EndDate = "NULL";
                DateTime today = DateTime.Today;
                EndDateTextBox.Text = today.ToString("MM/dd/yyyy");
            }
            if (String.IsNullOrEmpty(pgTextBox.Text))
            {
                PG = "NULL";
                pgTextBox.Text = "ALL";
            }
            if (String.IsNullOrEmpty(branchTextBox.Text))
            {
                Branch = "NULL";
                branchTextBox.Text = "ALL";
            }

            string query = ($@"
                                DECLARE @Branch VARCHAR(5)
                                DECLARE @StartDate Date
                                DECLARE @EndDate Date
                                DECLARE @Acct VARCHAR(10)
                                DECLARE @PG VARCHAR(3)

                                SET @StartDate = {StartDate}
                                SET @EndDate = {EndDate}
                                SET @Acct = {Acct}
                                SET @PG = {PG}
                                SET @Branch = {Branch}


                                SELECT 
                                Branch, 
                                Acct, 
                                Name, 
                                CAST(DateTime as date) as Date, 
                                COrder as PO, 
                                Document, 
                                Part, 
                                Description, 
                                Qty, 
                                Unit, 
                                Ext 
                                FROM VW_MERI_I 
                                WHERE Acct = @Acct AND 
                                PG = IIF(@PG IS NULL, PG, @PG) AND
                                Branch = IIF(@Branch IS NULL,Branch, @Branch) AND
                                (Cast(DateTime as Date) BETWEEN @StartDate AND IIF(@EndDate IS NULL, GETDATE(), @EndDate))
                            ");


            SqlCommand cmd = new SqlCommand(query, conn);

            using (SqlDataAdapter a = new SqlDataAdapter(cmd))
            {
                //add row number
                //table.Columns.Add("#", typeof(int));
                //table.Columns[0].AutoIncrement = true;
                //table.Columns[0].AutoIncrementSeed = 1;
                //table.Columns[0].AutoIncrementStep = 1;
                //
                a.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            }
            Cursor.Current = Cursors.Default;

            dataGridView1.MouseClick += new MouseEventHandler(dataGridView1_MouseClick);
        }

        void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip my_Menu = new System.Windows.Forms.ContextMenuStrip();
                int Mouse_XY_Position = dataGridView1.HitTest(e.X, e.Y).RowIndex;

                if (Mouse_XY_Position >= 0)
                {
                    my_Menu.Items.Add("Save as CSV").Name = "Save as CSV";
                    my_Menu.Items.Add("Select All").Name = "Select All";
                    my_Menu.Items.Add("Copy").Name = "Copy";
                    my_Menu.Items.Add("Copy with headers").Name = "Copy with headers";
                }

                my_Menu.Show(dataGridView1, new Point(e.X, e.Y));
                my_Menu.AutoClose = true;
                my_Menu.ItemClicked += new ToolStripItemClickedEventHandler(my_Menu_ItemClicked);
            }
        }

        void my_Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            switch (e.ClickedItem.Name.ToString())
            {
                case "Save as CSV":

                    int x = 0;
                    string username1 = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    int SLength = username1.Length;
                    string username = username1.Substring(10, (username1.Length - 10));
                    string currFile = @"C:\Users\" + username + @"\Documents\MyFile.csv";

                    if (File.Exists(currFile))
                    {
                        do
                        {
                            x++;
                            currFile = @"C:\Users\" + username + @"\Documents\MyFile" + x + ".csv";
                        } while (File.Exists(currFile));
                    }

                    Cursor.Current = Cursors.WaitCursor;

                    StringBuilder sb = new StringBuilder();

                    IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().
                                                      Select(column => column.ColumnName);
                    sb.AppendLine(string.Join(",", columnNames));

                    foreach (DataRow row in table.Rows)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sb.AppendLine(string.Join(",", fields));
                    }

                    File.WriteAllText(currFile, sb.ToString());
                    MessageBox.Show("Saved to " + currFile);
                    Cursor.Current = Cursors.Default;
                    break;

                case "Select All":
                    dataGridView1.SelectAll();
                    break;

                case "Copy":
                    if (this.dataGridView1.GetCellCount(DataGridViewElementStates.Selected) > 0)
                    {
                        dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
                        Clipboard.SetDataObject(this.dataGridView1.GetClipboardContent());
                    }
                    break;

                case "Copy with headers":
                    if (this.dataGridView1.GetCellCount(DataGridViewElementStates.Selected) > 0)
                    {
                        dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;
                        Clipboard.SetDataObject(this.dataGridView1.GetClipboardContent());
                    }
                    break;
            }
        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveAsButton_Click(object sender, EventArgs e)
        {
            int x = 0;
            string username1 = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            int SLength = username1.Length;
            string username = username1.Substring(10, (username1.Length - 10));
            string currFile = @"C:\Users\" + username + @"\Documents\MyFile.csv";

            if (File.Exists(currFile))
            {
                do
                {
                    x++;
                    currFile = @"C:\Users\" + username + @"\Documents\MyFile" + x + ".csv";
                } while (File.Exists(currFile));
            }

            Cursor.Current = Cursors.WaitCursor;

            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().
                                                Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in table.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(currFile, sb.ToString());
            MessageBox.Show("Saved to " + currFile);
            Cursor.Current = Cursors.Default;
        }

    }
}
