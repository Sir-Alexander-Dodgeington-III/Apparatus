
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace FVBC
{
    public partial class FVBC : Form
    {
        public DataTable table = new DataTable();
        public FVBC()
        {
            InitializeComponent();
            this.CenterToScreen();
            this.dataGridView1.VirtualMode = true;
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            table.Clear();

            Cursor.Current = Cursors.WaitCursor;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();

            string pg = "'" + pgTextBox.Text + "'";
            string range = "'" + rangeTextBox.Text + "'";

            if (String.IsNullOrEmpty(pgTextBox.Text))
            {
                pg = "NULL";
            }
            if (String.IsNullOrEmpty(rangeTextBox.Text))
            {
                range = "NULL";
            }

            try
            {
                string query = ($@"

                                    DECLARE @PG VARCHAR(10)
                                    DECLARE @Range VARCHAR(10)

                                    SET @PG = {pg}
                                    SET @Range = {range}

                                    ;WITH CTE (PG, Range, Branch, Part, Free, OldValue, NewValue, Change) as (
                                    SELECT        
                                    IIF(@PG IS NULL, '', @PG) as PG,
                                    IIF(@Range IS NULL, '', @Range) as 'Range',
                                    S.Branch,
                                    '' as Part, 
                                    CAST(S.Free as int) as Free, 
                                    CAST(B.OldValue as decimal(18,2)) as OldValue, 
                                    CAST(B.NewValue as decimal(18,2)) as NewValue, 
                                    (TRY_CONVERT(decimal(18,2), B.NewValue) - TRY_CONVERT(decimal(18,2), B.OldValue)) * S.Free AS Change
                                    FROM            
                                    dbo.BLMaint AS B WITH (nolock) INNER JOIN
                                    dbo.Product AS P WITH (nolock) ON B.PartSpec = P.KeyCode INNER JOIN
                                    dbo.Stock AS S WITH (nolock) ON B.PartSpec = S.Part AND S.Free > 0
                                    WHERE        (B.Key1 = 'S') AND (P.PG = IIF(@PG IS NULL, P.PG, @PG)) AND (P.Range = IIF(@Range IS NULL, P.Range, @Range)) AND (B.Key2 = CASE WHEN s.Branch IN('BR30','BR60','BR51','BR61','BR50','BR48') THEN '27' ELSE '26' END)
                                    Group By S.Branch, S.Free, B.OldValue, B.NewValue)

                                    SELECT 
                                    CTE.PG,
                                    CTE.Range,
                                    CTE.Branch,
                                    CTE.Part, 
                                    SUM(Free) as Free, 
                                    SUM(OldValue) as OldValue, 
                                    SUM(NewValue) as NewValue, 
                                    CAST(SUM(Change) as decimal(18,2)) as Change
                                    FROM CTE  
                                    Group By CTE.Branch, CTE.PG, CTE.Range, CTE.Part

                                    UNION ALL

                                    SELECT 
                                    '' as PG,
                                    '' as 'Range',
                                    '' as Branch,
                                    '' as Part,
                                    SUM(Free) as Free, 
                                    SUM(OldValue) as OldValue, 
                                    SUM(NewValue) as NewValue, 
                                    SUM(Change) as Change
                                    FROM CTE  

                                 ");

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandTimeout = 300000;
                using (SqlDataAdapter a = new SqlDataAdapter(cmd))
                {
                    a.Fill(table);
                    dataGridView1.DataSource = table;
                    //dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            dataGridView1.MouseClick += new MouseEventHandler(dataGridView1_MouseClick);

        }

        private void showDetailButton_Click(object sender, EventArgs e)
        {
            table.Clear();

            Cursor.Current = Cursors.WaitCursor;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();

            string pg = "'" + pgTextBox.Text + "'";
            string range = "'" + rangeTextBox.Text + "'";

            if (String.IsNullOrEmpty(pgTextBox.Text))
            {
                pg = "NULL";
            }
            if (String.IsNullOrEmpty(rangeTextBox.Text))
            {
                range = "NULL";
            }

            try
            {
                string query = ($@"
                                    DECLARE @PG VARCHAR(10)
                                    DECLARE @Range VARCHAR(10)

                                    SET @PG = {pg}
                                    SET @Range = {range}

                                    ;WITH CTE (PG, Range, Branch, Part, Free, OldValue, NewValue, Change) as (
                                    SELECT        
                                    IIF(@PG IS NULL, '', @PG) as PG,
                                    IIF(@Range IS NULL, '', @Range) as 'Range',
                                    s.Branch,
                                    s.Part, 
                                    CAST(S.Free as int) as Free, 
                                    CAST(B.OldValue as decimal(18,2)) as OldValue, 
                                    CAST(B.NewValue as decimal(18,2)) as NewValue, 
                                    (TRY_CONVERT(decimal(18,2), B.NewValue) - TRY_CONVERT(decimal(18,2), B.OldValue)) * S.Free AS Change
                                    FROM            
                                    dbo.BlMaint AS B WITH (nolock) INNER JOIN
                                    dbo.Product AS P WITH (nolock) ON B.PartSpec = P.KeyCode INNER JOIN
                                    dbo.Stock AS S WITH (nolock) ON B.PartSpec = S.Part AND S.Free > 0
                                    WHERE        (B.Key1 = 'S') AND (P.PG = IIF(@PG IS NULL, P.PG, @PG)) AND (P.Range = IIF(@Range IS NULL, P.Range, @Range)) AND (B.Key2 = CASE WHEN s.Branch IN('BR30','BR60','BR51','BR61','BR50','BR48') THEN '27' ELSE '26' END)
                                    Group By S.Branch, S.Part, S.Free, B.OldValue, B.NewValue)

                                    SELECT 
                                    IIF(@PG IS NULL, '', @PG) as PG,
                                    IIF(@Range IS NULL, '', @Range) as 'Range',
                                    Branch, 
                                    Part, 
                                    SUM(Free) as Free, 
                                    SUM(OldValue) as OldValue, 
                                    SUM(NewValue) as NewValue, 
                                    CAST(SUM(Change) as decimal(18,2)) as Change
                                    FROM CTE 
                                    GROUP BY CTE.Branch, CTE.Part

                                    /*UNION ALL

                                    SELECT 
                                    IIF(@PG IS NULL, '', @PG) as PG,
                                    IIF(@Range IS NULL, '', @Range) as 'Range',
                                    '' as Branch, 
                                    '' as Part, 
                                    SUM(Free) as Free, 
                                    SUM(OldValue) as OldValue, 
                                    SUM(NewValue) as NewValue, 
                                    SUM(Change) as Change
                                    FROM CTE*/
                                    Order By CTE.Branch, CTE.Part
                                 ");

                SqlCommand cmd = new SqlCommand(query, conn);
                using (SqlDataAdapter a = new SqlDataAdapter(cmd))
                {
                    a.Fill(table);
                    dataGridView1.DataSource = table;
                    //dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            dataGridView1.MouseClick += new MouseEventHandler(dataGridView1_MouseClick);
        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
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
    }
}
