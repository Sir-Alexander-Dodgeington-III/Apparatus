using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BR60GenLines_Fulfillment
{
    public partial class BR60GenLines_Fulfillment : Form
    {
        public DataTable table = new DataTable();
        public BR60GenLines_Fulfillment()
        {
            InitializeComponent();
            this.CenterToScreen();

            string[] lines = File.ReadAllLines(@"\\svrsql1\AUTOPART\VUGDocs\orders.vtm");
            //List<string> lines = new List<string>();
            if (File.Exists(@"\\svrsql1\AUTOPART\VUGDocs\orders.vtm"))
            {
                // Read the file and display it line by line.
                IEnumerable<string> selectLines = lines.Where(line => line.StartsWith("[SUN]"));
                IEnumerable<string> selectLines1 = lines.Where(line => line.StartsWith("[MON]"));
                IEnumerable<string> selectLines2 = lines.Where(line => line.StartsWith("[TUE]"));
                IEnumerable<string> selectLines3 = lines.Where(line => line.StartsWith("[WED]"));
                IEnumerable<string> selectLines4 = lines.Where(line => line.StartsWith("[THU]"));

                foreach (var item in selectLines)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    MondayTextBox.Text = item;
                    MondayTextBox.Text = MondayTextBox.Text.Replace(",", "','");
                    MondayTextBox.Text = MondayTextBox.Text.Replace("*BR60***BR60", "'");
                    MondayTextBox.Text = MondayTextBox.Text.Replace("[SUN]U=Orders@!*", "");
                }

                foreach (var item in selectLines1)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    TuesdayTextBox.Text = item;
                    TuesdayTextBox.Text = TuesdayTextBox.Text.Replace(",", "','");
                    TuesdayTextBox.Text = TuesdayTextBox.Text.Replace("*BR60***BR60", "'");
                    TuesdayTextBox.Text = TuesdayTextBox.Text.Replace("[MON]U=Orders@!*", ",'");
                }

                foreach (var item in selectLines2)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    WednesdayTextBox.Text = item;
                    WednesdayTextBox.Text = WednesdayTextBox.Text.Replace(",", "','");
                    WednesdayTextBox.Text = WednesdayTextBox.Text.Replace("*BR60***BR60", "',");
                    WednesdayTextBox.Text = WednesdayTextBox.Text.Replace("[TUE]U=Orders@!*", ",'");
                }

                foreach (var item in selectLines3)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    ThursdayTextBox.Text = item;
                    ThursdayTextBox.Text = ThursdayTextBox.Text.Replace("*BR60***BR60", "");
                    ThursdayTextBox.Text = ThursdayTextBox.Text.Replace("[WED]U=Orders@!*~", "");
                    ThursdayTextBox.Text = ThursdayTextBox.Text.Replace(",", "','");
                }

                foreach (var item in selectLines4)
                {
                    //BR60Lines_ListBox.Items.Add(item);
                    FridayTextBox.Text = item;
                    FridayTextBox.Text = FridayTextBox.Text.Replace("*BR60***BR60", "");
                    FridayTextBox.Text = FridayTextBox.Text.Replace("[THU]U=Orders@!*", "'");
                    FridayTextBox.Text = FridayTextBox.Text.Replace(",", "','");
                }
                //BR60Lines_ListBox.Items.Add(lines);
            }

            string pgs = MondayTextBox.Text + TuesdayTextBox.Text + WednesdayTextBox.Text + FridayTextBox.Text;
            string rngs = ThursdayTextBox.Text;

            SqlConnection conn = new System.Data.SqlClient.SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();
            string query = ($@"
                                select TOP(1) stuff(( select ', ' + Branch from [AUTOPART].[dbo].[Branches] where branchGroup = 'BR60' AND RIGHT(Branch,2) > 61 AND RIGHT(Branch,2) < 90 for xml path(''), type).value('(./text())[1]','varchar(max)') , 1, 2, '') as single_value from [AUTOPART].[dbo].[Branches] WHERE branchGroup = 'BR60' AND RIGHT(Branch,2) > 61 AND RIGHT(Branch,2) < 90
                            ");
            SqlCommand cmd = new SqlCommand(query, conn);
            string branches = (string)cmd.ExecuteScalar();
            branchTextBox.Text = branches;
            branchTextBox.Text = branchTextBox.Text.Replace(",", "','");
            branchTextBox.Text = branchTextBox.Text.Replace(" ", "");
            branches = branchTextBox.Text;
            conn.Close();

            Cursor.Current = Cursors.WaitCursor;
            //SqlConnection conn = new System.Data.SqlClient.SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();

            string query1 = ($@"
                            SELECT VP.Part, VP.Branch, CAST(VP.Max-VP.Free-VP.QtyOnPO+VP.QtyOnBO as int) AS Needed, CAST(S30.Free as int) as BR30Free, CAST(S30.Max as int) AS BR30Max
                            FROM VW_MERI_Pro VP
                            INNER JOIN Stock (nolock) S30 ON VP.Part = S30.Part and S30.Branch = 'BR30'
                            INNER JOIN Stock (nolock) S60 ON VP.Part = S60.Part AND s60.Branch = 'BR60'
                            WHERE VP.Branch IN ('{branches}') AND
                            VP.PG IN ('{pgs}') AND
                            VP.Free < VP.Min and VP.ExcFromReplen <> 'Y' AND VP.ExcFromReplenPG = '' AND VP.ExcFromReplenRange = '' and VP.BR30ExFromReplen <> 'Y' AND S60.MNIndicator <> 'X' AND
                            VP.Part IN (SELECT Part FROM Stock (nolock) WHERE Branch = 'BR30' aND Free > 0 and PG IN ('{pgs}')) AND
                            VP.Part IN (SELECT Part From Stock (nolock) WHERE Branch = 'BR60' AND Free = 0 AND PG IN ('{pgs}'))
                            AND VP.Max-VP.Free-VP.QtyOnPO+VP.QtyOnBO > 0
                            UNION
                            SELECT VP.Part, VP.Branch, CAST(VP.Max-VP.Free-VP.QtyOnPO+VP.QtyOnBO as int) AS Needed, CAST(S30.Free as int) as BR30Free, CAST(S30.Max as int) AS BR30Max
                            FROM VW_MERI_Pro VP
                            INNER JOIN Stock (nolock) S30 ON VP.Part = S30.Part and S30.Branch = 'BR30'
                            INNER JOIN Stock (nolock) S60 ON VP.Part = S60.Part AND s60.Branch = 'BR60'
                            WHERE VP.Branch IN ('{branches}') AND
                            VP.Range IN ('{rngs}') AND
                            VP.Free < VP.Min and VP.ExcFromReplen <> 'Y' AND VP.ExcFromReplenPG = '' AND VP.ExcFromReplenRange = '' and VP.BR30ExFromReplen <> 'Y' AND S60.MNIndicator <> 'X' AND
                            VP.Part IN (SELECT Part FROM Stock (nolock) WHERE Branch = 'BR30' aND Free > 0 and Range IN ('{rngs}')) AND
                            VP.Part IN (SELECT Part From Stock (nolock) WHERE Branch = 'BR60' AND Free = 0 AND Range IN ('{rngs}'))
                            AND VP.Max-VP.Free-VP.QtyOnPO+VP.QtyOnBO > 0
                            Order BY VP.Branch
            ");

            SqlCommand cmd1 = new SqlCommand(query1, conn);
            cmd1.CommandTimeout = 300000;
            using (SqlDataAdapter a = new SqlDataAdapter(cmd1))
            {
                a.Fill(table);
                dataGridView1.DataSource = table;
                Cursor.Current = Cursors.Default;
            }

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
                        dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                        Clipboard.SetDataObject(this.dataGridView1.GetClipboardContent());
                    }
                    break;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}