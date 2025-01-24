using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OnBOonSU
{
    public partial class OnBOonPO : Form
    {
        public DataTable table = new DataTable();
        public OnBOonPO()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                listBox1.Items.Add("'" + pgTextBox.Text.ToUpper() + "'");
                pgTextBox.Text = "";
            }
            else
            {
                listBox1.Items.Add(",'" + pgTextBox.Text.ToUpper() + "'");
                pgTextBox.Text = "";
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            string text = "";
            foreach (var item in listBox1.Items)
            {
                text += item.ToString(); // /n to print each item on new line or you omit /n to print text on same line
            }
            //textBox1.Text = text;

            table.Clear();

            Cursor.Current = Cursors.WaitCursor;
            string PGs = text;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            conn.Open();


            string query = ($@"
                                SELECT        BLines.[Document], CAST(BHeads.DateTime as date) as Date, BHeads.Acct, Customer.Name, SUM(BLines.Qty * BLines.Unit) as total, Customer.Rep, BLines.PG
                                FROM            BLines INNER JOIN
                                                         BHeads ON BLines.[Document] = BHeads.[Document] INNER JOIN
                                                         Customer ON BHeads.Acct = Customer.KeyCode
                                WHERE        BLines.PG IN ({ PGs }) AND (BLines.CQty = BLines.Qty) AND Customer.LType <> 'NONE'
                                GROUP BY BLines.[Document], BHeads.Acct, Customer.Name, Customer.Rep, BLines.PG, CAST(BHeads.DateTime as date)
                                UNION ALL
                                SELECT        ULines.[Document], CAST(UHeads.DateTime as date) as Date, UHeads.Acct, Customer.Name, SUM(ULines.Qty * ULines.Unit) as total, Customer.Rep, ULines.PG
                                FROM            ULines INNER JOIN
                                                         UHeads ON ULines.[Document] = UHeads.[Document] INNER JOIN
                                                         Customer ON UHeads.Acct = Customer.KeyCode
                                WHERE        ULines.PG IN ({PGs}) AND Customer.LType <> 'NONE'
                                GROUP BY ULines.[Document], UHeads.Acct, Customer.Name, Customer.Rep, ULines.PG, CAST(UHeads.DateTime as date)
                            ");


            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.CommandTimeout = 300000;
            using (SqlDataAdapter a = new SqlDataAdapter(cmd))
            {
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

        private void pgTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                AddButton_Click((object)sender, (EventArgs)e);
            }
            if (e.KeyChar == (char)Keys.F5)
            {
                searchButton_Click((object)sender, (EventArgs)e);
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

        private void removeButton_Click(object sender, EventArgs e)
        {
            while (listBox1.SelectedItems.Count > 0)
            {
                listBox1.Items.Remove(listBox1.SelectedItems[0]);
            }
        }
    }
}
