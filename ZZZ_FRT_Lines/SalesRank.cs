using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace SalesRank
{
    public partial class SalesRank : Form
    {

        public DataTable table = new DataTable();
        public SalesRank()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string search = Interaction.InputBox("Please enter a PG or Range", "Enter PG or Range");
            var product = "";

            if (search.Length > 3)
            {
                product = "range";
            }
            else
            {
                product = "PG";
            }

            Cursor.Current = Cursors.WaitCursor;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();


            string query = ($@"
                                WITH CTE (Branch) as (
                                    SELECT Distinct Branch from Branches
                                ),

                                CTE2 (Branch, {product}, Local15, Free, Max) as (
                                    SELECT Branch, {product}, SUM(LocalUsageLast15), SUM(Free), SUM(MAX) FROM VW_MERI_Pro GROUP BY Branch, {product}
                                ),

                                CTE3 ({product}, Usage) as (
                                    SELECT {product}, SUM(LocalUsageLast15) FROM VW_MERI_Pro WHERE {product} = '{search}' GROUP BY {product}
                                ),

                                CTE4 (Branch, {product}, Qty, [Rank]) as (
                                    SELECT Branch, {product}, SUM(FREE), ROW_Number() OVER(ORDER BY SUM(FREE) DESC) AS [Rank] FROM VW_MERI_Pro
                                    WHERE {product} = '{search}' AND Branch NOT IN('BR30','BR60')
                                    GROUP BY Branch, {product}

                                )

                                SELECT I.Branch, II.{product}, CAST(II.Local15 as int) as Local15, CAST(II.Free as int) as Free, CAST(II.Max as int) as 'MAX', CAST(SUM((II.Local15) / (III.Usage)) * 100 as int) as '% Usage', ROW_NUMBER() OVER (ORDER BY II.Local15 DESC) as 'Sales Rank', IV.Rank as 'INV Rank', CAST(ROW_NUMBER() OVER (ORDER BY II.Local15 DESC) - IV.Rank as int) as Diff
                                FROM CTE I LEFT OUTER JOIN
                                CTE2 II ON I.Branch = II.Branch INNER JOIN
                                CTE3 III ON II.{product} = III.{product} INNER JOIN
                                CTE4 IV ON I.Branch = IV.Branch
                                WHERE II.{product} = '{search}' AND II.Local15 > 0 AND I.Branch NOT IN ('BR30', 'BR60') AND III.Usage > 0
                                GROUP BY I.Branch, II.{product}, II.Local15, II.Free, II.Max, IV.Rank
                            ");

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.CommandTimeout = 300000;
            using (SqlDataAdapter a = new SqlDataAdapter(cmd))
            {
                a.Fill(table);
                dataGridView1.DataSource = table;
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
    
        private void quitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
