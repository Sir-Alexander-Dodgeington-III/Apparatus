//using OnHandOnPo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OnHandOnPo_Form
{
    
    public partial class OnHandOnPO_Form : Form

    {
        public DataTable table = new DataTable();

        public OnHandOnPO_Form()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {

            table.Clear();

            PGTextBox.Text = PGTextBox.Text.ToUpper();
            BranchTextBox.Text = BranchTextBox.Text.ToUpper();

            if (String.IsNullOrEmpty(BranchTextBox.Text))
            {
                MessageBox.Show("The branch search criteria can not be left empty!");
                return;
            }


            if (String.IsNullOrEmpty(PGTextBox.Text))
            {
                MessageBox.Show("The Product Group search criteria can not be left empty!");
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            string Branch = BranchTextBox.Text;
            Branch = Branch.Replace(",", "', '");
            Branch = "'" + Branch + "'";
            string PG = PGTextBox.Text;
            string Range = "'" + RangeTextBox.Text + "'";

            if (String.IsNullOrEmpty(RangeTextBox.Text) || RangeTextBox.Text == "ALL")
            {
                Range = "NULL";
                RangeTextBox.Text = "ALL";
            }

            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();
            
            if (ExLocationChkBx.Checked)
            {
                string query = ($@"
                                DECLARE @PG VARCHAR(10)
                                DECLARE @Range VARCHAR(10)

                                SET @PG = '{PG}'
                                SET @Range = {Range}

                                ;WITH CTE (Branch, Part, Description, PG, Range, QtyOnPO, QtyOnPN, [MIN], [MAX], Cind, MNIndicator, WMSSort, MNDate, TUL12) as (
	                                SELECT Branch, Part, Description, PG, Range, QtyOnPO, QtyOnPN, [MIN], [MAX], Cind, MNIndicator, WMSSort30, MNDate, TotalUsageLast12 FROM VW_MERI_Pro
                                ),

                                CTE2 (Branch, Part, QtQty) as (
	                                SELECT Branch, Part, SUM(Qty) FROM Locations WHERE [Location] = 'Qt'
	                                GROUP BY Branch, Part
                                ),

                                CTE3 (Branch, Part, Qty) as (
	                                SELECT Branch, Part, SUM(Qty) FROM Locations WHERE [Location] != 'Qt' 
	                                GROUP BY Branch, Part
                                )

                                SELECT I.Branch, I.Part, I.Description, I.PG, I.Range, ISNULL(CAST(III.Qty as int),0) as Free, 
                                SUM(ISNULL(CAST(II.QtQty as int),0)) as QtQty, SUM(ISNULL(CAST(I.QtyOnPO as int),0)) as QtyOnPO, 
                                SUM(ISNULL(CAST(I.QtyOnPN as int),0)) as QtyOnPN, 
                                SUM(ISNULL(CAST(I.QtyOnPO as int),0) + ISNULL(CAST(I.QtyOnPN as int),0) + ISNULL(CAST(II.QtQty as int),0) + ISNULL(CAST(III.Qty as int),0)) as Total,
                                CAST(I.MIN as int) as Min, CAST(I.MAX as int) as Max, I.MNIndicator, I.MNDate, I.Cind, 
                                CAST(I.TUL12 as int) as TUL12, I.WMSSort
                                FROM CTE I LEFT OUTER JOIN
                                CTE2 II ON I.Branch = II.Branch AND I.Part = II.Part LEFT OUTER JOIN
                                CTE3 III ON I.Branch = III.Branch AND I.Part = III.Part
                                WHERE I.Branch IN({Branch}) AND PG = @PG AND I.Range = IIF(@Range IS NULL, I.Range, @Range) AND (III.Qty > 0 OR I.QtyOnPO > 0 OR (I.MAX > 0 AND MNIndicator != 'X' AND I.Cind NOT IN ('W', 'O')))
                                GROUP BY I.Branch, I.Part, I.Description, I.PG, I.Range, I.WMSSort, I.MIN, I.MAX, I.MNIndicator, I.MNDate, I.Cind, I.TUL12, I.WMSSort, III.Qty
                                ORDER BY I.WMSSort ASC
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
                    //dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

                }
                Cursor.Current = Cursors.Default;
            }
            else
            {
                string query = ($@"
                                DECLARE @PG VARCHAR(10)
                                DECLARE @Range VARCHAR(10)

                                SET @PG = '{PG}'
                                SET @Range = {Range}

                                ;WITH CTE (Branch, Part, Description, PG, Range, QtyOnPO, QtyOnPN, [MIN], [MAX], Cind, MNIndicator, WMSSort, MNDate, TUL12) as (
	                                SELECT Branch, Part, Description, PG, Range, QtyOnPO, QtyOnPN, [MIN], [MAX], Cind, MNIndicator, WMSSort30, MNDate, TotalUsageLast12 FROM VW_MERI_Pro
                                ),

                                CTE2 (Branch, Part, QtQty, QtLocn) as (
	                                SELECT Branch, Part, SUM(Qty), [Location] FROM Locations WHERE [Location] = 'Qt'
	                                GROUP BY Branch, Part, [Location]
                                ),

                                CTE3 (Branch, Part, Qty, Locn) as (
	                                SELECT Branch, Part, SUM(Qty), [Location] FROM Locations WHERE [Location] != 'Qt' 
	                                GROUP BY Branch, Part, [Location]
                                )

                                SELECT I.Branch, I.Part, I.Description, I.PG, I.Range, ISNULL(CAST(III.Qty as int),0) as Free, 
                                SUM(ISNULL(CAST(II.QtQty as int),0)) as QtQty, SUM(ISNULL(CAST(I.QtyOnPO as int),0)) as QtyOnPO, 
                                SUM(ISNULL(CAST(I.QtyOnPN as int),0)) as QtyOnPN, 
                                SUM(ISNULL(CAST(I.QtyOnPO as int),0) + ISNULL(CAST(I.QtyOnPN as int),0) + ISNULL(CAST(II.QtQty as int),0) + ISNULL(CAST(III.Qty as int),0)) as Total,
                                CAST(I.MIN as int) as Min, CAST(I.MAX as int) as Max, I.MNIndicator, I.MNDate, I.Cind, 
                                CAST(I.TUL12 as int) as TUL12, III.Locn, I.WMSSort
                                FROM CTE I LEFT OUTER JOIN
                                CTE2 II ON I.Branch = II.Branch AND I.Part = II.Part LEFT OUTER JOIN
                                CTE3 III ON I.Branch = III.Branch AND I.Part = III.Part
                                WHERE I.Branch IN({Branch}) AND PG = @PG AND I.Range = IIF(@Range IS NULL, I.Range, @Range) AND (III.Qty > 0 OR I.QtyOnPO > 0 OR (I.MAX > 0 AND MNIndicator != 'X' AND I.Cind NOT IN ('W', 'O')))
                                GROUP BY I.Branch, I.Part, I.Description, I.PG, I.Range, I.WMSSort, I.MIN, I.MAX, I.MNIndicator, I.MNDate, I.Cind, I.TUL12, II.QtLocn, III.Locn, I.WMSSort, III.Qty
                                ORDER BY I.WMSSort ASC
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
                    //dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

                }
                Cursor.Current = Cursors.Default;
            }

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
