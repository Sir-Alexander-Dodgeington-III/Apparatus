using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace productDataImport
{
    public partial class productDataForm : Form
    {
        DataTable dt = new DataTable();
        public string filePath;
        public string[] lines;
        public productDataForm()
        {
            InitializeComponent();
            this.CenterToScreen();
            dataGridView1.MouseClick += new MouseEventHandler(dataGridView1_MouseClick);
        }


        // Select file
        public void importButton_Click(object sender, EventArgs e)
        {

            OpenFileDialog OFD1 = new OpenFileDialog();
            OFD1.Filter = "csv files (*.csv)|*.csv";
            OFD1.ShowDialog();
            filePath = OFD1.FileName;
            //pathTextBox.Text = filePath;
            ReadCSV(filePath);
        }


        public void ReadCSV(string filePath)
        {
            // Get headers
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
                string[] headerLabels = firstLine.Split(',');

                //dt.Columns.Add("Row");

                foreach (string headerWord in headerLabels)
                {
                    dt.Columns.Add(new DataColumn(headerWord));
                }

                // Get cell data
                for (int r = 1; r < lines.Length; r++)
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

            }

            for (int column = 0; column < dt.Columns.Count; column++)
            {
                if (column > 3)
                {
                    dt.Columns.RemoveAt(column);
                }
            }
            dt.AcceptChanges();

            // Add to table
            if (dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            using (SqlConnection dbConnection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                dbConnection.Open();

                    string list = dt.Rows[0][2].ToString();

                    SqlCommand removeDuplicates = new SqlCommand($@"
                                                IF EXISTS (SELECT * FROM MERI_PartList WHERE List = '{list}') 
                                                BEGIN
                                                    DELETE FROM MERI_PartList WHERE  List = '{list}'
                                                END
                                    ", dbConnection);
                    removeDuplicates.CommandTimeout = 3000000;
                    removeDuplicates.ExecuteNonQuery();
                try
                {
                    using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                    {
                        s.DestinationTableName = "MERI_PartList";
                        foreach (DataColumn column in dt.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());
                        s.WriteToServer(dt);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            MessageBox.Show("Done!");
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
    }
}
