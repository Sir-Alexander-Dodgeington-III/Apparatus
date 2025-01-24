using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LastReceived
{
    public partial class LastReceived : Form
    {
        public DataTable table = new DataTable();
        public LastReceived()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            pgTextBox.Text = pgTextBox.Text.ToUpper();
            rangeTextBox.Text = rangeTextBox.Text.ToUpper();
            branchTextBox.Text = branchTextBox.Text.ToUpper();
            Cursor.Current = Cursors.WaitCursor;
            SqlConnection conn = new System.Data.SqlClient.SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();

            string pg = "'" + pgTextBox.Text + "'";
            string range = "'" + rangeTextBox.Text + "'";
            string branch = "'" + branchTextBox.Text + "'";

            if (String.IsNullOrEmpty(pgTextBox.Text))
            {
                pg = "NULL";
            }
            if (String.IsNullOrEmpty(rangeTextBox.Text))
            {
                range = "NULL";
            }
            if (String.IsNullOrEmpty(branchTextBox.Text))
            {
                branch = "NULL";
            }


            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();

            try
            {
                string query = ($@"

                                DECLARE @PG VARCHAR(10)
                                DECLARE @Range VARCHAR(10)
                                DECLARE @Branch VARCHAR(10)

                                SET @PG = {pg}
                                SET @Range = {range}
                                SET @Branch = {branch}

                                SELECT        L.Part, MAX(L.Datetime) AS DateTime, R.Supp
                                FROM            LocMovements L LEFT OUTER JOIN
                                                         Product P ON L.Part = P.KeyCode LEFT OUTER JOIN
                                                         RHeads R ON L.SourceDocument = R.[Document]
                                WHERE        P.PG = IIF(@PG IS NULL, PG, @PG) AND L.Branch = IIF(@Branch IS NULL, L.Branch, @Branch) AND P.Range = IIF(@Range IS NULL, Range, @Range) AND R.Supp IS NOT NULL AND L.SourceDocument LIKE '%GR%'
                                GROUP BY L.Part, R.Supp
                                Order By L.Part, DateTime Asc
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

                    Hashtable hTable = new Hashtable();
                    ArrayList duplicateList = new ArrayList();

                    foreach (DataRow drow in table.Rows)
                    {
                        if (hTable.Contains(drow["Part"]))
                        {
                            duplicateList.Add(drow);
                        }
                        else
                        {
                            hTable.Add(drow["Part"], string.Empty);
                        }

                    }

                    //Removing a list of duplicate items from datatable.
                    foreach (DataRow dRow in duplicateList)
                    {
                        table.Rows.Remove(dRow);
                    }

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
                        dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                        Clipboard.SetDataObject(this.dataGridView1.GetClipboardContent());
                    }
                    break;
            }
        }

        public DataTable RemoveDuplicates(DataTable table, String colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            foreach (DataRow drow in table.Rows)
            {
                if (hTable.Contains(drow["Part"]))
                {
                    duplicateList.Add(drow);
                }
                else
                {
                    hTable.Add(drow["Part"], string.Empty);
                }

            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
            {
                table.Rows.Remove(dRow);
            }


            //Datatable which contains unique records will be return as output.
            return table;
        }



    }
}
