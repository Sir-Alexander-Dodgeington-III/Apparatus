using PartPerDay;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PartPerDay_Form
{
    public partial class PartPerDay_Form : Form
    {
        List<ProdlineItems> ProdlineItems = new List<ProdlineItems>();
        public DataTable table = new DataTable();

        public PartPerDay_Form()
        {
            InitializeComponent();
            this.CenterToScreen();
        }


        private void SearchButtom_Click(object sender, EventArgs e)
        {
            table.Clear();

            if(String.IsNullOrEmpty(BranchTextBox.Text))
            {
                MessageBox.Show("The branch search criteria can not be left empty!");
                return;
            }

            if (String.IsNullOrEmpty(StartDateTextBox.Text))
            {
                MessageBox.Show("The Start Date search criteria can not be left empty!");
                return;
            }

            if (StartDateTextBox.Text.Length < 7)
            {
                StartDateTextBox.Text = StartDateTextBox.Text + "/" + DateTime.Now.Year;
            }

            if (EndDateTextBox.Text.Length < 7 && EndDateTextBox.Text != "")
            {
                EndDateTextBox.Text = EndDateTextBox.Text + "/" + DateTime.Now.Year;
            }

            if (String.IsNullOrEmpty(EndDateTextBox.Text))
            {
                EndDateTextBox.Text = DateTime.Now.ToString("MM/dd/yyyy");
            }

            if (String.IsNullOrEmpty(PGTextBox.Text))
            {
                MessageBox.Show("The Product Group search criteria can not be left empty!");
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            string Branch = BranchTextBox.Text;
            string StartDate = StartDateTextBox.Text;
            string EndDate = "'" + EndDateTextBox.Text + "'";
            string PG = PGTextBox.Text;
            string Qty = "'" + QtyGreaterThan_TextBox.Text + "'";
            string Part = "'" + PartTextBox.Text + "'";

            if (String.IsNullOrEmpty(EndDateTextBox.Text))
            {
                EndDate = "NULL";
                DateTime today = DateTime.Today;
                EndDateTextBox.Text = today.ToString("MM/dd/yyyy");
            }

            if (String.IsNullOrEmpty(PartTextBox.Text))
            {
                Part = "NULL";
            }

            if (String.IsNullOrEmpty(QtyGreaterThan_TextBox.Text))
            {
                Qty = "NULL";
            }


            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();

            if (AdjChkBox.Checked)
                {
                string query = ($@"
                                    DECLARE @StartDate DATE
                                    DECLARE @EndDate DATE
                                    DECLARE @Branch VARCHAR(20)
                                    DECLARE @PG VARCHAR(10)
                                    DECLARE @Qty Money
                                    DECLARE @Part VARCHAR(40)

                                    SET @StartDate = '{StartDate}'
                                    SET @EndDate  = {EndDate}
                                    SET @Branch = '{Branch}'
                                    SET @PG = '{PG}'
                                    SET @Qty = {Qty}
                                    SET @Part = {Part}



                                    SELECT A.Prefix, A.Branch, A.PG, A.Part, SUM(A.Qty) as Qty, A.DateTime, A.RC 
                                    FROM ALines as A 
                                    WHERE (Cast(A.DateTime as date) BETWEEN @StartDate AND IIF(@EndDate IS NULL, GETDATE(), @EndDate)) AND A.Branch = @Branch AND A.PG = @PG AND A.Qty > IIF(@Qty IS NULL, -1000000, @Qty) AND A.Prefix <> 'X' AND A.Part = IIF(@Part IS NULL, Part, @Part)
                                    Group By A.Branch, A.Part, A.PG, A.DateTime, A.Prefix, A.RC, A.Part 
                                    UNION 
                                    SELECT I.Prefix, I.Branch, I.PG, I.Part, SUM(I.Qty) as Qty, I.DateTime, '' as RC 
                                    FROM VW_MERI_I as I 
                                    WHERE (Cast(I.DateTime as date) BETWEEN @StartDate AND IIF(@EndDate IS NULL, GETDATE(), @EndDate)) AND I.Branch = @Branch AND I.PG = @PG AND I.Qty > IIF(@Qty IS NULL, -1000000, @Qty) AND I.Part = IIF(@Part IS NULL, Part, @Part)
                                    Group By I.Branch, I.Part, I.PG, I.DateTime, I.Prefix , I.PArt
                                    ORDER BY DateTime
                                    ");
                SqlCommand cmd = new SqlCommand(query, conn);
                using (SqlDataAdapter a = new SqlDataAdapter(cmd))
                {
                    a.Fill(table);
                    dataGridView1.DataSource = table;
                    dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

                }
                Cursor.Current = Cursors.Default;
            }
                else
                {
                string query = ($@"
                                    DECLARE @StartDate DATE
                                    DECLARE @EndDate DATE
                                    DECLARE @Branch VARCHAR(20)
                                    DECLARE @PG VARCHAR(10)
                                    DECLARE @Qty Money
                                    DECLARE @Part VARCHAR(40)

                                    SET @StartDate = '{StartDate}'
                                    SET @EndDate  = {EndDate}
                                    SET @Branch = '{Branch}'
                                    SET @PG = '{PG}'
                                    SET @Qty = {Qty}
                                    SET @Part = {Part}

                                    SELECT I.Prefix, I.Branch, I.PG, I.Part, SUM(I.Qty) as Qty, I.DateTime, '' as RC 
                                    FROM VW_MERI_I as I 
                                    WHERE (Cast(I.DateTime as date) BETWEEN @StartDate AND IIF(@EndDate IS NULL, GETDATE(), @EndDate)) AND I.Branch = @Branch AND I.PG = @PG AND I.Qty > IIF(@Qty IS NULL, -1000000, @Qty) AND I.Part = IIF(@Part IS NULL, Part, @Part)
                                    Group By I.Branch, I.Part, I.PG, I.DateTime, I.Prefix , I.PArt
                                    ORDER BY DateTime
                                ");

                SqlCommand cmd = new SqlCommand(query, conn);
                using (SqlDataAdapter a = new SqlDataAdapter(cmd))
                {
                    a.Fill(table);
                    dataGridView1.DataSource = table;
                    dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

                }
                Cursor.Current = Cursors.Default;
            }

            if (string.IsNullOrEmpty(PartTextBox.Text))
            {
                PartTextBox.Text = "ALL";
            }

            if (QtyGreaterThan_TextBox.Text == "-100000")
            {
                QtyGreaterThan_TextBox.Text = "";
                QtyGreaterThan_TextBox.Visible = true;
            }

            Cursor.Current = Cursors.Default;
            dataGridView1.MouseClick += new MouseEventHandler(dataGridView1_MouseClick);
        }


        void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip my_Menu = new ContextMenuStrip();
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


        private void QuitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SaveButton_Click(object sender, EventArgs e)
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
