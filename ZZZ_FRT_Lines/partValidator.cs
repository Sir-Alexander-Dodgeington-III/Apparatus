using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace PartValidator
{
    public partial class IsPart : Form
    {
        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();
        public string filePath;
        public string[] lines;
        public bool importManual = false;
        public int x = 0;
        public IsPart()
        {
            InitializeComponent();
            this.CenterToScreen();
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
    
            // Get headers
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
                    if(dt.Rows[0][0].ToString() != "KeyCode")
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

        private void validateButton_Click(object sender, EventArgs e)
        {
            if (importManual == true)
            {
                Cursor.Current = Cursors.WaitCursor;
                int i = 0;
                if (exactMatchCheckbox.Checked)
                {
                    dataGridView1.Columns.Add("Cind", "Cind");
                    dataGridView1.Columns.Add("Range", "Range");
                    dataGridView1.Columns.Add("Description", "Description");
                    dataGridView1.Columns.Add("A1", "A1");
                    dataGridView1.Columns.Add("A2", "A2");
                    dataGridView1.Columns.Add("A3", "A3");
                    dataGridView1.Columns.Add("A4", "A4");
                    dataGridView1.Columns.Add("P8", "P8");
                    dataGridView1.Columns.Add("P10", "P10");
                    dataGridView1.Columns.Add("P11", "P11");
                    dataGridView1.Columns.Add("CorePrice", "CorePrice");
                    dataGridView1.Columns.Add("CoreCost", "CoreCost");
                    dataGridView1.Columns.Add("NPD", "NPD");
                    dataGridView1.Columns.Add("MAP", "MAP");
                }
                else if (possibleMatchCheckbox.Checked)
                {
                    dataGridView1.Columns.Add("Possible Matches", "Possible Matches");
                }

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (i != dataGridView1.Rows.Count - 1)
                    {
                        var part = dataGridView1.Rows[i].Cells[0].Value.ToString();
                        SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                        {
                            conn.Open();
                            //DataSet ds1 = new DataSet();
                            if (exactMatchCheckbox.Checked)
                            {
                                SqlDataAdapter adapter = new SqlDataAdapter(
                                $@"
                                    SELECT 
                                        P.keycode, 
                                        Pri.Cind, 
                                        Pri.Range, 
                                        Pri.Description, 
                                        Pri.A1, 
                                        Pri.A2, 
                                        Pri.A3, 
                                        Pri.A4, 
                                        Pri.P8, 
                                        Pri.P10, 
                                        Pri.P11, 
                                        Pri.CorePrice, 
                                        Pri.CoreCost, 
                                        Pri.NPD, 
                                        CASE 
                                        WHEN Pri.MAP IS NULL THEN '0.00' 
                                        WHEN Pri.Map = '' THEN '0.00' 
                                        ELSE Pri.MAP 
                                        END as MAP
                                    FROM Product P INNER JOIN 
                                    VW_MERI_Pri Pri ON P.KeyCode = Pri.Part WHERE P.KeyCode = '{part}'
                                   ", conn);
                                adapter.Fill(dt1);
                                conn.Close();
                            }
                            else if (possibleMatchCheckbox.Checked)
                            {
                                SqlDataAdapter adapter = new SqlDataAdapter(
                                $@"
                                    SELECT 
                                        P.Word5,
                                        STUFF((SELECT ',' + P.keycode
                                        FROM Product P WHERE Word5 = '{part}'
                                        FOR XML PATH('')), 1, 1, '') as 'Possible Matches'
                                    FROM Product P WHERE P.Word5 = '{part}'
                                   ", conn);
                                adapter.Fill(dt1);
                                conn.Close();
                            }
                        }

                        if (dt1.Rows.Count == 0)
                        {
                            row.DefaultCellStyle.BackColor = Color.Red;
                            x++;
                        }
                        else
                        {
                            int countCol = dt1.Columns.Count;
                            if (exactMatchCheckbox.Checked)
                            {
                                dataGridView1.Rows[i].Cells[0].Value = dt1.Rows[0][0].ToString();
                                dataGridView1.Rows[i].Cells[1].Value = dt1.Rows[0][1].ToString();
                                dataGridView1.Rows[i].Cells[2].Value = dt1.Rows[0][2].ToString();
                                dataGridView1.Rows[i].Cells[3].Value = dt1.Rows[0][3].ToString();
                                dataGridView1.Rows[i].Cells[4].Value = Convert.ToDecimal(dt1.Rows[0][4]).ToString("#.##");
                                dataGridView1.Rows[i].Cells[5].Value = Convert.ToDecimal(dt1.Rows[0][5]).ToString("#.##");
                                dataGridView1.Rows[i].Cells[6].Value = Convert.ToDecimal(dt1.Rows[0][6]).ToString("#.##");
                                dataGridView1.Rows[i].Cells[7].Value = Convert.ToDecimal(dt1.Rows[0][7]).ToString("#.##");
                                dataGridView1.Rows[i].Cells[8].Value = Convert.ToDecimal(dt1.Rows[0][8]).ToString("#.##");
                                dataGridView1.Rows[i].Cells[9].Value = Convert.ToDecimal(dt1.Rows[0][9]).ToString("#.##");
                                dataGridView1.Rows[i].Cells[10].Value = Convert.ToDecimal(dt1.Rows[0][10]).ToString("#.##");
                                dataGridView1.Rows[i].Cells[11].Value = Convert.ToDecimal(dt1.Rows[0][11]).ToString("#.##");
                                dataGridView1.Rows[i].Cells[12].Value = Convert.ToDecimal(dt1.Rows[0][12]).ToString("#.##");
                                //string test = dataGridView1.Rows[i].Cells[13].Value.ToString();
                                //string test1 = dataGridView1.Rows[i].Cells[14].Value.ToString();
                                dataGridView1.Rows[i].Cells[13].Value = Convert.ToDecimal(dt1.Rows[0][13]).ToString("#.##");
                                dataGridView1.Rows[i].Cells[14].Value = Convert.ToDecimal(dt1.Rows[0][14]).ToString("#.##");

                            }
                            else if (possibleMatchCheckbox.Checked)
                            {
                                dataGridView1.Rows[i].Cells[1].Value = dt1.Rows[0][1].ToString();
                            }
                        }
                        i++;
                    }
                    dt1.Rows.Clear();
                }
                MessageBox.Show($"There were a total of {x} invalid parts");
                Cursor.Current = Cursors.Default;
            }
            else
            {
                dt.Clear();
                Cursor.Current = Cursors.WaitCursor;
                SqlConnection conn = new SqlConnection(Helper.ConnString("AUTOPART"));
                {
                    conn.Open();
                    //DataSet ds1 = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(
                    $@"
                    SELECT 
                        PV.KeyCode, 
                        Pri.Cind, 
                        Pri.Range, 
                        Pri.Description, 
                        CAST(Pri.A1 as decimal(18,2)) as A1, 
                        CAST(Pri.A2 as decimal(18,2)) as A2, 
                        CAST(Pri.A3 as decimal(18,2)) as A3, 
                        CAST(Pri.A4 as decimal(18,2)) as A4, 
                        CAST(Pri.P8 as decimal(18,2)) as P8, 
                        CAST(Pri.P10 as decimal(18,2)) as P10, 
                        CAST(Pri.P11 as decimal(18,2)) as P11, 
                        CAST(Pri.CorePrice as decimal(18,2)) as CorePrice, 
                        CAST(Pri.CoreCost as decimal(18,2)) as CoreCost, 
                        CAST(Pri.NPD as decimal(18,2)) as NPD,
                        Pri.MAP
                    FROM MERI_APA_PartValidator PV LEFT OUTER JOIN 
                    VW_MERI_Pri Pri ON PV.KeyCode = Pri.Part
                    ", conn);
                    adapter.Fill(dt);
                    
                    if (dt.Rows.Count > 0)
                    {
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = dt;
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
                    if (row.Cells["Range"].Value == null)
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                        x++;
                    }
                    else if (row.Cells["Range"].Value.ToString() == "")
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                        x++;
                    }
                }
                MessageBox.Show($"There were a total of {x} invalid parts");
                Cursor.Current = Cursors.Default;
            }
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

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            importManual = true;
            importButton.Enabled = false;
            button1.Enabled = false;
            //dataGridView1.Columns.Add("Row", "Row");
            dataGridView1.Columns.Add("Part", "Part");
            dataGridView1.Rows.Add();

            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
            dataGridView1.BeginEdit(true);
        }

        private void exactMatchCheckbox_Click(object sender, EventArgs e)
        {
            exactMatchCheckbox.Checked = true;
            possibleMatchCheckbox.Checked = false;
        }

        private void possibleMatchCheckbox_Click(object sender, EventArgs e)
        {
            possibleMatchCheckbox.Checked = true;
            exactMatchCheckbox.Checked = false;
            importManual = true;
        }
    }
}
