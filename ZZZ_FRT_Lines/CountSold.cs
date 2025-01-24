using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CountSold
{
    public partial class CountSold : Form
    {

        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();
        DataTable dt2 = new DataTable();
        public string filePath;
        public string[] lines;
        public bool importManual = false;
        public int x = 0;

        public CountSold()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            importManual = false;
            OpenFileDialog OFD1 = new OpenFileDialog();
            OFD1.Filter = "csv files (*.csv)|*.csv";
            OFD1.ShowDialog();
            filePath = OFD1.FileName;
            //pathTextBox.Text = filePath;
            ReadCSV(filePath);
        }

        private void ReadCSV(string filePath)
        {
            int i = 1;
            try
            {
                lines = File.ReadAllLines(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (lines.Length > 0)
            {
                string firstLine = lines[0];

                if (firstLine != "KeyCode")
                {
                    firstLine = "KeyCode";
                }

                string[] headerLabels = firstLine.Split(',');

                //dt.Columns.Add("Row");

                foreach (string headerWord in headerLabels)
                {
                    dt.Columns.Add(new DataColumn(headerWord));
                }

                // Get cell data
                for (int r = 0; r < lines.Length; r++)
                {
                    string[] dataWords = lines[r].Split(',');
                    DataRow dr = dt.NewRow();
                    int columnIndex = 0;
                    foreach (string headerWord in headerLabels)
                    {
                        //dr["Row"] = i;
                        dr[headerWord] = dataWords[columnIndex++];
                    }
                    dt.Rows.Add(dr);
                    i++;
                }

                // Add to table
                if (dt.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dt;
                    if (dt.Rows[0][0].ToString() != "KeyCode")
                    {
                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = "KeyCode";
                        dt.Rows.Add(dr);
                    }
                    InsertDataIntoSQLServerUsingSQLBulkCopy(dt);
                }
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
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

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(currFile, sb.ToString());
            MessageBox.Show("Saved to " + currFile);
            Cursor.Current = Cursors.Default;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            importManual = true;
            importButton.Enabled = false;
            button1.Enabled = false;
            //dataGridView1.Columns.Add("Row", "Row");
            dataGridView1.Columns.Add("Part", "Part");
            dataGridView1.Rows.Add();
        }

        private void validateButton_Click(object sender, EventArgs e)
        {
            if (importManual == true)
            {
                Cursor.Current = Cursors.WaitCursor;
                int i = 0;

                dataGridView1.Columns.Add("TotalQty", "TotalQty");

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (i != dataGridView1.Rows.Count - 1)
                    {
                        var part = dataGridView1.Rows[i].Cells[0].Value.ToString();
                        SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                        {
                            conn.Open();
                            //DataSet ds1 = new DataSet();

                                SqlDataAdapter adapter = new SqlDataAdapter(
                                $@"
                                    SELECT Part, CAST(SUM(Qty) as int) FROM VW_MERI_I  WHERE Part = '{part}' AND CAST(DateTime as date) >= CAST(DATEADD(YEAR, -3,GETDATE()) as date) AND LType = 'NONE' GROUP BY Part
                                   ", conn);
                                adapter.Fill(dt1);
                                conn.Close();
                        }

                        if (dt1.Rows.Count == 0)
                        {
                            row.DefaultCellStyle.BackColor = Color.Red;
                            x++;
                        }
                        else
                        {
                            dataGridView1.Rows[i].Cells[1].Value = dt1.Rows[0][1].ToString();
                        }

                        i++;
                    }
                    dt1.Rows.Clear();
                }
                MessageBox.Show($"There were a total of {x} with no sales");
                Cursor.Current = Cursors.Default;
            }
            else
            {
                dataGridView1.DataSource = null;
                dt2.Clear();
                dt2.AcceptChanges();
                Cursor.Current = Cursors.WaitCursor;
                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                {
                    conn.Open();
                    //DataSet ds1 = new DataSet();

                    SqlDataAdapter adapter = new SqlDataAdapter(
                    $@"
                        WITH CTE1 (Part) as (
                            SELECT KeyCode FROM MERI_APA_PartValidator
                        ),

                        CTE2 (Part, QTY) as (
                            SELECT Part, SUM(Qty) FROM VW_MERI_I WHERE CAST(DateTime as date) >= CAST(DATEADD(YEAR, -3,GETDATE()) as date) AND LType = 'NONE' GROUP BY Part
                        )

                        SELECT I.Part, ISNULL(CAST(II.Qty as int),0) as TotalQty FROM CTE1 I LEFT OUTER JOIN CTE2 II ON I.Part = II.Part
                    ", conn);
                    adapter.Fill(dt2);

                    if (dt2.Rows.Count > 0)
                    {
                        dataGridView1.DataSource = dt2;
                        conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("nothing found");
                        return;
                    }

                }

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    //string value = row.Cells["Range"].Value.ToString();
                    if (Convert.ToInt32(row.Cells["TotalQty"].Value) == 0)
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                        x++;
                    }
                    else if (row.Cells["TotalQty"].Value.ToString() == "")
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                        x++;
                    }
                }
                MessageBox.Show($"There were a total of {x} with no sales");
                Cursor.Current = Cursors.Default;
            }
        }

        static void InsertDataIntoSQLServerUsingSQLBulkCopy(DataTable csvFileData)
        {
            using (SqlConnection dbConnection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {

                dbConnection.Open();

                string query = "DELETE FROM MERI_APA_PartValidator";
                SqlCommand command = new SqlCommand(query, dbConnection);
                command.ExecuteNonQuery();

                using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                {
                    s.DestinationTableName = "MERI_APA_PartValidator";
                    foreach (var column in csvFileData.Columns)
                        s.ColumnMappings.Add(column.ToString(), column.ToString());
                    s.WriteToServer(csvFileData);
                }
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
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

                    IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                                      Select(column => column.ColumnName);
                    sb.AppendLine(string.Join(",", columnNames));

                    foreach (DataRow row in dt.Rows)
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


    }
}
