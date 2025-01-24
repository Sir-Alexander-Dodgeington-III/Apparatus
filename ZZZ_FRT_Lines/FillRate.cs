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

namespace FillRate
{
    public partial class FillRateReport : Form
    {
        public double avg;
        public DataTable table = new DataTable();
        public FillRateReport()

        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void FillRateReport_Load(object sender, EventArgs e)
        {
            table.Clear();

            Cursor.Current = Cursors.WaitCursor;
            SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
            using (SqlCommand cmd = new SqlCommand("sp_MERI_FillRateSupp1", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                string pm = Interaction.InputBox("Enter the supplier number", "Supplier");
                string pm1 = Interaction.InputBox("Enter the start date", "Start Date", "MM/DD/YYYY");
                cmd.Parameters.Add("@Supp", SqlDbType.VarChar).Value = pm;
                cmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = pm1;

                conn.Open();
                using (SqlDataAdapter a = new SqlDataAdapter(cmd))
                {
                    a.Fill(table);

                    object sumObject;
                    sumObject = table.Compute("Sum(FillRate)", string.Empty);

                    int rows = Convert.ToInt32(table.Rows.Count);
                    if (rows == 0)
                    {
                        MessageBox.Show("No records within 4 weeks of the start date provided");
                        return;
                    }
                    else
                    {
                        avg = (Convert.ToDouble(sumObject) / rows);
                    }

                    //Removing a list of duplicate items from datatable.
                    averageTextBox.Text = String.Format("{0:.##}", avg);
                    //averageTextBox.Text = avg.ToString();

                    dataGridView1.DataSource = table;
                    dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
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
                        dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
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
